using SQLite;

namespace LumoraAcademy.Core.Entities;

// One row per enrolled student (proposal FR-03).
[Table("Students")]
public class Student
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Human-readable ID shown on screen, e.g. "STU-2024-0001". Must be unique.
    [Indexed(Unique = true)]
    public string StudentId { get; set; } = "";

    public string FullName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = "";
    public string BloodGroup { get; set; } = "";
    public string School { get; set; } = "";
    public string Grade { get; set; } = "";           // e.g. "10th Grade"
    public string Section { get; set; } = "";         // e.g. "B"
    public string PreviousSchool { get; set; } = "";
    public string Email { get; set; } = "";

    public string GuardianName { get; set; } = "";
    public string GuardianPhone { get; set; } = "";
    public string GuardianEmail { get; set; } = "";
    public string Address { get; set; } = "";

    public DateTime JoiningDate { get; set; }
    public string ClassDayTime { get; set; } = "";   // e.g. "Mon, Wed 4:00 PM"
    public int? AssignedTeacherId { get; set; }

    public string Status { get; set; } = "Active";    // Active, Pending, Inactive, Dropout
    public string Notes { get; set; } = "";
    public string PhotoPath { get; set; } = "";

    public DateTime? DepartureDate { get; set; }
    public string DepartureReason { get; set; } = "";

    // ---- Helpers for the screens (not stored in the database) ----

    [Ignore]
    public string Initials => MakeInitials(FullName);

    [Ignore]
    public string AssignedTeacherName { get; set; } = "";

    [Ignore]
    public string AssignedTeacherInitials => MakeInitials(AssignedTeacherName);

    public static string MakeInitials(string name)
    {
        var parts = name.Replace(",", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "";
        if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpper();
        return (parts[0].Substring(0, 1) + parts[^1].Substring(0, 1)).ToUpper();
    }
}
