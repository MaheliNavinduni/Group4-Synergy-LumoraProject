using LumoraAcademy.Core.Database;
using LumoraAcademy.Core.Entities;

namespace LumoraAcademy.Core.Services;

// The weekly timetable: which teacher teaches which class, when and where.
public class ScheduleService
{
    private readonly AppDatabase _db;

    public ScheduleService(AppDatabase db)
    {
        _db = db;
    }

    // ---------- Read ----------

    public List<ClassSession> GetAll()
    {
        var all = _db.Connection.Table<ClassSession>().ToList();
        FillNames(all);
        return all.OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime).ToList();
    }

    public ClassSession? GetById(int id)
    {
        var s = _db.Connection.Find<ClassSession>(id);
        if (s != null) FillNames(new[] { s });
        return s;
    }

    // All lessons for one teacher, Monday to Sunday.
    public List<ClassSession> GetWeekForTeacher(int teacherId)
    {
        return GetAll().Where(s => s.TeacherId == teacherId).ToList();
    }

    // Lessons a teacher has on one day (default: today).
    public List<ClassSession> GetDayForTeacher(int teacherId, DayOfWeek? day = null)
    {
        int d = (int)(day ?? DateTime.Today.DayOfWeek);
        return GetWeekForTeacher(teacherId).Where(s => s.DayOfWeek == d).ToList();
    }

    // Lessons a class (grade) has on one day - used on the Student Details page.
    public List<ClassSession> GetDayForClass(string className, DayOfWeek? day = null)
    {
        int d = (int)(day ?? DateTime.Today.DayOfWeek);
        return GetAll().Where(s => s.ClassName == className && s.DayOfWeek == d).ToList();
    }

    // Summary per subject for the "My Subjects" cards: which days, what time, how many students.
    public List<SubjectSummary> GetSubjectSummariesForTeacher(int teacherId)
    {
        var week = GetWeekForTeacher(teacherId);
        var students = _db.Connection.Table<Student>().Where(s => s.Status == "Active").ToList();

        return week
            .GroupBy(s => new { s.SubjectId, s.SubjectName, s.ClassName })
            .Select(g =>
            {
                var first = g.OrderBy(s => s.DayOfWeek).First();
                return new SubjectSummary
                {
                    SubjectName = g.Key.SubjectName,
                    ClassName = g.Key.ClassName,
                    Room = first.Room,
                    Days = string.Join(", ", g.OrderBy(s => s.DayOfWeek).Select(s => s.ShortDayName).Distinct()),
                    TimeRange = first.TimeRange,
                    StudentCount = students.Count(s => s.Grade == g.Key.ClassName),
                };
            })
            .OrderBy(x => x.SubjectName)
            .ToList();
    }

    // ---------- Write ----------

    public ClassSession Add(ClassSession session)
    {
        Validate(session);
        CheckClash(session);
        _db.Connection.Insert(session);
        FillNames(new[] { session });
        return session;
    }

    public void Update(ClassSession session)
    {
        Validate(session);
        CheckClash(session);
        _db.Connection.Update(session);
    }

    public void Delete(int id) => _db.Connection.Delete<ClassSession>(id);

    // ---------- Helpers ----------

    private static void Validate(ClassSession s)
    {
        if (s.TeacherId <= 0) throw new ArgumentException("A teacher is required.");
        if (string.IsNullOrWhiteSpace(s.ClassName)) throw new ArgumentException("A class is required.");
        if (s.DayOfWeek < 0 || s.DayOfWeek > 6) throw new ArgumentException("Day must be Sunday to Saturday.");
        if (s.EndTime <= s.StartTime) throw new ArgumentException("End time must be after start time.");
    }

    // A teacher cannot be in two lessons at the same time.
    private void CheckClash(ClassSession s)
    {
        var clash = _db.Connection.Table<ClassSession>()
            .Where(x => x.TeacherId == s.TeacherId && x.DayOfWeek == s.DayOfWeek && x.Id != s.Id)
            .ToList()
            .FirstOrDefault(x => s.StartTime < x.EndTime && x.StartTime < s.EndTime);

        if (clash != null)
        {
            throw new InvalidOperationException($"This teacher already has a lesson on {s.DayName} at {clash.TimeRange}.");
        }
    }

    private void FillNames(IEnumerable<ClassSession> sessions)
    {
        var subjects = _db.Connection.Table<Subject>().ToList().ToDictionary(x => x.Id, x => x.Name);
        var teachers = _db.Connection.Table<Teacher>().ToList().ToDictionary(x => x.Id, x => x.FullName);
        foreach (var s in sessions)
        {
            s.SubjectName = s.SubjectId.HasValue && subjects.TryGetValue(s.SubjectId.Value, out var n) ? n : "";
            s.TeacherName = teachers.TryGetValue(s.TeacherId, out var t) ? t : "";
        }
    }
}

// One "My Subjects" card.
public class SubjectSummary
{
    public string SubjectName { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string Room { get; set; } = "";
    public string Days { get; set; } = "";
    public string TimeRange { get; set; } = "";
    public int StudentCount { get; set; }

    public string Code => string.IsNullOrWhiteSpace(Room) ? ClassName : $"{ClassName} • Room {Room}";
    public string Students => $"{StudentCount} Students";
}
