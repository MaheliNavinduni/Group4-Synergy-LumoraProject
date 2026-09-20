using LumoraAcademy.Core.Database;
using LumoraAcademy.Core.Entities;

namespace LumoraAcademy.Core.Services;

// School calendar events (Event Management / Institutional Calendar pages).
public class EventService
{
    private readonly AppDatabase _db;

    public EventService(AppDatabase db)
    {
        _db = db;
    }

    public List<SchoolEvent> GetAll()
    {
        return _db.Connection.Table<SchoolEvent>().OrderBy(e => e.Date).ToList();
    }

    public SchoolEvent? GetById(int id) => _db.Connection.Find<SchoolEvent>(id);

    // Events from today onwards, soonest first.
    public List<SchoolEvent> GetUpcoming(int max = 10, bool includeAdminOnly = true)
    {
        var today = DateTime.Today;
        var list = _db.Connection.Table<SchoolEvent>().Where(e => e.Date >= today).OrderBy(e => e.Date).ToList();
        if (!includeAdminOnly) list = list.Where(e => e.Visibility != "Admin Only").ToList();
        return list.Take(max).ToList();
    }

    public List<SchoolEvent> GetForMonth(int year, int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1);
        return _db.Connection.Table<SchoolEvent>().Where(e => e.Date >= start && e.Date < end).OrderBy(e => e.Date).ToList();
    }

    public SchoolEvent Create(SchoolEvent ev)
    {
        Validate(ev);
        _db.Connection.Insert(ev);
        return ev;
    }

    public void Update(SchoolEvent ev)
    {
        Validate(ev);
        _db.Connection.Update(ev);
    }

    public void Delete(int id) => _db.Connection.Delete<SchoolEvent>(id);

    private static void Validate(SchoolEvent ev)
    {
        if (string.IsNullOrWhiteSpace(ev.Title)) throw new ArgumentException("Event title is required.");
        if (ev.Date == default) throw new ArgumentException("Event date is required.");
        if (string.IsNullOrWhiteSpace(ev.Category)) ev.Category = "Academic";
        if (string.IsNullOrWhiteSpace(ev.Visibility)) ev.Visibility = "Staff";
    }
}
