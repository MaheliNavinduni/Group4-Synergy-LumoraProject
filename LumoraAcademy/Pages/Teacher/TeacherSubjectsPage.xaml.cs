using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Teacher;

public partial class TeacherSubjectsPage : ContentPage
{
    public TeacherSubjectsPage()
    {
        InitializeComponent();

        // Profile card for the logged-in teacher
        var teacher = AppData.CurrentTeacherId.HasValue ? AppData.Teachers.GetById(AppData.CurrentTeacherId.Value) : null;
        NameLabel.Text = teacher?.FullName ?? AppNavigation.CurrentUserName;
        Avatar.Initials = teacher?.Initials ?? "";
        DesignationLabel.Text = teacher?.Designation ?? "";
        EmailLabel.Text = teacher?.Email ?? "";

        // Subject cards built from this teacher's timetable
        var cards = AppData.CurrentTeacherId.HasValue
            ? AppData.Schedule.GetSubjectSummariesForTeacher(AppData.CurrentTeacherId.Value)
            : new List<Core.Services.SubjectSummary>();
        BindableLayout.SetItemsSource(SubjectList, cards);
    }
}
