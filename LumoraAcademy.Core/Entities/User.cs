using SQLite;

namespace LumoraAcademy.Core.Entities;

// A login account. Role is "Admin" or "Teacher".
[Table("Users")]
public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed(Unique = true)]
    public string Username { get; set; } = "";

    // We never store the real password - only a salted hash (see AuthService).
    public string PasswordHash { get; set; } = "";
    public string PasswordSalt { get; set; } = "";

    public string Role { get; set; } = "Teacher";      // "Admin" or "Teacher"
    public string DisplayName { get; set; } = "";
    public bool IsActive { get; set; } = true;

    // Filled in when the account belongs to a teacher record.
    public int? TeacherId { get; set; }
}
