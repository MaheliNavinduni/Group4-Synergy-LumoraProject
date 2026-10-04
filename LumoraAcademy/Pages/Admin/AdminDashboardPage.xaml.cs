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
    }

    // The latest things that actually happened: students who joined, fees
    // taken at the office, and classes whose attendance was marked.
    private void LoadRecentActivity()
    {
        var items = new List<(DateTime When, Models.ActivityItem Item)>();

        foreach (var student in AppData.Students.GetAll().OrderByDescending(s => s.JoiningDate).Take(3))
        {
            items.Add((student.JoiningDate, new Models.ActivityItem
            {
                Message = $"{student.FullName} was registered in {student.Grade}.",
                TimeAgo = Ago(student.JoiningDate),
                IconGlyph = SampleData.Icons.Person,
            }));
        }

        foreach (var payment in AppData.Payments.GetRecentlyPaid(3))
        {
            DateTime paidOn = payment.PaymentDate ?? DateTime.Today;

            items.Add((paidOn, new Models.ActivityItem
            {
                Message = $"{payment.StudentName} paid Rs. {payment.AmountPaid:N0} for {payment.ClassName}.",
                TimeAgo = Ago(paidOn),
                IconGlyph = SampleData.Icons.Payments,
            }));
        }

        foreach (var day in AppData.Attendance.GetDailySummaries().Take(2))
        {
            items.Add((day.Date, new Models.ActivityItem
            {
                Message = $"Attendance marked for {day.ClassName}: {day.Present} of {day.Enrolled} present.",
                TimeAgo = Ago(day.Date),
                IconGlyph = SampleData.Icons.Attendance,
            }));
        }

        var newest = items.OrderByDescending(i => i.When).Take(5).Select(i => i.Item).ToList();

        BindableLayout.SetItemsSource(ActivityList, newest);
        NoActivityLabel.IsVisible = newest.Count == 0;
    }

    // Turns a date into wording like "Today" or "3 days ago".
    private static string Ago(DateTime when)
    {
        int days = (DateTime.Today - when.Date).Days;

        if (days <= 0) return "Today";
        if (days == 1) return "Yesterday";
        if (days < 30) return days + " days ago";
        if (days < 60) return "Last month";
        return when.ToString("MMM yyyy");
    }

    // Fills the four cards and the bar chart from the database.
    private void LoadDashboard()
    {
        StudentsCard.Value = AppData.Students.Count().ToString("N0");
        TeachersCard.Value = AppData.Teachers.Count().ToString();

        var summary = AppData.Payments.GetSummary();
        RevenueCard.Value = "Rs. " + summary.Collected.ToString("N0");
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

        LoadRecentActivity();
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
