using SQLite;

namespace LumoraAcademy.Core.Entities;

// One weekly time slot of a class, e.g. "Science - Grade 10, Monday 08:00-09:30".
// The subject, teacher and room come from the ClassGroup.
[Table("ClassSessions")]
public class ClassSession
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int ClassGroupId { get; set; }             // ClassGroups.Id

    public int DayOfWeek { get; set; }                // 0 = Sunday ... 6 = Saturday
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    // ---- Helpers for the screens (not stored) ----

    [Ignore]
    public ClassGroup? ClassGroup { get; set; }

    [Ignore]
    public string SubjectName => ClassGroup?.SubjectName ?? "";

    [Ignore]
    public string TeacherName => ClassGroup?.TeacherName ?? "";

    [Ignore]
    public string Grade => ClassGroup?.Grade ?? "";

    [Ignore]
    public string Room => ClassGroup?.Room ?? "";

    [Ignore]
    public int StudentCount => ClassGroup?.StudentCount ?? 0;

    [Ignore]
    public string ClassName => ClassGroup?.Name ?? "";

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

    // "10th Grade • Room 302"
    [Ignore]
    public string Detail => string.IsNullOrWhiteSpace(Room) ? Grade : $"{Grade} • Room {Room}";

    // Set by the attendance screen: has this class already been marked today?
    [Ignore]
    public bool IsMarked { get; set; }

    [Ignore]
    public string MarkedText => IsMarked ? "Marked" : "Not marked";

    public static string FormatTime(TimeSpan t) => DateTime.Today.Add(t).ToString("hh:mm tt");
}
