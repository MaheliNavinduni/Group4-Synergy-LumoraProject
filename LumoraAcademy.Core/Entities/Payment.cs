using SQLite;

namespace LumoraAcademy.Core.Entities;

// One month's fee for one student in one class (proposal FR-05, FR-06).
// All payments are cash, taken at the office. This table replaces the paper fee register:
// it records whether the student has paid for that month.
[Table("Payments")]
public class Payment
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int StudentId { get; set; }                // Students.Id

    [Indexed]
    public int ClassGroupId { get; set; }             // ClassGroups.Id

    [Indexed]
    public string Month { get; set; } = "";           // e.g. "2026-09"

    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaymentDate { get; set; }

    // If the admin approves a 1-3 month extension, this holds the new deadline (FR-06).
    public DateTime? ExtensionUntil { get; set; }

    public string Remarks { get; set; } = "";
    public string ReceiptNumber { get; set; } = "";

    // ---- Helpers (not stored) ----

    [Ignore]
    public decimal Outstanding => Math.Max(0, AmountDue - AmountPaid);

    [Ignore]
    public bool IsPaid => AmountPaid >= AmountDue && AmountDue > 0;

    [Ignore]
    public string StudentName { get; set; } = "";

    [Ignore]
    public string StudentCode { get; set; } = "";

    [Ignore]
    public string ClassName { get; set; } = "";

    [Ignore]
    public string Status { get; set; } = "";          // filled by PaymentService

    [Ignore]
    public string PaidDateText => PaymentDate.HasValue ? PaymentDate.Value.ToString("MMM dd, yyyy") : "-";

    [Ignore]
    public string MonthText
    {
        get
        {
            return DateTime.TryParse(Month + "-01", out var d) ? d.ToString("MMMM yyyy") : Month;
        }
    }
}
