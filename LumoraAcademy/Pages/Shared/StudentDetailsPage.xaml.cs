using LumoraAcademy.Data;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Shared;

// Shown to both Teachers and Admins.
// Admins also get the "Print Report" and "Student Resignation" buttons.
public partial class StudentDetailsPage : ContentPage
{
    public StudentDetailsPage()
    {
        InitializeComponent();

        BindableLayout.SetItemsSource(ScheduleList, SampleData.StudentTodaySchedule);

        bool isAdmin = AppNavigation.CurrentRole == "Admin";
        PrintButton.IsVisible = isAdmin;
        ResignationButton.IsVisible = isAdmin;
    }

    private async void OnPrintReportClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new Admin.PrintStudentReportPage());
    }

    private async void OnResignationClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new Admin.StudentDeparturePage());
    }

    private async void OnGoBackClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }
}
