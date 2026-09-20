using SQLite;

namespace LumoraAcademy.Core.Entities;

// One mark for one student in one subject (proposal FR-07, FR-08).
[Table("ExamResults")]
public class ExamResult
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int StudentId { get; set; }                // Students.Id

    [Indexed]
    public int SubjectId { get; set; }                // Subjects.Id

    // "Monthly", "Term", "YearEnd" or "Cambridge" (external exam)
    public string ExamType { get; set; } = "";

    // Which exam it was, e.g. "September 2026", "Term 2 2026", "IGCSE May 2026"
    public string ExamName { get; set; } = "";

    public double Marks { get; set; }                 // 0 - 100
    public string Grade { get; set; } = "";           // A+, A, A-, B ... (Cambridge: A*, A, B ...)
    public string Remarks { get; set; } = "";
    public DateTime RecordedOn { get; set; }
    public int? RecordedByTeacherId { get; set; }

    [Ignore]
    public string SubjectName { get; set; } = "";

    // Turns a mark out of 100 into a letter grade.
    public static string GradeFor(double marks)
    {
        if (marks >= 95) return "A+";
        if (marks >= 90) return "A";
        if (marks >= 85) return "A-";
        if (marks >= 80) return "B+";
        if (marks >= 75) return "B";
        if (marks >= 70) return "B-";
        if (marks >= 65) return "C+";
        if (marks >= 60) return "C";
        if (marks >= 50) return "D";
        return "F";
    }
}
