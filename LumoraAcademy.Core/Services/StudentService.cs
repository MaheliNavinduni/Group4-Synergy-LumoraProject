using LumoraAcademy.Core.Database;
using LumoraAcademy.Core.Entities;

namespace LumoraAcademy.Core.Services;

// Epic 2 - Student Management: register, update, search, departure.
public class StudentService
{
    private readonly AppDatabase _db;

    public StudentService(AppDatabase db)
    {
        _db = db;
    }

    // ---------- Read ----------

    public List<Student> GetAll()
    {
        var students = _db.Connection.Table<Student>().OrderBy(s => s.FullName).ToList();
        FillTeacherNames(students);
        return students;
    }

    public Student? GetById(int id)
    {
        var student = _db.Connection.Find<Student>(id);
        if (student != null) FillTeacherNames(new[] { student });
        return student;
    }

    public Student? GetByStudentId(string studentId)
    {
        string key = studentId.Trim().ToUpper();
        var student = _db.Connection.Table<Student>().FirstOrDefault(s => s.StudentId == key);
        if (student != null) FillTeacherNames(new[] { student });
        return student;
    }

    public int Count()
    {
        return _db.Connection.Table<Student>().Count();
    }

    public int CountByStatus(string status)
    {
        return _db.Connection.Table<Student>().Count(s => s.Status == status);
    }

    // FR-10: search by Student ID, name, or parent contact number. Also filters by grade and status.
    public List<Student> Search(string? text = null, string? grade = null, IEnumerable<string>? statuses = null)
    {
        var query = _db.Connection.Table<Student>().ToList().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(text))
        {
            string key = text.Trim().ToLower();
            string digits = new string(key.Where(char.IsDigit).ToArray());

            query = query.Where(s =>
                s.FullName.ToLower().Contains(key)
                || s.StudentId.ToLower().Contains(key)
                || s.Email.ToLower().Contains(key)
                || (digits.Length >= 3 && new string(s.GuardianPhone.Where(char.IsDigit).ToArray()).Contains(digits)));
        }

        if (!string.IsNullOrWhiteSpace(grade) && grade != "All Grades")
        {
            query = query.Where(s => s.Grade == grade);
        }

        if (statuses != null)
        {
            var allowed = statuses.ToList();
            if (allowed.Count > 0)
            {
                query = query.Where(s => allowed.Contains(s.Status));
            }
        }

        var result = query.OrderBy(s => s.FullName).ToList();
        FillTeacherNames(result);
        return result;
    }

    // ---------- Write ----------

    // FR-03: register a new student. Throws if required fields are missing or the ID is taken.
    public Student Register(Student student)
    {
        Validate(student);

        if (string.IsNullOrWhiteSpace(student.StudentId))
        {
            student.StudentId = NextStudentId();
        }
        student.StudentId = student.StudentId.Trim().ToUpper();

        if (StudentIdExists(student.StudentId))
        {
            throw new InvalidOperationException($"Student ID '{student.StudentId}' already exists.");
        }

        if (student.JoiningDate == default) student.JoiningDate = DateTime.Today;
        if (string.IsNullOrWhiteSpace(student.Status)) student.Status = "Active";

        _db.Connection.Insert(student);
        return student;
    }

    // FR-04: update an existing student.
    public void Update(Student student)
    {
        Validate(student);

        var other = _db.Connection.Table<Student>().FirstOrDefault(s => s.StudentId == student.StudentId && s.Id != student.Id);
        if (other != null)
        {
            throw new InvalidOperationException($"Student ID '{student.StudentId}' belongs to another student.");
        }

        _db.Connection.Update(student);
    }

    // Stores the path of the student's profile photo (the file itself is copied by the app).
    public void SetPhoto(int studentId, string photoPath)
    {
        var student = _db.Connection.Get<Student>(studentId);
        student.PhotoPath = photoPath ?? "";
        _db.Connection.Update(student);
    }

    public void SaveNotes(int studentId, string notes)
    {
        var student = _db.Connection.Get<Student>(studentId);
        student.Notes = notes ?? "";
        _db.Connection.Update(student);
    }

    // Student departure / resignation: keeps the record but marks it Inactive or Dropout.
    public void ProcessDeparture(int studentId, DateTime departureDate, string reason, string notes, bool isDropout = false)
    {
        var student = _db.Connection.Get<Student>(studentId);
        student.Status = isDropout ? "Dropout" : "Inactive";
        student.DepartureDate = departureDate;
        student.DepartureReason = reason ?? "";
        if (!string.IsNullOrWhiteSpace(notes))
        {
            student.Notes = string.IsNullOrWhiteSpace(student.Notes) ? notes : student.Notes + "\n" + notes;
        }
        _db.Connection.Update(student);
    }

    public void Delete(int studentId)
    {
        _db.Connection.Delete<Student>(studentId);
    }

    // ---------- Helpers ----------

    public bool StudentIdExists(string studentId)
    {
        string key = studentId.Trim().ToUpper();
        return _db.Connection.Table<Student>().Any(s => s.StudentId == key);
    }

    // Makes the next ID like "STU-2026-0001".
    public string NextStudentId()
    {
        string prefix = $"STU-{DateTime.Today.Year}-";
        int count = _db.Connection.Table<Student>().Count(s => s.StudentId.StartsWith(prefix));
        string candidate;
        do
        {
            count++;
            candidate = prefix + count.ToString("0000");
        } while (StudentIdExists(candidate));
        return candidate;
    }

    private static void Validate(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.FullName)) throw new ArgumentException("Full name is required.");
        if (student.DateOfBirth == default) throw new ArgumentException("Date of birth is required.");
        if (string.IsNullOrWhiteSpace(student.Grade)) throw new ArgumentException("Grade is required.");
        if (string.IsNullOrWhiteSpace(student.GuardianName)) throw new ArgumentException("Parent / guardian name is required.");
        if (string.IsNullOrWhiteSpace(student.GuardianPhone)) throw new ArgumentException("Parent / guardian phone number is required.");
    }

    private void FillTeacherNames(IEnumerable<Student> students)
    {
        var teachers = _db.Connection.Table<Teacher>().ToList().ToDictionary(t => t.Id, t => t.FullName);
        foreach (var s in students)
        {
            s.AssignedTeacherName = s.AssignedTeacherId.HasValue && teachers.TryGetValue(s.AssignedTeacherId.Value, out var name) ? name : "";
        }
    }
}
