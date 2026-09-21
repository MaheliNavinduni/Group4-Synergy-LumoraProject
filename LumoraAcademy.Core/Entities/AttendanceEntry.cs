using SQLite;

namespace LumoraAcademy.Core.Entities;

// One student's attendance on one day in one class.
[Table("Attendance")]
public class AttendanceEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int StudentId { get; set; }                // Students.Id

    [Indexed]
    public DateTime Date { get; set; }

    public string ClassName { get; set; } = "";       // e.g. "Grade 10 - A"
    public int? SubjectId { get; set; }
    public string Status { get; set; } = "Present";   // Present, Absent, Late
    public string Remarks { get; set; } = "";
    public int? MarkedByTeacherId { get; set; }
}

// A summary row for one class on one day (what the Attendance History table shows).
public class AttendanceSummary
{
    public DateTime Date { get; set; }
    public string ClassName { get; set; } = "";
    public string SubjectName { get; set; } = "";
    public int Total { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }

    public string DateText => Date.ToString("MMM dd, yyyy");

    // Excellent / Good / Poor / Perfect - based on the present percentage.
    public string Status
    {
        get
        {
            if (Total == 0) return "";
            double rate = (double)Present / Total;
            if (Present == Total) return "Perfect";
            if (rate >= 0.9) return "Excellent";
            if (rate >= 0.8) return "Good";
            return "Poor";
        }
    }
}
