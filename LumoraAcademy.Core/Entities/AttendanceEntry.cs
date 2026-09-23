using SQLite;

namespace LumoraAcademy.Core.Entities;

// One student's attendance for one class on one day.
// A student with no row for that day is "not recorded" - that is not the same as Absent.
[Table("Attendance")]
public class AttendanceEntry
{
    public const string Present = "Present";
    public const string Absent = "Absent";
    public const string Late = "Late";

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int StudentId { get; set; }                // Students.Id

    [Indexed]
    public int ClassGroupId { get; set; }             // ClassGroups.Id

    [Indexed]
    public DateTime Date { get; set; }

    public string Status { get; set; } = "";          // Present, Absent or Late
    public string Remarks { get; set; } = "";
    public int? MarkedByTeacherId { get; set; }
    public DateTime MarkedOn { get; set; }
}

// A summary row for one class on one day (what the Attendance History table shows).
public class AttendanceSummary
{
    public DateTime Date { get; set; }
    public int ClassGroupId { get; set; }
    public string ClassName { get; set; } = "";
    public string SubjectName { get; set; } = "";
    public string Grade { get; set; } = "";
    public int Enrolled { get; set; }                 // how many students are in the class
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }

    public int Marked => Present + Absent + Late;
    public int NotRecorded => Math.Max(0, Enrolled - Marked);

    public string DateText => Date.ToString("MMM dd, yyyy");

    // Excellent / Good / Poor / Perfect - based on the students who were marked.
    public string Status
    {
        get
        {
            if (Marked == 0) return "";
            double rate = (double)Present / Marked;
            if (Present == Marked) return "Perfect";
            if (rate >= 0.9) return "Excellent";
            if (rate >= 0.8) return "Good";
            return "Poor";
        }
    }
}
