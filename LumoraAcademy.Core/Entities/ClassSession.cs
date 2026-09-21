using SQLite;

namespace LumoraAcademy.Core.Entities;

// One lesson in the weekly timetable, e.g. "Mathematics, Grade 10, Monday 08:00-09:30, Room 204".
[Table("ClassSessions")]
public class ClassSession
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int TeacherId { get; set; }                // Teachers.Id

    public int? SubjectId { get; set; }               // Subjects.Id
    public string ClassName { get; set; } = "";       // e.g. "10th Grade" (matches Student.Grade)
    public int DayOfWeek { get; set; }                // 0 = Sunday ... 6 = Saturday (same as System.DayOfWeek)
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Room { get; set; } = "";

    // ---- Helpers for the screens (not stored) ----

    [Ignore]
    public string SubjectName { get; set; } = "";

    [Ignore]
    public string TeacherName { get; set; } = "";

    [Ignore]
    public string DayName => ((DayOfWeek)DayOfWeek).ToString();

    [Ignore]
    public string ShortDayName => DayName.Substring(0, 3);

    [Ignore]
    public string StartText => FormatTime(StartTime);

    [Ignore]
    public string EndText => FormatTime(EndTime);

    [Ignore]
    public string TimeRange => $"{StartText} - {EndText}";

    [Ignore]
    public string Detail => string.IsNullOrWhiteSpace(Room) ? ClassName : $"{ClassName} • Room {Room}";

    public static string FormatTime(TimeSpan t) => DateTime.Today.Add(t).ToString("hh:mm tt");
}
