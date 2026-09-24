using LumoraAcademy.Core.Database;
using LumoraAcademy.Core.Entities;

namespace LumoraAcademy.Core.Services;

// Classes the institute runs, their weekly time slots, and which students are enrolled.
// Attendance and payments both work off these classes.
public class ClassService
{
    private readonly AppDatabase _db;

    public ClassService(AppDatabase db)
    {
        _db = db;
    }

    // ================= Classes =================

    public List<ClassGroup> GetAll(bool activeOnly = true)
    {
        var classes = _db.Connection.Table<ClassGroup>().ToList();
        if (activeOnly) classes = classes.Where(c => c.IsActive).ToList();

        FillDetails(classes);
        return classes.OrderBy(c => c.Grade).ThenBy(c => c.SubjectName).ToList();
    }

    public ClassGroup? GetById(int id)
    {
        var c = _db.Connection.Find<ClassGroup>(id);
        if (c != null) FillDetails(new[] { c });
        return c;
    }

    public List<ClassGroup> GetForTeacher(int teacherId)
    {
        return GetAll().Where(c => c.TeacherId == teacherId).ToList();
    }

    public ClassGroup Create(ClassGroup group)
    {
        Validate(group);

        bool duplicate = _db.Connection.Table<ClassGroup>()
            .Any(c => c.SubjectId == group.SubjectId && c.Grade == group.Grade && c.TeacherId == group.TeacherId);
        if (duplicate)
        {
            throw new InvalidOperationException("That teacher already has a class for this subject and grade.");
        }

        _db.Connection.Insert(group);
        FillDetails(new[] { group });
        return group;
    }

    public void Update(ClassGroup group)
    {
        Validate(group);
        _db.Connection.Update(group);
    }

    // Closes a class without deleting its history.
    public void Deactivate(int classGroupId)
    {
        var group = _db.Connection.Get<ClassGroup>(classGroupId);
        group.IsActive = false;
        _db.Connection.Update(group);

        foreach (var e in _db.Connection.Table<Enrollment>().Where(e => e.ClassGroupId == classGroupId && e.IsActive).ToList())
        {
            e.IsActive = false;
            e.LeftOn = DateTime.Today;
            _db.Connection.Update(e);
        }
    }

    // ================= Weekly time slots =================

    public List<ClassSession> GetSessions(int classGroupId)
    {
        var sessions = _db.Connection.Table<ClassSession>().Where(s => s.ClassGroupId == classGroupId).ToList();
        FillSessionDetails(sessions);
        return sessions.OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime).ToList();
    }

    public ClassSession AddSession(ClassSession session)
    {
        if (session.ClassGroupId <= 0) throw new ArgumentException("A class is required.");
        if (session.DayOfWeek < 0 || session.DayOfWeek > 6) throw new ArgumentException("Day must be Sunday to Saturday.");
        if (session.EndTime <= session.StartTime) throw new ArgumentException("End time must be after start time.");

        CheckTeacherIsFree(session);

        _db.Connection.Insert(session);
        FillSessionDetails(new[] { session });
        return session;
    }

    public void DeleteSession(int sessionId) => _db.Connection.Delete<ClassSession>(sessionId);

    // A teacher cannot teach two classes at the same time.
    private void CheckTeacherIsFree(ClassSession session)
    {
        var group = _db.Connection.Get<ClassGroup>(session.ClassGroupId);
        var otherGroups = _db.Connection.Table<ClassGroup>().Where(c => c.TeacherId == group.TeacherId).ToList().Select(c => c.Id).ToList();

        var clash = _db.Connection.Table<ClassSession>()
            .Where(s => s.DayOfWeek == session.DayOfWeek && s.Id != session.Id)
            .ToList()
            .Where(s => otherGroups.Contains(s.ClassGroupId))
            .FirstOrDefault(s => session.StartTime < s.EndTime && s.StartTime < session.EndTime);

        if (clash != null)
        {
            throw new InvalidOperationException($"That teacher already has a class on {session.DayName} at {clash.TimeRange}.");
        }
    }

    // ================= Enrolments =================

    // Students in a class (used by attendance and payments).
    public List<Student> GetStudents(int classGroupId)
    {
        var studentIds = _db.Connection.Table<Enrollment>()
            .Where(e => e.ClassGroupId == classGroupId && e.IsActive)
            .ToList()
            .Select(e => e.StudentId)
            .ToList();

        return _db.Connection.Table<Student>().ToList()
            .Where(s => studentIds.Contains(s.Id))
            .OrderBy(s => s.FullName)
            .ToList();
    }

    public int CountStudents(int classGroupId)
    {
        return _db.Connection.Table<Enrollment>().Count(e => e.ClassGroupId == classGroupId && e.IsActive);
    }

    // Classes one student attends.
    public List<ClassGroup> GetClassesForStudent(int studentId)
    {
        var classIds = _db.Connection.Table<Enrollment>()
            .Where(e => e.StudentId == studentId && e.IsActive)
            .ToList()
            .Select(e => e.ClassGroupId)
            .ToList();

        return GetAll(activeOnly: false).Where(c => classIds.Contains(c.Id)).ToList();
    }

    public bool IsEnrolled(int studentId, int classGroupId)
    {
        return _db.Connection.Table<Enrollment>().Any(e => e.StudentId == studentId && e.ClassGroupId == classGroupId && e.IsActive);
    }

    public Enrollment Enroll(int studentId, int classGroupId)
    {
        if (_db.Connection.Find<Student>(studentId) == null) throw new ArgumentException("Student not found.");
        if (_db.Connection.Find<ClassGroup>(classGroupId) == null) throw new ArgumentException("Class not found.");

        // If they were in the class before, switch the old row back on instead of adding another.
        var existing = _db.Connection.Table<Enrollment>().FirstOrDefault(e => e.StudentId == studentId && e.ClassGroupId == classGroupId);
        if (existing != null)
        {
            if (existing.IsActive) throw new InvalidOperationException("That student is already in this class.");

            existing.IsActive = true;
            existing.LeftOn = null;
            existing.EnrolledOn = DateTime.Today;
            _db.Connection.Update(existing);
            return existing;
        }

        var enrollment = new Enrollment { StudentId = studentId, ClassGroupId = classGroupId, EnrolledOn = DateTime.Today, IsActive = true };
        _db.Connection.Insert(enrollment);
        return enrollment;
    }

    // Removes a student from a class but keeps their attendance and payment history.
    public void Unenroll(int studentId, int classGroupId)
    {
        var enrollment = _db.Connection.Table<Enrollment>().FirstOrDefault(e => e.StudentId == studentId && e.ClassGroupId == classGroupId && e.IsActive);
        if (enrollment == null) return;

        enrollment.IsActive = false;
        enrollment.LeftOn = DateTime.Today;
        _db.Connection.Update(enrollment);
    }

    // When a student leaves the institute, take them out of every class.
    public void UnenrollFromAll(int studentId)
    {
        foreach (var e in _db.Connection.Table<Enrollment>().Where(e => e.StudentId == studentId && e.IsActive).ToList())
        {
            e.IsActive = false;
            e.LeftOn = DateTime.Today;
            _db.Connection.Update(e);
        }
    }

    // ================= Helpers =================

    private void FillDetails(IEnumerable<ClassGroup> classes)
    {
        var subjects = _db.Connection.Table<Subject>().ToList().ToDictionary(s => s.Id, s => s.Name);
        var teachers = _db.Connection.Table<Teacher>().ToList().ToDictionary(t => t.Id, t => t.FullName);
        var counts = _db.Connection.Table<Enrollment>().Where(e => e.IsActive).ToList()
            .GroupBy(e => e.ClassGroupId).ToDictionary(g => g.Key, g => g.Count());

        foreach (var c in classes)
        {
            c.SubjectName = subjects.TryGetValue(c.SubjectId, out var s) ? s : "";
            c.TeacherName = teachers.TryGetValue(c.TeacherId, out var t) ? t : "";
            c.StudentCount = counts.TryGetValue(c.Id, out var n) ? n : 0;

            var sessions = _db.Connection.Table<ClassSession>().Where(x => x.ClassGroupId == c.Id).ToList().OrderBy(x => x.DayOfWeek).ToList();
            c.Days = string.Join(", ", sessions.Select(x => x.ShortDayName).Distinct());
            c.TimeRange = sessions.Count > 0 ? sessions[0].TimeRange : "";
        }
    }

    private void FillSessionDetails(IEnumerable<ClassSession> sessions)
    {
        var groups = GetAll(activeOnly: false).ToDictionary(c => c.Id);
        foreach (var s in sessions)
        {
            s.ClassGroup = groups.TryGetValue(s.ClassGroupId, out var g) ? g : null;
        }
    }

    // Every session that runs on a given day, earliest first (the Mark Attendance page).
    public List<ClassSession> GetSessionsForDay(DateTime date)
    {
        int day = (int)date.DayOfWeek;
        var sessions = _db.Connection.Table<ClassSession>().Where(s => s.DayOfWeek == day).ToList();
        FillSessionDetails(sessions);

        return sessions
            .Where(s => s.ClassGroup != null && s.ClassGroup.IsActive)
            .OrderBy(s => s.StartTime)
            .ToList();
    }

    private static void Validate(ClassGroup group)
    {
        if (group.SubjectId <= 0) throw new ArgumentException("A subject is required.");
        if (group.TeacherId <= 0) throw new ArgumentException("A teacher is required.");
        if (string.IsNullOrWhiteSpace(group.Grade)) throw new ArgumentException("A grade is required.");
        if (group.MonthlyFee < 0) throw new ArgumentException("The monthly fee cannot be negative.");
    }
}
