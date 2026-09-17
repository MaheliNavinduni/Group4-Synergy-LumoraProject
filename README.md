# Group4-Synergy-LumoraProject
Desktop application for Lumora institute

## Tech stack
- **Frontend:** .NET MAUI (C#), Windows desktop
- **Framework:** .NET 8
- **Database:** not connected yet (SQLite / MySQL planned)

## How to run
1. Open `LumoraAcademy.sln` in Visual Studio 2022 (with the .NET MAUI workload installed).
2. Make sure the run target says **Windows Machine**.
3. Press **F5**.

Or from a terminal:

```
dotnet build LumoraAcademy/LumoraAcademy.csproj -f net8.0-windows10.0.19041.0
```

If the app crashes on start after switching between packaged and unpackaged builds, delete the
`LumoraAcademy/bin` and `LumoraAcademy/obj` folders and build again.

## Demo logins (temporary, no database yet)
| Username | Password   | Opens             |
|----------|------------|-------------------|
| admin    | admin123   | Admin dashboard   |
| teacher  | teacher123 | Teacher dashboard |

These are hard-coded in `Pages/LoginPage.xaml.cs` and must be replaced with a real
database check (with hashed passwords) once the backend exists.

## Project layout
```
LumoraAcademy/
  App.xaml / App.xaml.cs        Starts on the Home page
  MauiProgram.cs                App setup
  Models/Models.cs              Data classes (Student, Teacher, PaymentAccount, ...)
  Data/SampleData.cs            Sample rows that fill the screens + icon glyph codes
  Services/AppNavigation.cs     Moving between pages, current role, logout
  Converters/                   Small XAML helpers
  Controls/                     Reusable pieces used by many pages
    SidebarView                 Left menu (Admin or Teacher items)
    StatCard                    Small statistic card
    StatusBadge                 Coloured pill (Active, Paid, Excellent, ...)
    AvatarView                  Round initials avatar
    AttendanceTableView         Daily Records table (Teacher + Admin)
    StudentDirectoryView        Student directory with filters (Teacher + Admin)
    MonthCalendarView           Month calendar with sample events
  Pages/
    HomePage, LoginPage
    Teacher/                    Dashboard, My Subjects, Attendance, Students, Upcoming Events
    Shared/                     Student Details, At-Risk Students (both roles)
    Admin/                      Dashboard, Students, Teachers, Payments, Academics, Attendance,
                                Events, Student Registration, Teacher Registration, Teacher Details,
                                Edit Teacher, Edit Payment, Add Subject, Mark Attendance,
                                Print Student Report, Teacher Resignation, Student Departure
  Resources/Styles/Brand.xaml   All brand colours and shared styles
```

## Where things are
- **Colours and styles:** `Resources/Styles/Brand.xaml`. Change a colour there and every page updates.
- **Sample data:** `Data/SampleData.cs`. Replace these lists with database queries later.
- **Icons:** the built-in Windows font "Segoe MDL2 Assets" (no icon files needed). Codes are in
  `SampleData.Icons`, and used in XAML as `Text="&#xE787;"`.
- **Adding a page:** copy any Admin page, change the class name, set `ActiveItem` on the sidebar,
  and add it to `AppNavigation.GoToMenuItemAsync` if it belongs in the menu.

## Still to do
- Connect the Login page and every "Save" button to the database
- Real search, filters and paging (currently visual only)
- PDF export for the student report
- Photo upload for students and teachers
