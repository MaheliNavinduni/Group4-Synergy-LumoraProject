using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Teacher;

public partial class TeacherDashboardPage : ContentPage
{
    public TeacherDashboardPage()
    {
        InitializeComponent();

        WelcomeLabel.Text = $"Welcome, {AppNavigation.CurrentUserName}";

        // Today's lessons for the logged-in teacher, taken from the class timetable.
        var lessons = AppData.Classes.GetSessionsForDay(DateTime.Today)
            .Where(s => s.ClassGroup != null && s.ClassGroup.TeacherId == AppData.CurrentTeacherId)
            .ToList();
        BindableLayout.SetItemsSource(ScheduleList, lessons);
        NoLessonsLabel.IsVisible = lessons.Count == 0;
    }

    private async void OnFullCalendarTapped(object sender, EventArgs e)
    {
        await AppNavigation.GoToMenuItemAsync("Upcoming Events");
    }
}
