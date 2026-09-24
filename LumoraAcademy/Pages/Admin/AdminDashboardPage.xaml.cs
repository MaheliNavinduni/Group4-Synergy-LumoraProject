using LumoraAcademy.Data;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class AdminDashboardPage : ContentPage
{
    public AdminDashboardPage()
    {
        InitializeComponent();

        LoadDashboard();
        Loaded += (s, e) => LoadDashboard();

        BindableLayout.SetItemsSource(ActivityList, SampleData.RecentActivity);
    }

    // Fills the four cards and the bar chart from the database.
    private void LoadDashboard()
    {
        StudentsCard.Value = AppData.Students.Count().ToString("N0");
        TeachersCard.Value = AppData.Teachers.Count().ToString();

        var summary = AppData.Payments.GetSummary();
        RevenueCard.Value = summary.Collected.ToString("$#,##0");
        PendingCard.Value = summary.UnpaidCount.ToString();

        // Average mark per subject, drawn as bars (170px = 100%).
        var averages = AppData.Reports.AverageBySubject().OrderBy(a => a.Key).ToList();
        var bars = new[] { Bar0, Bar1, Bar2, Bar3, Bar4, Bar5 };
        var labels = new[] { BarLabel0, BarLabel1, BarLabel2, BarLabel3, BarLabel4, BarLabel5 };

        for (int i = 0; i < bars.Length; i++)
        {
            if (i < averages.Count)
            {
                bars[i].HeightRequest = Math.Max(4, averages[i].Value * 1.7);
                bars[i].IsVisible = true;
                labels[i].Text = $"{averages[i].Key} ({averages[i].Value:0}%)";
            }
            else
            {
                bars[i].IsVisible = false;
                labels[i].Text = "";
            }
        }
    }

    private async void OnRegisterStudentTapped(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new StudentRegistrationPage());
    }

    private async void OnRecordPaymentTapped(object sender, EventArgs e)
    {
        await AppNavigation.GoToMenuItemAsync("Payments");
    }
}
