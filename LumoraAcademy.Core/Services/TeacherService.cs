using LumoraAcademy.Core.Database;
using LumoraAcademy.Core.Entities;

namespace LumoraAcademy.Core.Services;

// Epic 1, User Story 2 - the Administrator manages teacher accounts.
public class TeacherService
{
    private readonly AppDatabase _db;
    private readonly AuthService _auth;

    public TeacherService(AppDatabase db, AuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    // ---------- Read ----------

    public List<Teacher> GetAll()
    {
        return _db.Connection.Table<Teacher>().OrderBy(t => t.FullName).ToList();
    }

    public List<Teacher> GetActive()
    {
        return _db.Connection.Table<Teacher>().Where(t => t.Status != "Resigned").OrderBy(t => t.FullName).ToList();
    }

    public Teacher? GetById(int id)
    {
        return _db.Connection.Find<Teacher>(id);
    }

    public Teacher? GetByTeacherId(string teacherId)
    {
        string key = teacherId.Trim().ToUpper();
        return _db.Connection.Table<Teacher>().FirstOrDefault(t => t.TeacherId == key);
    }

    public List<Teacher> Search(string text)
    {
        string key = (text ?? "").Trim().ToLower();
        if (key == "") return GetAll();

        return _db.Connection.Table<Teacher>().ToList()
            .Where(t => t.FullName.ToLower().Contains(key)
                     || t.TeacherId.ToLower().Contains(key)
                     || t.Email.ToLower().Contains(key)
                     || t.Department.ToLower().Contains(key)
                     || t.Subjects.ToLower().Contains(key))
            .OrderBy(t => t.FullName)
            .ToList();
    }

    public int Count() => _db.Connection.Table<Teacher>().Count(t => t.Status != "Resigned");
    public int CountOnLeave() => _db.Connection.Table<Teacher>().Count(t => t.Status == "On Leave");

    // Percentage of active teachers in each department, e.g. { "STEM": 45, "Humanities": 35 }.
    public Dictionary<string, int> DepartmentPercentages()
    {
        var active = GetActive();
        if (active.Count == 0) return new Dictionary<string, int>();

        return active
            .GroupBy(t => string.IsNullOrWhiteSpace(t.Department) ? "Other" : t.Department)
            .ToDictionary(g => g.Key, g => (int)Math.Round(100.0 * g.Count() / active.Count));
    }

    // ---------- Write ----------

    // Registers a teacher AND creates their login account in one step.
    public Teacher Register(Teacher teacher, string username, string password)
    {
        Validate(teacher);

        if (string.IsNullOrWhiteSpace(teacher.TeacherId)) teacher.TeacherId = NextTeacherId();
        teacher.TeacherId = teacher.TeacherId.Trim().ToUpper();

        if (GetByTeacherId(teacher.TeacherId) != null)
        {
            throw new InvalidOperationException($"Teacher ID '{teacher.TeacherId}' already exists.");
        }
        if (_auth.UsernameExists(username))
        {
            throw new InvalidOperationException($"Username '{username}' is already taken.");
        }

        if (teacher.JoiningDate == default) teacher.JoiningDate = DateTime.Today;
        if (string.IsNullOrWhiteSpace(teacher.Status)) teacher.Status = "Active";

        _db.Connection.Insert(teacher);
        _auth.CreateUser(username, password, "Teacher", teacher.FullName, teacher.Id);
        return teacher;
    }

    public void Update(Teacher teacher)
    {
        Validate(teacher);

        var other = _db.Connection.Table<Teacher>().FirstOrDefault(t => t.TeacherId == teacher.TeacherId && t.Id != teacher.Id);
        if (other != null) throw new InvalidOperationException($"Teacher ID '{teacher.TeacherId}' belongs to another teacher.");

        _db.Connection.Update(teacher);

        // Keep the login's display name in sync.
        var user = _auth.GetByTeacherId(teacher.Id);
        if (user != null)
        {
            user.DisplayName = teacher.FullName;
            _db.Connection.Update(user);
        }
    }

    public void SetPhoto(int teacherId, string photoPath)
    {
        var teacher = _db.Connection.Get<Teacher>(teacherId);
        teacher.PhotoPath = photoPath ?? "";
        _db.Connection.Update(teacher);
    }

    public void SetStatus(int teacherId, string status)
    {
        var teacher = _db.Connection.Get<Teacher>(teacherId);
        teacher.Status = status;
        _db.Connection.Update(teacher);
    }

    // Resignation: marks the teacher as Resigned and blocks their login (deactivate account).
    public void ProcessResignation(int teacherId, DateTime lastWorkingDay, string reason, string notes)
    {
        var teacher = _db.Connection.Get<Teacher>(teacherId);
        teacher.Status = "Resigned";
        teacher.LastWorkingDay = lastWorkingDay;
        teacher.ResignationReason = string.IsNullOrWhiteSpace(notes) ? reason : $"{reason} - {notes}";
        _db.Connection.Update(teacher);

        var user = _auth.GetByTeacherId(teacherId);
        if (user != null)
        {
            _auth.SetActive(user.Id, false);
        }
    }

    // ---------- Helpers ----------

    public string NextTeacherId()
    {
        int count = _db.Connection.Table<Teacher>().Count();
        string candidate;
        do
        {
            count++;
            candidate = "TCH-" + (1000 + count);
        } while (GetByTeacherId(candidate) != null);
        return candidate;
    }

    private static void Validate(Teacher teacher)
    {
        if (string.IsNullOrWhiteSpace(teacher.FullName)) throw new ArgumentException("Full name is required.");
        if (string.IsNullOrWhiteSpace(teacher.Email)) throw new ArgumentException("Email address is required.");
        if (string.IsNullOrWhiteSpace(teacher.Phone)) throw new ArgumentException("Phone number is required.");
    }
}
