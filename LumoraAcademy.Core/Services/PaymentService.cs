using LumoraAcademy.Core.Database;
using LumoraAcademy.Core.Entities;

namespace LumoraAcademy.Core.Services;

// Epic 3 - Payment Management (FR-05, FR-06).
public class PaymentService
{
    private readonly AppDatabase _db;

    public PaymentService(AppDatabase db)
    {
        _db = db;
    }

    // ---------- Status rules (FR-06) ----------
    // Paid              -> fully paid
    // Dropout           -> student has left with money still owed
    // Extension Granted -> not paid, but an approved extension date is still in the future
    // Overdue           -> not paid and past the due date
    // Pending           -> not paid, due date not reached yet
    public static string StatusFor(Payment p, string studentStatus, DateTime? today = null)
    {
        var now = (today ?? DateTime.Today).Date;

        if (p.AmountPaid >= p.AmountDue) return "Paid";
        if (studentStatus == "Dropout") return "Dropout";
        if (p.ExtensionUntil.HasValue && now <= p.ExtensionUntil.Value.Date) return "Extension Granted";
        if (now > p.DueDate.Date) return "Overdue";
        return "Pending";
    }

    // ---------- Read ----------

    public List<Payment> GetAll()
    {
        var payments = _db.Connection.Table<Payment>().OrderByDescending(p => p.DueDate).ToList();
        FillDetails(payments);
        return payments;
    }

    public List<Payment> GetForStudent(int studentId)
    {
        var payments = _db.Connection.Table<Payment>().Where(p => p.StudentId == studentId).OrderByDescending(p => p.DueDate).ToList();
        FillDetails(payments);
        return payments;
    }

    public Payment? GetById(int id)
    {
        var p = _db.Connection.Find<Payment>(id);
        if (p != null) FillDetails(new[] { p });
        return p;
    }

    public decimal OutstandingForStudent(int studentId)
    {
        return GetForStudent(studentId).Sum(p => p.Outstanding);
    }

    // The four numbers on the Payment Tracking page.
    public PaymentSummary GetSummary()
    {
        var all = GetAll();
        return new PaymentSummary
        {
            TotalExpected = all.Sum(p => p.AmountDue),
            Collected = all.Sum(p => Math.Min(p.AmountPaid, p.AmountDue)),
            Pending = all.Where(p => p.Status == "Pending" || p.Status == "Extension Granted").Sum(p => p.Outstanding),
            Overdue = all.Where(p => p.Status == "Overdue" || p.Status == "Dropout").Sum(p => p.Outstanding),
            PendingCount = all.Count(p => p.Status == "Pending" || p.Status == "Extension Granted"),
        };
    }

    // ---------- Write ----------

    // Creates the monthly fee row for a student (what they are expected to pay).
    public Payment CreateMonthlyFee(int studentId, string month, decimal amountDue, DateTime dueDate)
    {
        if (amountDue <= 0) throw new ArgumentException("Amount due must be greater than zero.");
        if (_db.Connection.Find<Student>(studentId) == null) throw new ArgumentException("Student not found.");

        var payment = new Payment { StudentId = studentId, Month = month, AmountDue = amountDue, AmountPaid = 0, DueDate = dueDate };
        _db.Connection.Insert(payment);
        return payment;
    }

    // FR-05: record money received. Amount and date are mandatory.
    public Payment RecordPayment(int paymentId, decimal amount, DateTime paymentDate, string remarks = "", string receiptPath = "")
    {
        if (amount <= 0) throw new ArgumentException("Payment amount must be greater than zero.");
        if (paymentDate == default) throw new ArgumentException("Payment date is required.");

        var payment = _db.Connection.Get<Payment>(paymentId);
        payment.AmountPaid += amount;
        payment.PaymentDate = paymentDate;
        if (!string.IsNullOrWhiteSpace(remarks)) payment.Remarks = remarks;
        if (!string.IsNullOrWhiteSpace(receiptPath)) payment.ReceiptPath = receiptPath;
        _db.Connection.Update(payment);

        FillDetails(new[] { payment });
        return payment;
    }

    // Edit Payment Record page: replaces the whole row.
    public void Update(Payment payment)
    {
        if (payment.AmountDue <= 0) throw new ArgumentException("Amount due must be greater than zero.");
        if (payment.AmountPaid < 0) throw new ArgumentException("Amount paid cannot be negative.");
        _db.Connection.Update(payment);
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

    public void Delete(int paymentId)
    {
        _db.Connection.Delete<Payment>(paymentId);
    }

    // ---------- Helpers ----------

    private void FillDetails(IEnumerable<Payment> payments)
    {
        var students = _db.Connection.Table<Student>().ToList().ToDictionary(s => s.Id);
        foreach (var p in payments)
        {
            if (students.TryGetValue(p.StudentId, out var s))
            {
                p.StudentName = s.FullName;
                p.StudentCode = s.StudentId;
                p.Status = StatusFor(p, s.Status);
            }
            else
            {
                p.Status = StatusFor(p, "");
            }
        }
    }
}

public class PaymentSummary
{
    public decimal TotalExpected { get; set; }
    public decimal Collected { get; set; }
    public decimal Pending { get; set; }
    public decimal Overdue { get; set; }
    public int PendingCount { get; set; }

    public int CollectedPercent => TotalExpected == 0 ? 0 : (int)Math.Round(100 * Collected / TotalExpected);
}
