using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Services;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// Admin sets up the classes here: subject, teacher, fee, weekly times and who is in the class.
// Attendance and the fee register both work off this.
public partial class ClassesPage : ContentPage
{
    private static readonly string[] Days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

    private List<Subject> _subjects = new();
    private List<Core.Entities.Teacher> _teachers = new();
    private List<Student> _pickableStudents = new();
    private ClassGroup? _managing;

    public ClassesPage()
    {
        InitializeComponent();

        _subjects = AppData.Academics.GetSubjects();
        SubjectPicker.ItemsSource = _subjects.Select(s => s.DisplayName).ToList();
        if (_subjects.Count > 0) SubjectPicker.SelectedIndex = 0;

        _teachers = AppData.Teachers.GetActive();
        TeacherPicker.ItemsSource = _teachers.Select(t => t.FullName).ToList();
        if (_teachers.Count > 0) TeacherPicker.SelectedIndex = 0;

        GradePicker.ItemsSource = new List<string> { "6th Grade", "7th Grade", "8th Grade", "9th Grade", "10th Grade", "11th Grade", "12th Grade", "13th Grade" };
        GradePicker.SelectedIndex = 4;

        DayPicker.ItemsSource = Days.ToList();
        DayPicker.SelectedIndex = 1;

        LoadClasses();
    }

    // ---------- The class list ----------

    private void LoadClasses()
    {
        var classes = AppData.Classes.GetAll();
        BindableLayout.SetItemsSource(ClassList, classes);
        NoClassesLabel.IsVisible = classes.Count == 0;
    }

    private async void OnAddClassClicked(object sender, EventArgs e)
    {
        string problem = Validation.FirstProblem(
            SubjectPicker.SelectedIndex < 0 ? "Please choose the subject." : "",
            GradePicker.SelectedIndex < 0 ? "Please choose the grade." : "",
            TeacherPicker.SelectedIndex < 0 ? "Please choose the teacher." : "",
            Validation.Money(FeeEntry.Text, "Monthly fee", 1, 100000));

        if (problem != "")
        {
            await DisplayAlert("Add Class", problem, "OK");
            return;
        }

        decimal fee = decimal.Parse(FeeEntry.Text!.Trim());

        var group = new ClassGroup
        {
            SubjectId = _subjects[SubjectPicker.SelectedIndex].Id,
            TeacherId = _teachers[TeacherPicker.SelectedIndex].Id,
            Grade = (string)GradePicker.SelectedItem,
            Room = (RoomEntry.Text ?? "").Trim(),
            MonthlyFee = fee,
            IsActive = true,
        };

        try
        {
            AppData.Classes.Create(group);
            RoomEntry.Text = "";
            FeeEntry.Text = "";
            LoadClasses();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Add Class", ex.Message, "OK");
        }
    }

    private async void OnCloseClassClicked(object sender, EventArgs e)
    {
        if (sender is not Button b || b.BindingContext is not ClassGroup group) return;

        bool confirm = await DisplayAlert("Close Class",
            $"Close {group.Name}? The students will be taken out of it, but the attendance and payment history is kept.",
            "Close Class", "Cancel");
        if (!confirm) return;

        AppData.Classes.Deactivate(group.Id);
        if (_managing?.Id == group.Id) CloseManage();
        LoadClasses();
    }

    // ---------- Managing one class ----------

    private void OnManageClicked(object sender, EventArgs e)
    {
        if (sender is Button b && b.BindingContext is ClassGroup group)
        {
            _managing = group;
            ManageTitle.Text = group.Name;
            ManageSubtitle.Text = $"{group.TeacherName} • {group.RoomText} • Fee {group.MonthlyFee:N2} a month";
            ManageSection.IsVisible = true;

            LoadSessions();
            LoadStudents();
        }
    }

    private void OnCloseManageClicked(object sender, EventArgs e) => CloseManage();

    private void CloseManage()
    {
        _managing = null;
        ManageSection.IsVisible = false;
    }

    // ---------- Weekly times ----------

    private void LoadSessions()
    {
        if (_managing == null) return;

        var sessions = AppData.Classes.GetSessions(_managing.Id);
        BindableLayout.SetItemsSource(SessionList, sessions);
        NoSessionsLabel.IsVisible = sessions.Count == 0;
    }

    private async void OnAddSessionClicked(object sender, EventArgs e)
    {
        if (_managing == null) return;

        if (DayPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Weekly Times", "Please choose the day of the week.", "OK");
            return;
        }
        if (EndPicker.Time <= StartPicker.Time)
        {
            await DisplayAlert("Weekly Times", "The finish time must be after the start time.", "OK");
            return;
        }

        var session = new ClassSession
        {
            ClassGroupId = _managing.Id,
            DayOfWeek = DayPicker.SelectedIndex,
            StartTime = StartPicker.Time,
            EndTime = EndPicker.Time,
        };

        try
        {
            AppData.Classes.AddSession(session);   // checks the times and that the teacher is free
            LoadSessions();
            LoadClasses();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Weekly Times", ex.Message, "OK");
        }
    }

    private void OnDeleteSessionTapped(object sender, EventArgs e)
    {
        if (sender is Label label && label.BindingContext is ClassSession session)
        {
            AppData.Classes.DeleteSession(session.Id);
            LoadSessions();
            LoadClasses();
        }
    }

    // ---------- Students ----------

    private void LoadStudents()
    {
        if (_managing == null) return;

        var enrolled = AppData.Classes.GetStudents(_managing.Id);
        BindableLayout.SetItemsSource(StudentList, enrolled);
        NoStudentsLabel.IsVisible = enrolled.Count == 0;

        // The picker offers active students who are not in this class yet.
        var enrolledIds = enrolled.Select(s => s.Id).ToList();
        _pickableStudents = AppData.Students.GetAll()
            .Where(s => s.Status == "Active" && !enrolledIds.Contains(s.Id))
            .ToList();

        StudentPicker.ItemsSource = _pickableStudents.Select(s => $"{s.FullName} ({s.StudentId})").ToList();
        StudentPicker.SelectedIndex = _pickableStudents.Count > 0 ? 0 : -1;
    }

    private async void OnEnrolClicked(object sender, EventArgs e)
    {
        if (_managing == null || StudentPicker.SelectedIndex < 0) return;

        try
        {
            AppData.Classes.Enroll(_pickableStudents[StudentPicker.SelectedIndex].Id, _managing.Id);
            LoadStudents();
            LoadClasses();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Enrol Student", ex.Message, "OK");
        }
    }

    private async void OnUnenrolTapped(object sender, EventArgs e)
    {
        if (_managing == null || sender is not Label label || label.BindingContext is not Student student) return;

        bool confirm = await DisplayAlert("Remove Student",
            $"Take {student.FullName} out of {_managing.Name}? Their attendance and payment history is kept.",
            "Remove", "Cancel");
        if (!confirm) return;

        AppData.Classes.Unenroll(student.Id, _managing.Id);
        LoadStudents();
        LoadClasses();
    }
}
