using SQLite;

namespace LumoraAcademy.Core.Entities;

// Links one student to one class. A student can be in several classes
// (e.g. Maths and ICT) and pays a separate monthly fee for each.
[Table("Enrollments")]
public class Enrollment
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int StudentId { get; set; }                // Students.Id

    [Indexed]
    public int ClassGroupId { get; set; }             // ClassGroups.Id

    public DateTime EnrolledOn { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LeftOn { get; set; }

    // ---- Helpers (not stored) ----

    [Ignore]
    public string StudentName { get; set; } = "";

    [Ignore]
    public string StudentCode { get; set; } = "";

    [Ignore]
    public string ClassName { get; set; } = "";
}
