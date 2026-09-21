using SQLite;

namespace LumoraAcademy.Core.Entities;

// A calendar event shown on the Event Management and Institutional Calendar pages.
[Table("Events")]
public class SchoolEvent
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; } = "";

    [Indexed]
    public DateTime Date { get; set; }

    public string Time { get; set; } = "";            // e.g. "09:00 AM - 03:00 PM" or "All Day"
    public string Location { get; set; } = "";
    public string Category { get; set; } = "";        // Academic, Admin, Holiday, Sports
    public string Description { get; set; } = "";
    public string Visibility { get; set; } = "Staff"; // Staff or Admin Only

    [Ignore]
    public string DateText => Date.ToString("MMM dd, yyyy");
}
