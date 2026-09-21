using SQLite;

namespace LumoraAcademy.Core.Entities;

// A teacher's note about a student, e.g. "Recommend extra maths class on Saturdays" (FR-09).
[Table("ProgressNotes")]
public class ProgressNote
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int StudentId { get; set; }

    public int? TeacherId { get; set; }
    public string Note { get; set; } = "";
    public bool RecommendExtraClass { get; set; }
    public DateTime CreatedOn { get; set; }
}
