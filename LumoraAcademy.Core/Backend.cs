using LumoraAcademy.Core.Database;
using LumoraAcademy.Core.Services;

namespace LumoraAcademy.Core;

// The single entry point to the backend.
// The MAUI app creates one Backend at startup and every page uses its services:
//
//     Backend.Students.Search("chen");
//     Backend.Payments.RecordPayment(...);
//
public class Backend
{
    public AppDatabase Database { get; }
    public AuthService Auth { get; }
    public StudentService Students { get; }
    public TeacherService Teachers { get; }
    public PaymentService Payments { get; }
    public AcademicService Academics { get; }
    public AttendanceService Attendance { get; }
    public EventService Events { get; }
    public ClassService Classes { get; }
    public ReportService Reports { get; }

    public Backend(string databasePath, bool seedDemoData = true)
    {
        Database = new AppDatabase(databasePath);

        if (seedDemoData)
        {
            DemoDataSeeder.SeedIfEmpty(Database);
        }

        Classes = new ClassService(Database);
        Auth = new AuthService(Database);
        Students = new StudentService(Database);
        Teachers = new TeacherService(Database, Auth);
        Payments = new PaymentService(Database, Classes);
        Academics = new AcademicService(Database);
        Attendance = new AttendanceService(Database, Classes);
        Events = new EventService(Database);

        Reports = new ReportService(Database, Academics, Attendance, Payments);
    }
}
