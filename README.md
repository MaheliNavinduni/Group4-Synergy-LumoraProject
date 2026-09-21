# Group4-Synergy-LumoraProject
Desktop Student Management System for Lumora Educational Institute (ICT 2243).

## Tech stack
- **Frontend:** .NET MAUI (C#), Windows desktop - project `LumoraAcademy`
- **Backend:** C# .NET 8 class library - project `LumoraAcademy.Core`
- **Database:** SQLite (file `lumora.db` created automatically on first run)
- **Tests:** xUnit - project `LumoraAcademy.Tests` (81 tests)
- **PDF reports:** QuestPDF (community licence)

## How to run
1. Open `LumoraAcademy.sln` in Visual Studio 2022 (with the .NET MAUI workload installed).
2. Make sure the startup project is `LumoraAcademy` and the run target says **Windows Machine**.
3. Press **F5**.

From a terminal:

```
dotnet build LumoraAcademy/LumoraAcademy.csproj -f net8.0-windows10.0.19041.0
dotnet test LumoraAcademy.Tests/LumoraAcademy.Tests.csproj
```

If the app crashes on start after switching between packaged and unpackaged builds, delete the
`LumoraAcademy/bin` and `LumoraAcademy/obj` folders and build again.

## Demo logins
The first time the app runs it fills the database with demo data, including two accounts:

| Username | Password   | Role    |
|----------|------------|---------|
| admin    | admin123   | Admin   |
| teacher  | teacher123 | Teacher (Miss. Aries) |

Passwords are stored as salted PBKDF2 hashes (`Core/Security/PasswordHasher.cs`), never as plain text.
New teacher accounts are created from **Admin -> Teachers -> Register New Teacher**.

The database file lives in the app's data folder (on Windows:
`%LOCALAPPDATA%\Packages\<app id>\LocalState\lumora.db`). Delete it to start again with fresh demo data.

## Solution layout
```
LumoraAcademy.sln
LumoraAcademy.Core/                 BACKEND
  Backend.cs                        One object that opens the database and exposes every service
  Database/AppDatabase.cs           Opens SQLite, creates tables, backup
  Database/DemoDataSeeder.cs        Demo rows inserted on first run
  Security/PasswordHasher.cs        Password hashing
  Entities/                         Tables: User, Student, Teacher, Subject, Payment, ExamResult,
                                    AttendanceEntry, ProgressNote, SchoolEvent, ClassSession
  Services/
    AuthService                     Epic 1 - login, create/deactivate accounts
    StudentService                  Epic 2 - register, update, search (ID / name / parent phone), departure
    TeacherService                  Epic 1 US2 - register with login, update, resignation
    PaymentService                  Epic 3 - fees, record payments, status rules, extensions
    AcademicService                 Epic 4 - subjects, marks, Cambridge results, at-risk, progress notes
    AttendanceService               Mark attendance, daily summaries, statistics
    EventService                    Calendar events
    ScheduleService                 Weekly timetable (ClassSessions) with clash checking
    ReportService                   Epic 5 - student progress report, payment and academic reports
  Reports/StudentReportPdf.cs       Builds the Academic Progress Report PDF

LumoraAcademy.Tests/                81 xUnit tests, one file per service

LumoraAcademy/                      FRONTEND (.NET MAUI)
  Services/AppData.cs               Gives pages access to the backend (AppData.Students, AppData.Payments ...)
  Services/AppNavigation.cs         Moving between pages, current role, logout
  Services/PhotoService.cs          Picks a JPG/PNG and copies it into the app's Photos folder
  Controls/                         Sidebar, StatCard, StatusBadge, AvatarView (photo or initials), PagerView,
                                    AttendanceTableView, StudentDirectoryView, MonthCalendarView
  Pages/
    HomePage, LoginPage
    Teacher/                        Dashboard, My Subjects, Attendance, Students, Institutional Calendar
    Shared/                         Student Details, At-Risk Students, Enter Marks
    Admin/                          Dashboard, Students, Teachers, Payments, Academics, Attendance, Events,
                                    Student Registration, Edit Student, Student Departure,
                                    Teacher Registration, Teacher Details, Edit Teacher, Teacher Resignation,
                                    Edit Payment, Add Subject, Mark Attendance, Print Student Report, Timetable
  Resources/Styles/Brand.xaml       All brand colours and shared styles
  Data/SampleData.cs                Icon glyph codes + a few sample rows for the admin dashboard activity feed
```

## How a page talks to the backend
```csharp
var students = AppData.Students.Search("chen");            // read
AppData.Payments.RecordPayment(paymentId, 450, DateTime.Today);   // write
```
Every service validates its input and throws an exception with a readable message; pages catch it
and show it with `DisplayAlert`.

## Epic coverage
| Epic | Status |
|------|--------|
| E1 Authentication & teacher accounts | Done - login, register teacher with login, edit, resignation deactivates login |
| E2 Student management | Done - register (unique ID), edit, search by ID / name / parent phone, departure |
| E3 Payments | Done - fee records, record payment, automatic status, balances, 1-3 month extensions |
| E4 Academic performance | Done - monthly/term/year-end marks, Cambridge results, at-risk list, progress notes |
| E5 Reports | Done - student progress report as a PDF (opens in the PDF viewer for printing), payment and academic report data |
| E6 Database | Done - SQLite with tables for every record type, backup helper |

## Also done
- Weekly timetable: Admin -> Teachers -> (teacher) -> Manage Timetable. Feeds the teacher dashboard and subject cards.
- Photo upload (JPG/PNG up to 2 MB) on student and teacher registration and edit pages.
- Paging (10 rows per page) on every long table.

## Still to do
- Payment and academic reports as PDF (only the student progress report has a PDF so far)
- Backup button in the UI (the backend has `Database.Backup()`)
