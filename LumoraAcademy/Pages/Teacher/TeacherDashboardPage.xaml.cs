using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Teacher;

public partial class TeacherDashboardPage : ContentPage
{
    public TeacherDashboardPage()
    {
        InitializeComponent();

        WelcomeLabel.Text = $"Welcome, {AppNavigation.CurrentUserName}";

        // Today's lessons for the logged-in teacher, from the timetable.
        var lessons = AppData.CurrentTeacherId.HasValue
            ? AppData.Schedule.GetDayForTeacher(AppData.CurrentTeacherId.Value)
            : new List<Core.Entities.ClassSession>();
        BindableLayout.SetItemsSource(ScheduleList, lessons);
        NoLessonsLabel.IsVisible = lessons.Count == 0;
    }

    private async void OnFullCalendarTapped(object sender, EventArgs e)
    {
        await AppNavigation.GoToMenuItemAsync("Upcoming Events");
    }
}
