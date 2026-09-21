using LumoraAcademy.Core.Database;
using LumoraAcademy.Core.Entities;

namespace LumoraAcademy.Core.Services;

// Marks and reports student attendance.
public class AttendanceService
{
    private readonly AppDatabase _db;

    public AttendanceService(AppDatabase db)
    {
        _db = db;
    }

    // ---------- Write ----------

    // Saves one student's status for a day. If already marked that day in that class, it is updated.
    public AttendanceEntry Mark(int studentId, DateTime date, string className, string status, string remarks = "", int? subjectId = null, int? teacherId = null)
    {
        if (status != "Present" && status != "Absent" && status != "Late")
            throw new ArgumentException("Status must be Present, Absent or Late.");

        var day = date.Date;
        var existing = _db.Connection.Table<AttendanceEntry>()
            .FirstOrDefault(a => a.StudentId == studentId && a.Date == day && a.ClassName == className);

        if (existing != null)
        {
            existing.Status = status;
            existing.Remarks = remarks ?? "";
            existing.SubjectId = subjectId;
            existing.MarkedByTeacherId = teacherId;
            _db.Connection.Update(existing);
            return existing;
        }

        var entry = new AttendanceEntry
        {
            StudentId = studentId,
            Date = day,
            ClassName = className,
            SubjectId = subjectId,
            Status = status,
            Remarks = remarks ?? "",
            MarkedByTeacherId = teacherId,
        };
        _db.Connection.Insert(entry);
        return entry;
    }

    // Saves a whole class at once (the Mark Attendance page's Save button).
    public void MarkClass(DateTime date, string className, IEnumerable<(int StudentId, string Status, string Remarks)> rows, int? subjectId = null, int? teacherId = null)
    {
        _db.Connection.RunInTransaction(() =>
        {
            foreach (var row in rows)
            {
                Mark(row.StudentId, date, className, row.Status, row.Remarks, subjectId, teacherId);
            }
        });
    }

    // ---------- Read ----------

    public List<AttendanceEntry> GetForStudent(int studentId)
    {
        return _db.Connection.Table<AttendanceEntry>().Where(a => a.StudentId == studentId).OrderByDescending(a => a.Date).ToList();
    }

    public List<AttendanceEntry> GetForClassOnDay(string className, DateTime date)
    {
        var day = date.Date;
        return _db.Connection.Table<AttendanceEntry>().Where(a => a.ClassName == className && a.Date == day).ToList();
    }

    // Attendance History table: one row per class per day, optionally filtered.
    public List<AttendanceSummary> GetDailySummaries(DateTime? from = null, DateTime? to = null, string? className = null, string? subjectName = null)
    {
        var entries = _db.Connection.Table<AttendanceEntry>().ToList().AsEnumerable();

        if (from.HasValue) entries = entries.Where(a => a.Date >= from.Value.Date);
        if (to.HasValue) entries = entries.Where(a => a.Date <= to.Value.Date);
        if (!string.IsNullOrWhiteSpace(className) && className != "All Classes") entries = entries.Where(a => a.ClassName == className);

        var subjects = _db.Connection.Table<Subject>().ToList().ToDictionary(s => s.Id, s => s.Name);

        var summaries = entries
            .GroupBy(a => new { a.Date, a.ClassName, a.SubjectId })
            .Select(g => new AttendanceSummary
            {
                Date = g.Key.Date,
                ClassName = g.Key.ClassName,
                SubjectName = g.Key.SubjectId.HasValue && subjects.TryGetValue(g.Key.SubjectId.Value, out var n) ? n : "",
                Total = g.Count(),
                Present = g.Count(a => a.Status == "Present"),
                Absent = g.Count(a => a.Status == "Absent"),
                Late = g.Count(a => a.Status == "Late"),
            });

        if (!string.IsNullOrWhiteSpace(subjectName) && subjectName != "All Subjects")
            summaries = summaries.Where(s => s.SubjectName == subjectName);

        return summaries.OrderByDescending(s => s.Date).ThenBy(s => s.ClassName).ToList();
    }

    // Percentage of days the student was present (Late counts as present).
    public double AttendanceRateForStudent(int studentId)
    {
        var entries = GetForStudent(studentId);
        if (entries.Count == 0) return 0;
        return Math.Round(100.0 * entries.Count(a => a.Status != "Absent") / entries.Count, 1);
    }

    public int AbsencesForStudent(int studentId)
    {
        return _db.Connection.Table<AttendanceEntry>().Count(a => a.StudentId == studentId && a.Status == "Absent");
    }

    // The three cards on the Admin Attendance History page.
    public AttendanceStats GetStats()
    {
        var all = _db.Connection.Table<AttendanceEntry>().ToList();
        var days = all.Select(a => a.Date).Distinct().ToList();
        var summaries = GetDailySummaries();

        return new AttendanceStats
        {
            AverageRate = all.Count == 0 ? 0 : Math.Round(100.0 * all.Count(a => a.Status != "Absent") / all.Count, 1),
            TotalDaysLogged = days.Count,
            PerfectDays = summaries.Count(s => s.Status == "Perfect"),
        };
    }

    public List<string> GetClassNames()
    {
        return _db.Connection.Table<AttendanceEntry>().ToList().Select(a => a.ClassName).Distinct().OrderBy(c => c).ToList();
    }
}

public class AttendanceStats
{
    public double AverageRate { get; set; }
    public int TotalDaysLogged { get; set; }
    public int PerfectDays { get; set; }
}
