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

        // One card per class this teacher takes
        var cards = AppData.CurrentTeacherId.HasValue
            ? AppData.Classes.GetForTeacher(AppData.CurrentTeacherId.Value)
            : new List<Core.Entities.ClassGroup>();
        BindableLayout.SetItemsSource(SubjectList, cards);
    }
}
