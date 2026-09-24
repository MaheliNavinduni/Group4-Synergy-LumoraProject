using SQLite;

namespace LumoraAcademy.Core.Entities;

// A class the institute runs, e.g. "Science - Grade 10" taught by Miss. Aries for Rs. 2500 a month.
// Students enrol in a ClassGroup (see Enrollment), attendance and payments are recorded per class.
// The weekly time slots of the class are ClassSession rows.
[Table("ClassGroups")]
public class ClassGroup
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int SubjectId { get; set; }                // Subjects.Id
    public int TeacherId { get; set; }                // Teachers.Id
    public string Grade { get; set; } = "";           // e.g. "10th Grade"
    public string Room { get; set; } = "";
    public decimal MonthlyFee { get; set; }
    public bool IsActive { get; set; } = true;

    // ---- Helpers for the screens (not stored) ----

    [Ignore]
    public string SubjectName { get; set; } = "";

    [Ignore]
    public string TeacherName { get; set; } = "";

    [Ignore]
    public int StudentCount { get; set; }

    // "Science - 10th Grade"
    [Ignore]
    public string Name => $"{SubjectName} - {Grade}";

    [Ignore]
    public string RoomText => string.IsNullOrWhiteSpace(Room) ? "" : "Room " + Room;

    [Ignore]
    public string StudentCountText => $"{StudentCount} students";

    [Ignore]
    public string FeeText => MonthlyFee.ToString("N2");

    // Days and times of this class, filled in when the screen needs them.
    [Ignore]
    public string Days { get; set; } = "";

    [Ignore]
    public string TimeRange { get; set; } = "";
}
