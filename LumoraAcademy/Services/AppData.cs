using LumoraAcademy.Core;

namespace LumoraAcademy.Services;

// Gives every page access to the backend (database + services).
// The database file lives in the app's data folder, e.g.
//   C:\Users\<you>\AppData\Local\Packages\...\LocalState\lumora.db
public static class AppData
{
    private static Backend? _backend;

    public static Backend Backend
    {
        get
        {
            if (_backend == null)
            {
                string path = Path.Combine(FileSystem.AppDataDirectory, "lumora.db");
                _backend = new Backend(path, seedDemoData: true);
            }
            return _backend;
        }
    }

    // The teacher record of the logged-in teacher (null for Admin).
    public static int? CurrentTeacherId { get; set; }

    // Shortcuts so pages can write AppData.Students instead of AppData.Backend.Students.
    public static Core.Services.AuthService Auth => Backend.Auth;
    public static Core.Services.StudentService Students => Backend.Students;
    public static Core.Services.TeacherService Teachers => Backend.Teachers;
    public static Core.Services.PaymentService Payments => Backend.Payments;
    public static Core.Services.AcademicService Academics => Backend.Academics;
    public static Core.Services.AttendanceService Attendance => Backend.Attendance;
    public static Core.Services.EventService Events => Backend.Events;
    public static Core.Services.ClassService Classes => Backend.Classes;
    public static Core.Services.ReportService Reports => Backend.Reports;
}
