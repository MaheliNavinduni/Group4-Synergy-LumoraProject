using LumoraAcademy.Core.Entities;
using LumoraAcademy.Services;

namespace LumoraAcademy.Controls;

public partial class AttendanceTableView : ContentView
{
    public AttendanceTableView()
    {
        InitializeComponent();

        LoadRecords();
        Loaded += (s, e) => LoadRecords();
    }

    private void OnPageChanged(object sender, EventArgs e)
    {
        LoadRecords();
    }

    // One row per class per day, newest first.
    public void LoadRecords()
    {
        List<AttendanceSummary> records = AppData.Attendance.GetDailySummaries();
        BindableLayout.SetItemsSource(RowList, Pager.Page(records));
        CountLabel.Text = Pager.RangeText("records");

        if (records.Count > 0)
        {
            var oldest = records.Min(r => r.Date);
            var newest = records.Max(r => r.Date);
            RangeLabel.Text = $"{oldest:MMM d} - {newest:MMM d}";
        }
    }
}
