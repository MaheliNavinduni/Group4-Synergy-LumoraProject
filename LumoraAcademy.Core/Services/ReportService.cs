using LumoraAcademy.Core.Database;
using LumoraAcademy.Core.Entities;

namespace LumoraAcademy.Core.Services;

// Epic 5 - Reporting & Analytics (FR-11): student, payment and academic reports.
public class ReportService
{
    private readonly AppDatabase _db;
    private readonly AcademicService _academic;
    private readonly AttendanceService _attendance;
    private readonly PaymentService _payments;

    public ReportService(AppDatabase db, AcademicService academic, AttendanceService attendance, PaymentService payments)
    {
        _db = db;
        _academic = academic;
        _attendance = attendance;
        _payments = payments;
    }

    // The Academic Progress Report for one student (the printable page).
    public StudentReport BuildStudentReport(int studentId)
    {
        var student = _db.Connection.Get<Student>(studentId);
        var results = _academic.GetResultsForStudent(studentId);

        var rows = results
            .Where(r => r.ExamType != AcademicService.ExamCambridge)
            .GroupBy(r => r.SubjectId)
            .Select(g =>
            {
                double monthly = g.Where(r => r.ExamType == AcademicService.ExamMonthly).Select(r => r.Marks).DefaultIfEmpty(0).Average();
                double term = g.Where(r => r.ExamType == AcademicService.ExamTerm).Select(r => r.Marks).DefaultIfEmpty(0).Average();
                double yearEnd = g.Where(r => r.ExamType == AcademicService.ExamYearEnd).Select(r => r.Marks).DefaultIfEmpty(0).Average();

                // Final score = average of the exams that actually exist.
                var present = new List<double>();
                if (g.Any(r => r.ExamType == AcademicService.ExamMonthly)) present.Add(monthly);
                if (g.Any(r => r.ExamType == AcademicService.ExamTerm)) present.Add(term);
                if (g.Any(r => r.ExamType == AcademicService.ExamYearEnd)) present.Add(yearEnd);
                double final = present.Count == 0 ? 0 : Math.Round(present.Average());

                return new ReportSubjectRow
                {
                    Subject = g.First().SubjectName,
                    MonthlyTest = (int)Math.Round(monthly),
                    TermExam = (int)Math.Round(term),
                    YearEndExam = (int)Math.Round(yearEnd),
                    FinalScore = (int)final,
                    Grade = ExamResult.GradeFor(final),
                    Remarks = g.OrderByDescending(r => r.RecordedOn).Select(r => r.Remarks).FirstOrDefault(r => !string.IsNullOrWhiteSpace(r)) ?? "",
                };
            })
            .OrderBy(r => r.Subject)
            .ToList();

        var cambridge = results.Where(r => r.ExamType == AcademicService.ExamCambridge).ToList();

        var attendance = _attendance.GetForStudent(studentId);

        return new StudentReport
        {
            Student = student,
            Rows = rows,
            CambridgeResults = cambridge,
            TotalPercent = rows.Count == 0 ? 0 : Math.Round(rows.Average(r => r.FinalScore), 1),
            Gpa = rows.Count == 0 ? 0 : Math.Round(rows.Average(r => GpaFor(r.FinalScore)), 2),
            DaysPresent = attendance.Count(a => a.Status != "Absent"),
            DaysTotal = attendance.Count,
        };
    }

    // Payment report: every student with their outstanding balance and latest status.
    public List<PaymentReportRow> BuildPaymentReport(string? statusFilter = null)
    {
        var students = _db.Connection.Table<Student>().OrderBy(s => s.FullName).ToList();
        var rows = new List<PaymentReportRow>();

        foreach (var s in students)
        {
            var payments = _payments.GetForStudent(s.Id);
            if (payments.Count == 0) continue;

            var latest = payments.First();
            var row = new PaymentReportRow
            {
                StudentId = s.StudentId,
                StudentName = s.FullName,
                Grade = s.Grade,
                MonthlyFee = latest.AmountDue,
                Outstanding = payments.Sum(p => p.Outstanding),
                LastPaidDate = payments.Where(p => p.PaymentDate.HasValue).Select(p => p.PaymentDate).Max(),
                Status = latest.Status,
            };

            if (string.IsNullOrWhiteSpace(statusFilter) || statusFilter == "All" || row.Status == statusFilter)
            {
                rows.Add(row);
            }
        }

        return rows;
    }

    // Academic report: average mark per student, optionally for one grade.
    public List<AcademicReportRow> BuildAcademicReport(string? grade = null)
    {
        var students = _db.Connection.Table<Student>().Where(s => s.Status == "Active").OrderBy(s => s.FullName).ToList();
        var rows = new List<AcademicReportRow>();

        foreach (var s in students)
        {
            if (!string.IsNullOrWhiteSpace(grade) && grade != "All Grades" && s.Grade != grade) continue;

            double avg = _academic.AverageForStudent(s.Id);
            rows.Add(new AcademicReportRow
            {
                StudentId = s.StudentId,
                StudentName = s.FullName,
                Grade = s.Grade,
                Average = avg,
                LetterGrade = avg == 0 ? "-" : ExamResult.GradeFor(avg),
                AttendanceRate = _attendance.AttendanceRateForStudent(s.Id),
            });
        }

        return rows;
    }

    // Average mark per subject across all students (Admin dashboard chart).
    public Dictionary<string, double> AverageBySubject()
    {
        var subjects = _db.Connection.Table<Subject>().ToList();
        var results = _db.Connection.Table<ExamResult>().Where(r => r.ExamType != AcademicService.ExamCambridge).ToList();

        return subjects
            .Select(sub => new { sub.Name, Marks = results.Where(r => r.SubjectId == sub.Id).Select(r => r.Marks).ToList() })
            .Where(x => x.Marks.Count > 0)
            .GroupBy(x => x.Name)
            .ToDictionary(g => g.Key, g => Math.Round(g.SelectMany(x => x.Marks).Average(), 1));
    }

    // Converts a percentage into a 4.0-scale grade point.
    public static double GpaFor(double percent)
    {
        if (percent >= 90) return 4.0;
        if (percent >= 85) return 3.7;
        if (percent >= 80) return 3.3;
        if (percent >= 75) return 3.0;
        if (percent >= 70) return 2.7;
        if (percent >= 65) return 2.3;
        if (percent >= 60) return 2.0;
        if (percent >= 50) return 1.0;
        return 0.0;
    }
}

public class StudentReport
{
    public Student Student { get; set; } = new();
    public List<ReportSubjectRow> Rows { get; set; } = new();
    public List<ExamResult> CambridgeResults { get; set; } = new();
    public double TotalPercent { get; set; }
    public double Gpa { get; set; }
    public int DaysPresent { get; set; }
    public int DaysTotal { get; set; }
}

public class ReportSubjectRow
{
    public string Subject { get; set; } = "";
    public int MonthlyTest { get; set; }
    public int TermExam { get; set; }
    public int YearEndExam { get; set; }
    public int FinalScore { get; set; }
    public string Grade { get; set; } = "";
    public string Remarks { get; set; } = "";
}

public class PaymentReportRow
{
    public string StudentId { get; set; } = "";
    public string StudentName { get; set; } = "";
    public string Grade { get; set; } = "";
    public decimal MonthlyFee { get; set; }
    public decimal Outstanding { get; set; }
    public DateTime? LastPaidDate { get; set; }
    public string Status { get; set; } = "";
}

public class AcademicReportRow
{
    public string StudentId { get; set; } = "";
    public string StudentName { get; set; } = "";
    public string Grade { get; set; } = "";
    public double Average { get; set; }
    public string LetterGrade { get; set; } = "";
    public double AttendanceRate { get; set; }
}
