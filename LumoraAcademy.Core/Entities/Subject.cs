using SQLite;

namespace LumoraAcademy.Core.Entities;

// A subject that can be taught, e.g. "Mathematics - M2103 (National Curriculum)".
[Table("Subjects")]
public class Subject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed(Unique = true)]
    public string Code { get; set; } = "";            // e.g. "M2103"

    public string Name { get; set; } = "";            // e.g. "Mathematics"
    public string Curriculum { get; set; } = "";      // e.g. "National Curriculum", "Cambridge Curriculum"

    [Ignore]
    public string DisplayName => $"{Name} - {Code}";
}
