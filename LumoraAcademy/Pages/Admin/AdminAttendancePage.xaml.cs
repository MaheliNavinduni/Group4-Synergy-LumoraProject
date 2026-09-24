using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class AdminAttendancePage : ContentPage
{
    public AdminAttendancePage()
    {
        InitializeComponent();

        LoadStats();
        Loaded += (s, e) => LoadStats();
    }

    private void LoadStats()
    {
        var stats = AppData.Attendance.GetStats();
        RateCard.Value = $"{stats.AverageRate:0.0}%";
        DaysCard.Value = stats.TotalDaysLogged.ToString();
        DaysCard.Subtitle = $"Academic Year {DateTime.Today.Year}";
        PerfectCard.Value = stats.PerfectDays.ToString();
    }

    private async void OnMarkAttendanceClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new MarkAttendancePage());
    }
}
