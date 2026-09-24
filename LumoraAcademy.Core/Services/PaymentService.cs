using LumoraAcademy.Core.Database;
using LumoraAcademy.Core.Entities;

namespace LumoraAcademy.Core.Services;

// Epic 3 - Payment Management (FR-05, FR-06).
// All fees are paid in cash at the office. This is the digital version of the fee register:
// pick a class and a month, then tick off the students who have paid.
public class PaymentService
{
    private readonly AppDatabase _db;
    private readonly ClassService _classes;

    public PaymentService(AppDatabase db, ClassService classes)
    {
        _db = db;
        _classes = classes;
    }

    // Month keys look like "2026-09".
    public static string MonthKey(DateTime date) => date.ToString("yyyy-MM");
    public static string CurrentMonth => MonthKey(DateTime.Today);

    // ---------- Status rules (FR-06) ----------
    // Paid              -> the full fee has been received
    // Partial           -> some money received, not all
    // Extension Granted -> not paid, but an approved extension date is still ahead
    // Overdue           -> not paid and past the due date
    // Pending           -> not paid, due date not reached yet
    public static string StatusFor(Payment p, DateTime? today = null)
    {
        var now = (today ?? DateTime.Today).Date;

        if (p.AmountPaid >= p.AmountDue && p.AmountDue > 0) return "Paid";
        if (p.AmountPaid > 0) return "Partial";
        if (p.ExtensionUntil.HasValue && now <= p.ExtensionUntil.Value.Date) return "Extension Granted";
        if (now > p.DueDate.Date) return "Overdue";
        return "Pending";
    }

    // ---------- The fee register screen ----------

    // One row per enrolled student for that class and month.
    // Rows that do not exist yet are created automatically with the class's monthly fee.
    public List<Payment> GetRegister(int classGroupId, string month)
    {
        var group = _db.Connection.Get<ClassGroup>(classGroupId);
        var students = _classes.GetStudents(classGroupId);
        var existing = _db.Connection.Table<Payment>()
            .Where(p => p.ClassGroupId == classGroupId && p.Month == month)
            .ToList()
            .ToDictionary(p => p.StudentId);

        var rows = new List<Payment>();

        foreach (var s in students)
        {
            if (existing.TryGetValue(s.Id, out var payment))
            {
                rows.Add(payment);
            }
            else
            {
                var fee = new Payment
                {
                    StudentId = s.Id,
                    ClassGroupId = classGroupId,
                    Month = month,
                    AmountDue = group.MonthlyFee,
                    AmountPaid = 0,
                    DueDate = DueDateFor(month),
                };
                _db.Connection.Insert(fee);
                rows.Add(fee);
            }
        }

        FillDetails(rows);
        return rows.OrderBy(p => p.StudentName).ToList();
    }

    // Marks a student as having paid the full fee in cash (the main action on the register).
    public Payment MarkPaid(int paymentId, DateTime? paidOn = null)
    {
        var payment = _db.Connection.Get<Payment>(paymentId);
        payment.AmountPaid = payment.AmountDue;
        payment.PaymentDate = (paidOn ?? DateTime.Today).Date;
        if (string.IsNullOrWhiteSpace(payment.ReceiptNumber)) payment.ReceiptNumber = NextReceiptNumber();
        _db.Connection.Update(payment);

        FillDetails(new[] { payment });
        return payment;
    }

    // Undo - the student has not paid after all.
    public Payment MarkUnpaid(int paymentId)
    {
        var payment = _db.Connection.Get<Payment>(paymentId);
        payment.AmountPaid = 0;
        payment.PaymentDate = null;
        payment.ReceiptNumber = "";
        _db.Connection.Update(payment);

        FillDetails(new[] { payment });
        return payment;
    }

    // FR-05: record a part payment (amount and date are mandatory).
    public Payment RecordPayment(int paymentId, decimal amount, DateTime paymentDate, string remarks = "")
    {
        if (amount <= 0) throw new ArgumentException("Payment amount must be greater than zero.");
        if (paymentDate == default) throw new ArgumentException("Payment date is required.");

        var payment = _db.Connection.Get<Payment>(paymentId);
        payment.AmountPaid += amount;
        payment.PaymentDate = paymentDate.Date;
        if (!string.IsNullOrWhiteSpace(remarks)) payment.Remarks = remarks;
        if (string.IsNullOrWhiteSpace(payment.ReceiptNumber)) payment.ReceiptNumber = NextReceiptNumber();
        _db.Connection.Update(payment);

        FillDetails(new[] { payment });
        return payment;
    }

    // FR-06: approve a 1 to 3 month extension.
    public void GrantExtension(int paymentId, int months, string remarks = "")
    {
        if (months < 1 || months > 3) throw new ArgumentException("Extensions can be 1, 2 or 3 months.");

        var payment = _db.Connection.Get<Payment>(paymentId);
        payment.ExtensionUntil = payment.DueDate.AddMonths(months);
        if (!string.IsNullOrWhiteSpace(remarks)) payment.Remarks = remarks;
        _db.Connection.Update(payment);
    }

    public void Update(Payment payment)
    {
        if (payment.AmountDue < 0) throw new ArgumentException("The fee cannot be negative.");
        if (payment.AmountPaid < 0) throw new ArgumentException("Amount paid cannot be negative.");
        _db.Connection.Update(payment);
    }

    // ---------- Reading ----------

    public Payment? GetById(int id)
    {
        var p = _db.Connection.Find<Payment>(id);
        if (p != null) FillDetails(new[] { p });
        return p;
    }

    public List<Payment> GetAll(string? month = null)
    {
        var list = _db.Connection.Table<Payment>().ToList();
        if (!string.IsNullOrWhiteSpace(month)) list = list.Where(p => p.Month == month).ToList();

        FillDetails(list);
        return list.OrderByDescending(p => p.Month).ThenBy(p => p.StudentName).ToList();
    }

    // Every fee for one student, newest month first (shown on Student Details).
    public List<Payment> GetForStudent(int studentId)
    {
        var list = _db.Connection.Table<Payment>().Where(p => p.StudentId == studentId).ToList();
        FillDetails(list);
        return list.OrderByDescending(p => p.Month).ThenBy(p => p.ClassName).ToList();
    }

    public decimal OutstandingForStudent(int studentId)
    {
        return GetForStudent(studentId).Sum(p => p.Outstanding);
    }

    // The four cards on the Payment Tracking page, for one month.
    public PaymentSummary GetSummary(string? month = null)
    {
        var rows = GetAll(month ?? CurrentMonth);

        return new PaymentSummary
        {
            TotalExpected = rows.Sum(p => p.AmountDue),
            Collected = rows.Sum(p => Math.Min(p.AmountPaid, p.AmountDue)),
            Pending = rows.Where(p => p.Status is "Pending" or "Extension Granted" or "Partial").Sum(p => p.Outstanding),
            Overdue = rows.Where(p => p.Status == "Overdue").Sum(p => p.Outstanding),
            PaidCount = rows.Count(p => p.Status == "Paid"),
            UnpaidCount = rows.Count(p => p.Status != "Paid"),
        };
    }

    // Students who still owe money, for the office to call (uses the parent phone number).
    public List<Payment> GetOutstanding(string? month = null)
    {
        return GetAll(month).Where(p => p.Outstanding > 0).OrderBy(p => p.StudentName).ToList();
    }

    // ---------- Helpers ----------

    // Fees are due on the 10th of the month.
    public static DateTime DueDateFor(string month)
    {
        return DateTime.TryParse(month + "-01", out var first) ? first.AddDays(9) : DateTime.Today;
    }

    public string NextReceiptNumber()
    {
        int count = _db.Connection.Table<Payment>().Count(p => p.ReceiptNumber != "");
        string candidate;
        do
        {
            count++;
            candidate = $"RCP-{DateTime.Today:yyyyMM}-{count:0000}";
        } while (_db.Connection.Table<Payment>().Any(p => p.ReceiptNumber == candidate));
        return candidate;
    }

    private void FillDetails(IEnumerable<Payment> payments)
    {
        var students = _db.Connection.Table<Student>().ToList().ToDictionary(s => s.Id);
        var classes = _classes.GetAll(activeOnly: false).ToDictionary(c => c.Id);

        foreach (var p in payments)
        {
            if (students.TryGetValue(p.StudentId, out var s))
            {
                p.StudentName = s.FullName;
                p.StudentCode = s.StudentId;
            }
            p.ClassName = classes.TryGetValue(p.ClassGroupId, out var c) ? c.Name : "";
            p.Status = StatusFor(p);
        }
    }
}

public class PaymentSummary
{
    public decimal TotalExpected { get; set; }
    public decimal Collected { get; set; }
    public decimal Pending { get; set; }
    public decimal Overdue { get; set; }
    public int PaidCount { get; set; }
    public int UnpaidCount { get; set; }

    public int CollectedPercent => TotalExpected == 0 ? 0 : (int)Math.Round(100 * Collected / TotalExpected);
}
