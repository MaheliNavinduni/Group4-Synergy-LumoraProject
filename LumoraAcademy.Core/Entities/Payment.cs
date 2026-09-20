using SQLite;

namespace LumoraAcademy.Core.Entities;

// One monthly fee record for one student (proposal FR-05, FR-06).
[Table("Payments")]
public class Payment
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int StudentId { get; set; }                // Students.Id

    public string Month { get; set; } = "";           // e.g. "2026-09"
    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaymentDate { get; set; }

    // If the admin approves a 1-3 month extension, this holds the new deadline.
    public DateTime? ExtensionUntil { get; set; }

    public string Remarks { get; set; } = "";
    public string ReceiptPath { get; set; } = "";

    // ---- Helpers (not stored) ----

    [Ignore]
    public decimal Outstanding => Math.Max(0, AmountDue - AmountPaid);

    [Ignore]
    public string StudentName { get; set; } = "";

    [Ignore]
    public string StudentCode { get; set; } = "";

    [Ignore]
    public string Status { get; set; } = "";          // filled by PaymentService
}
