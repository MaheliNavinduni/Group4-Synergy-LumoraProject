using LumoraAcademy.Data;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Teacher;

public partial class TeacherDashboardPage : ContentPage
{
    public TeacherDashboardPage()
    {
        InitializeComponent();

        // Fill the schedule list with today's lessons.
        BindableLayout.SetItemsSource(ScheduleList, SampleData.TeacherTodaySchedule);
    }

    private async void OnFullCalendarTapped(object sender, EventArgs e)
    {
        await AppNavigation.GoToMenuItemAsync("Upcoming Events");
    }
}
