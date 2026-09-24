using LumoraAcademy.Core.Entities;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Shared;

// Shown to both Teachers and Admins.
// Admins also get the "Print Report", "Edit Student" and "Student Resignation" buttons.
public partial class StudentDetailsPage : ContentPage
{
    private readonly int _studentId;
    private Student? _student;

    public StudentDetailsPage(int studentId)
    {
        InitializeComponent();
        _studentId = studentId;

        bool isAdmin = AppNavigation.CurrentRole == "Admin";
        PrintButton.IsVisible = isAdmin;
        EditButton.IsVisible = isAdmin;
        ResignationButton.IsVisible = isAdmin;

        LoadStudent();
        Loaded += (s, e) => LoadStudent();   // refresh after editing or entering marks
    }

    // Reads the student and their statistics from the database and fills the screen.
    private void LoadStudent()
    {
        _student = AppData.Students.GetById(_studentId);
        if (_student == null) return;

        TitleName.Text = _student.FullName;
        IdLabel.Text = "ID: " + _student.StudentId;
        StatusBadge.Text = _student.Status == "Active" ? "Enrolled" : _student.Status;
        SchoolLabel.Text = string.IsNullOrWhiteSpace(_student.School) ? "Lumora Educational Institute" : _student.School;

        Avatar.Initials = _student.Initials;
        Avatar.ImagePath = _student.PhotoPath;
        CardName.Text = _student.FullName;
        GradeLabel.Text = string.IsNullOrWhiteSpace(_student.Section) ? _student.Grade : $"{_student.Grade}, Section {_student.Section}";
        DobLabel.Text = _student.DateOfBirth.ToString("MMM dd, yyyy");
        GenderLabel.Text = _student.Gender;
        JoinedLabel.Text = _student.JoiningDate.ToString("MMM dd, yyyy");
        GuardianLabel.Text = _student.GuardianName;
        GuardianPhoneLabel.Text = _student.GuardianPhone;
        GuardianEmailLabel.Text = _student.GuardianEmail;
        NotesEditor.Text = _student.Notes;

        // Statistics cards
        var report = AppData.Reports.BuildStudentReport(_studentId);
        GpaCard.Value = report.Rows.Count == 0 ? "-" : report.Gpa.ToString("0.00");
        GpaCard.Subtitle = report.Rows.Count == 0 ? "No marks yet" : $"{report.Rows.Count} subjects";

        double rate = AppData.Attendance.AttendanceRateForStudent(_studentId);
        int absences = AppData.Attendance.AbsencesForStudent(_studentId);
        AttendanceCard.Value = report.DaysTotal == 0 ? "-" : $"{rate:0}%";
        AttendanceCard.Subtitle = report.DaysTotal == 0 ? "No records" : $"{absences} Absences";

        // Classes the student is enrolled in, and what they still owe
        var classes = AppData.Classes.GetClassesForStudent(_studentId);
        decimal owed = AppData.Payments.OutstandingForStudent(_studentId);

        TeacherCard.Title = "CLASSES";
        TeacherCard.Value = classes.Count.ToString();
        TeacherCard.Subtitle = classes.Count == 0 ? "Not enrolled yet" : string.Join(", ", classes.Select(c => c.SubjectName));

        FeesCard.Value = owed == 0 ? "Settled" : owed.ToString("N2");
        FeesCard.Subtitle = owed == 0 ? "Nothing owing" : "Outstanding fees";



        // Recent exam results (latest 5)
        var results = AppData.Academics.GetResultsForStudent(_studentId).Take(5).ToList();
        BindableLayout.SetItemsSource(ScheduleList, results);
        NoResultsLabel.IsVisible = results.Count == 0;
    }

    private async void OnSaveNotesClicked(object sender, EventArgs e)
    {
        AppData.Students.SaveNotes(_studentId, NotesEditor.Text ?? "");
        await DisplayAlert("Notes", "Notes saved.", "OK");
    }

    private async void OnPrintReportClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new Admin.PrintStudentReportPage(_studentId));
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new Admin.EditStudentPage(_studentId));
    }

    private async void OnEnterMarksClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new EnterMarksPage(_studentId));
    }

    private async void OnResignationClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new Admin.StudentDeparturePage(_studentId));
    }

    private async void OnGoBackClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }
}
