using LumoraAcademy.Core.Database;
using LumoraAcademy.Core.Entities;

namespace LumoraAcademy.Core.Services;

// Epic 4 - Academic Performance Management (FR-07, FR-08, FR-09) plus subject management.
public class AcademicService
{
    public const string ExamMonthly = "Monthly";
    public const string ExamTerm = "Term";
    public const string ExamYearEnd = "YearEnd";
    public const string ExamCambridge = "Cambridge";

    private readonly AppDatabase _db;

    public AcademicService(AppDatabase db)
    {
        _db = db;
    }

    // ================= Subjects =================

    public List<Subject> GetSubjects()
    {
        return _db.Connection.Table<Subject>().OrderBy(s => s.Name).ThenBy(s => s.Code).ToList();
    }

    public Subject? GetSubject(int id) => _db.Connection.Find<Subject>(id);

    public Subject AddSubject(string name, string code, string curriculum)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Subject name is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Subject code is required.");

        string key = code.Trim().ToUpper();
        if (_db.Connection.Table<Subject>().Any(s => s.Code == key))
        {
            throw new InvalidOperationException($"Subject code '{key}' already exists.");
        }

        var subject = new Subject { Name = name.Trim(), Code = key, Curriculum = (curriculum ?? "").Trim() };
        _db.Connection.Insert(subject);
        return subject;
    }

    public void UpdateSubject(Subject subject) => _db.Connection.Update(subject);

    public void DeleteSubject(int id) => _db.Connection.Delete<Subject>(id);

    // ================= Exam marks (FR-07) =================

    // Records a monthly, term or year-end mark. Marks must be 0-100.
    public ExamResult RecordMark(int studentId, int subjectId, string examType, string examName, double marks, string remarks = "", int? teacherId = null)
    {
        if (examType != ExamMonthly && examType != ExamTerm && examType != ExamYearEnd)
            throw new ArgumentException("Exam type must be Monthly, Term or YearEnd.");
        if (marks < 0 || marks > 100) throw new ArgumentException("Marks must be between 0 and 100.");
        if (_db.Connection.Find<Student>(studentId) == null) throw new ArgumentException("Student not found.");
        if (_db.Connection.Find<Subject>(subjectId) == null) throw new ArgumentException("Subject not found.");

        var result = new ExamResult
        {
            StudentId = studentId,
            SubjectId = subjectId,
            ExamType = examType,
            ExamName = examName ?? "",
            Marks = marks,
            Grade = ExamResult.GradeFor(marks),
            Remarks = remarks ?? "",
            RecordedOn = DateTime.Now,
            RecordedByTeacherId = teacherId,
        };
        _db.Connection.Insert(result);
        return result;
    }

    // FR-08: records an external (Cambridge) result. Grade is given by the exam board, e.g. "A*".
    public ExamResult RecordCambridgeResult(int studentId, int subjectId, string examName, string grade, double? marks = null, int? teacherId = null)
    {
        if (string.IsNullOrWhiteSpace(examName)) throw new ArgumentException("Exam name is required, e.g. IGCSE May 2026.");
        if (string.IsNullOrWhiteSpace(grade)) throw new ArgumentException("Grade is required.");
        if (_db.Connection.Find<Student>(studentId) == null) throw new ArgumentException("Student not found.");

        var result = new ExamResult
        {
            StudentId = studentId,
            SubjectId = subjectId,
            ExamType = ExamCambridge,
            ExamName = examName,
            Marks = marks ?? 0,
            Grade = grade.Trim().ToUpper(),
            RecordedOn = DateTime.Now,
            RecordedByTeacherId = teacherId,
        };
        _db.Connection.Insert(result);
        return result;
    }

    public void UpdateResult(ExamResult result)
    {
        if (result.ExamType != ExamCambridge) result.Grade = ExamResult.GradeFor(result.Marks);
        _db.Connection.Update(result);
    }

    public void DeleteResult(int id) => _db.Connection.Delete<ExamResult>(id);

    public List<ExamResult> GetResultsForStudent(int studentId)
    {
        var results = _db.Connection.Table<ExamResult>().Where(r => r.StudentId == studentId).OrderByDescending(r => r.RecordedOn).ToList();
        FillSubjectNames(results);
        return results;
    }

    public List<ExamResult> GetResultsForSubject(int subjectId)
    {
        var results = _db.Connection.Table<ExamResult>().Where(r => r.SubjectId == subjectId).ToList();
        FillSubjectNames(results);
        return results;
    }

    // Average of a student's internal marks (Cambridge results are graded separately).
    public double AverageForStudent(int studentId)
    {
        var marks = _db.Connection.Table<ExamResult>()
            .Where(r => r.StudentId == studentId && r.ExamType != ExamCambridge)
            .ToList();
        return marks.Count == 0 ? 0 : Math.Round(marks.Average(r => r.Marks), 1);
    }

    // ================= Weak students (FR-09) =================

    // Students whose average internal mark is below the threshold (default 60%).
    // Each entry lists the subjects where they are below the threshold.
    public List<AtRiskStudent> GetAtRiskStudents(double threshold = 60, string? grade = null, string? subjectName = null)
    {
        var students = _db.Connection.Table<Student>().Where(s => s.Status == "Active").ToList();
        var subjects = _db.Connection.Table<Subject>().ToList().ToDictionary(s => s.Id);
        var allResults = _db.Connection.Table<ExamResult>().Where(r => r.ExamType != ExamCambridge).ToList();

        var list = new List<AtRiskStudent>();

        foreach (var s in students)
        {
            if (!string.IsNullOrWhiteSpace(grade) && grade != "All Classes" && !s.Grade.StartsWith(grade)) continue;

            var results = allResults.Where(r => r.StudentId == s.Id).ToList();
            if (results.Count == 0) continue;

            double average = results.Average(r => r.Marks);
            if (average >= threshold) continue;

            var weakSubjects = results
                .GroupBy(r => r.SubjectId)
                .Select(g => new { Name = subjects.TryGetValue(g.Key, out var sub) ? sub.Name : "?", Avg = g.Average(r => r.Marks) })
                .Where(x => x.Avg < threshold)
                .Select(x => x.Name)
                .Distinct()
                .ToList();

            if (!string.IsNullOrWhiteSpace(subjectName) && subjectName != "All Subjects" && !weakSubjects.Contains(subjectName)) continue;

            // Falling = latest mark is lower than the earlier one
            var ordered = results.OrderBy(r => r.RecordedOn).ToList();
            bool falling = ordered.Count >= 2 && ordered[^1].Marks < ordered[0].Marks;

            list.Add(new AtRiskStudent
            {
                Student = s,
                Average = Math.Round(average, 1),
                WeakSubjects = weakSubjects,
                IsFalling = falling,
            });
        }

        return list.OrderBy(a => a.Average).ToList();
    }

    // ================= Progress notes (FR-09) =================

    public ProgressNote AddProgressNote(int studentId, string note, bool recommendExtraClass, int? teacherId = null)
    {
        if (string.IsNullOrWhiteSpace(note)) throw new ArgumentException("Note text is required.");

        var pn = new ProgressNote { StudentId = studentId, TeacherId = teacherId, Note = note.Trim(), RecommendExtraClass = recommendExtraClass, CreatedOn = DateTime.Now };
        _db.Connection.Insert(pn);
        return pn;
    }

    public List<ProgressNote> GetProgressNotes(int studentId)
    {
        return _db.Connection.Table<ProgressNote>().Where(n => n.StudentId == studentId).OrderByDescending(n => n.CreatedOn).ToList();
    }

    public void DeleteProgressNote(int id) => _db.Connection.Delete<ProgressNote>(id);

    // ================= Helpers =================

    private void FillSubjectNames(IEnumerable<ExamResult> results)
    {
        var subjects = _db.Connection.Table<Subject>().ToList().ToDictionary(s => s.Id, s => s.Name);
        foreach (var r in results)
        {
            r.SubjectName = subjects.TryGetValue(r.SubjectId, out var n) ? n : "";
        }
    }
}

// One row on the At-Risk Students page.
public class AtRiskStudent
{
    public Student Student { get; set; } = new();
    public double Average { get; set; }
    public List<string> WeakSubjects { get; set; } = new();
    public bool IsFalling { get; set; }

    public string AverageText => $"{Average:0}%";
    public string Subject1 => WeakSubjects.Count > 0 ? WeakSubjects[0] : "";
    public string Subject2 => WeakSubjects.Count > 1 ? WeakSubjects[1] : "";
    public string Cohort => $"{Student.Grade} - {Student.Section}";
}
