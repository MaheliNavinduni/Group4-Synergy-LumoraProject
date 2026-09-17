using LumoraAcademy.Data;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class AdminDashboardPage : ContentPage
{
    public AdminDashboardPage()
    {
        InitializeComponent();

        BindableLayout.SetItemsSource(ActivityList, SampleData.RecentActivity);
    }

    private async void OnRegisterStudentTapped(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new StudentRegistrationPage());
    }

    private async void OnRecordPaymentTapped(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new EditPaymentPage());
    }
}
