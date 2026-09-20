using SQLite;

namespace LumoraAcademy.Core.Entities;

// One row per teacher (staff member).
[Table("Teachers")]
public class Teacher
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed(Unique = true)]
    public string TeacherId { get; set; } = "";       // e.g. "TCH-2048"

    public string FullName { get; set; } = "";
    public string Designation { get; set; } = "";     // e.g. "Senior Mathematics Faculty"
    public string Department { get; set; } = "";      // e.g. "STEM"
    public string Subjects { get; set; } = "";        // e.g. "Calculus, Algebra"
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = "";
    public string BloodGroup { get; set; } = "";
    public DateTime JoiningDate { get; set; }
    public int YearsOfExperience { get; set; }

    public string Status { get; set; } = "Active";    // Active, On Leave, Resigned
    public DateTime? LastWorkingDay { get; set; }
    public string ResignationReason { get; set; } = "";
    public string PhotoPath { get; set; } = "";

    [Ignore]
    public string Initials => Student.MakeInitials(FullName);
}
