using LumoraAcademy.Core.Database;
using LumoraAcademy.Core.Entities;

namespace LumoraAcademy.Core.Services;

// Attendance is marked in the office, per class, as students arrive.
// Nothing is assumed: a student with no row for that day is "not recorded", not absent.
public class AttendanceService
{
    private readonly AppDatabase _db;
    private readonly ClassService _classes;

    public AttendanceService(AppDatabase db, ClassService classes)
    {
        _db = db;
        _classes = classes;
    }

    // ---------- The Mark Attendance screen ----------

    // One row per enrolled student, with the status already saved for that day (blank if not marked).
    public List<AttendanceRow> GetSheet(int classGroupId, DateTime date)
    {
        var day = date.Date;
        var students = _classes.GetStudents(classGroupId);
        var saved = _db.Connection.Table<AttendanceEntry>()
            .Where(a => a.ClassGroupId == classGroupId && a.Date == day)
            .ToList()
            .ToDictionary(a => a.StudentId);

        return students.Select(s => new AttendanceRow
        {
            StudentId = s.Id,
            StudentCode = s.StudentId,
            StudentName = s.FullName,
            Initials = s.Initials,
            PhotoPath = s.PhotoPath,
            Status = saved.TryGetValue(s.Id, out var entry) ? entry.Status : "",
            Remarks = saved.TryGetValue(s.Id, out var e2) ? e2.Remarks : "",
        }).ToList();
    }

    // Saves the sheet. Rows left blank are not saved, and any earlier mark for them is removed.
    public void SaveSheet(int classGroupId, DateTime date, IEnumerable<AttendanceRow> rows, int? teacherId = null)
    {
        var day = date.Date;

        _db.Connection.RunInTransaction(() =>
        {
            foreach (var row in rows)
            {
                var existing = _db.Connection.Table<AttendanceEntry>()
                    .FirstOrDefault(a => a.ClassGroupId == classGroupId && a.Date == day && a.StudentId == row.StudentId);

                if (string.IsNullOrWhiteSpace(row.Status))
                {
                    // Left blank - remove any mark that was there before.
                    if (existing != null) _db.Connection.Delete(existing);
                    continue;
                }

                ValidateStatus(row.Status);

                if (existing == null)
                {
                    _db.Connection.Insert(new AttendanceEntry
                    {
                        StudentId = row.StudentId,
                        ClassGroupId = classGroupId,
                        Date = day,
                        Status = row.Status,
                        Remarks = row.Remarks ?? "",
                        MarkedByTeacherId = teacherId,
                        MarkedOn = DateTime.Now,
                    });
                }
                else
                {
                    existing.Status = row.Status;
                    existing.Remarks = row.Remarks ?? "";
                    existing.MarkedByTeacherId = teacherId;
                    existing.MarkedOn = DateTime.Now;
                    _db.Connection.Update(existing);
                }
            }
        });
    }

    // Marks one student straight away (used when a student checks in at the office).
    public void Mark(int studentId, int classGroupId, DateTime date, string status, string remarks = "", int? teacherId = null)
    {
        ValidateStatus(status);

        var row = new AttendanceRow { StudentId = studentId, Status = status, Remarks = remarks };
        SaveSheet(classGroupId, date, new[] { row }, teacherId);
    }

    // True when at least one student has been marked for that class that day.
    public bool IsMarked(int classGroupId, DateTime date)
    {
        var day = date.Date;
        return _db.Connection.Table<AttendanceEntry>().Any(a => a.ClassGroupId == classGroupId && a.Date == day);
    }

    // ---------- Reading ----------

    public List<AttendanceEntry> GetForStudent(int studentId)
    {
        return _db.Connection.Table<AttendanceEntry>().Where(a => a.StudentId == studentId).OrderByDescending(a => a.Date).ToList();
    }

    public List<AttendanceEntry> GetForClassOnDay(int classGroupId, DateTime date)
    {
        var day = date.Date;
        return _db.Connection.Table<AttendanceEntry>().Where(a => a.ClassGroupId == classGroupId && a.Date == day).ToList();
    }

    // Attendance History table: one row per class per day.
    public List<AttendanceSummary> GetDailySummaries(DateTime? from = null, DateTime? to = null, int? classGroupId = null)
    {
        var entries = _db.Connection.Table<AttendanceEntry>().ToList().AsEnumerable();

        if (from.HasValue) entries = entries.Where(a => a.Date >= from.Value.Date);
        if (to.HasValue) entries = entries.Where(a => a.Date <= to.Value.Date);
        if (classGroupId.HasValue) entries = entries.Where(a => a.ClassGroupId == classGroupId.Value);

        var classes = _classes.GetAll(activeOnly: false).ToDictionary(c => c.Id);

        return entries
            .GroupBy(a => new { a.Date, a.ClassGroupId })
            .Select(g =>
            {
                classes.TryGetValue(g.Key.ClassGroupId, out var cls);
                return new AttendanceSummary
                {
                    Date = g.Key.Date,
                    ClassGroupId = g.Key.ClassGroupId,
                    ClassName = cls?.Name ?? "",
                    SubjectName = cls?.SubjectName ?? "",
                    Grade = cls?.Grade ?? "",
                    Enrolled = cls?.StudentCount ?? g.Count(),
                    Present = g.Count(a => a.Status == AttendanceEntry.Present),
                    Absent = g.Count(a => a.Status == AttendanceEntry.Absent),
                    Late = g.Count(a => a.Status == AttendanceEntry.Late),
                };
            })
            .OrderByDescending(s => s.Date)
            .ThenBy(s => s.ClassName)
            .ToList();
    }

    // Percentage of marked days the student was present (Late counts as present).
    public double AttendanceRateForStudent(int studentId, int? classGroupId = null)
    {
        var entries = GetForStudent(studentId);
        if (classGroupId.HasValue) entries = entries.Where(a => a.ClassGroupId == classGroupId.Value).ToList();

        if (entries.Count == 0) return 0;
        return Math.Round(100.0 * entries.Count(a => a.Status != AttendanceEntry.Absent) / entries.Count, 1);
    }

    public int AbsencesForStudent(int studentId)
    {
        return _db.Connection.Table<AttendanceEntry>().Count(a => a.StudentId == studentId && a.Status == AttendanceEntry.Absent);
    }

    // The three cards on the Admin Attendance History page.
    public AttendanceStats GetStats()
    {
        var all = _db.Connection.Table<AttendanceEntry>().ToList();
        var summaries = GetDailySummaries();

        return new AttendanceStats
        {
            AverageRate = all.Count == 0 ? 0 : Math.Round(100.0 * all.Count(a => a.Status != AttendanceEntry.Absent) / all.Count, 1),
            TotalDaysLogged = all.Select(a => a.Date).Distinct().Count(),
            PerfectDays = summaries.Count(s => s.Status == "Perfect"),
        };
    }

    private static void ValidateStatus(string status)
    {
        if (status != AttendanceEntry.Present && status != AttendanceEntry.Absent && status != AttendanceEntry.Late)
        {
            throw new ArgumentException("Status must be Present, Absent or Late.");
        }
    }
}

// One line on the Mark Attendance sheet.
public class AttendanceRow
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = "";
    public string StudentName { get; set; } = "";
    public string Initials { get; set; } = "";
    public string PhotoPath { get; set; } = "";
    public string Status { get; set; } = "";          // blank = not marked yet
    public string Remarks { get; set; } = "";

    public bool IsMarked => !string.IsNullOrWhiteSpace(Status);
}

public class AttendanceStats
{
    public double AverageRate { get; set; }
    public int TotalDaysLogged { get; set; }
    public int PerfectDays { get; set; }
}
