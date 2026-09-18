using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class AdminAttendancePage : ContentPage
{
    public AdminAttendancePage()
    {
        InitializeComponent();
    }

    private async void OnMarkAttendanceClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new MarkAttendancePage());
    }
}
