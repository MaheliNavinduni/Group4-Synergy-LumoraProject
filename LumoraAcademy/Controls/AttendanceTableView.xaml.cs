using LumoraAcademy.Core.Entities;
using LumoraAcademy.Services;

namespace LumoraAcademy.Controls;

public partial class AttendanceTableView : ContentView
{
    // The classes behind the "All classes" drop-down. Position 0 is "All classes",
    // so the class for row N in the list is _classes[N - 1].
    private List<ClassGroup> _classes = new();

    // The months behind the "All dates" drop-down, as "2026-09".
    private List<string> _months = new();

    private bool _resetting;

    public AttendanceTableView()
    {
        InitializeComponent();

        FillFilters();
        LoadRecords();

        Loaded += (s, e) => LoadRecords();
    }

    // ---------- Filters ----------

    // Fills the two drop-downs from what is actually in the database.
    private void FillFilters()
    {
        _resetting = true;

        _classes = AppData.Classes.GetAll(activeOnly: false);

        var classNames = new List<string> { "All classes" };
        classNames.AddRange(_classes.Select(c => c.Name));
        ClassPicker.ItemsSource = classNames;
        ClassPicker.SelectedIndex = 0;

        _months = AppData.Attendance.GetDailySummaries()
            .Select(s => s.Date.ToString("yyyy-MM"))
            .Distinct()
            .OrderByDescending(m => m)
            .ToList();

        var monthNames = new List<string> { "All dates" };
        monthNames.AddRange(_months.Select(m => DateTime.Parse(m + "-01").ToString("MMMM yyyy")));
        MonthPicker.ItemsSource = monthNames;
        MonthPicker.SelectedIndex = 0;

        _resetting = false;
    }

    private void OnFilterChanged(object sender, EventArgs e)
    {
        if (_resetting) return;

        Pager.Reset();
        LoadRecords();
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        _resetting = true;
        ClassPicker.SelectedIndex = 0;
        MonthPicker.SelectedIndex = 0;
        _resetting = false;

        Pager.Reset();
        LoadRecords();
    }

    private void OnPageChanged(object sender, EventArgs e)
    {
        LoadRecords();
    }

    // ---------- Loading ----------

    // One row per class per day, newest first.
    public void LoadRecords()
    {
        // "All classes" is position 0, so anything above that is a real class.
        int? classGroupId = ClassPicker.SelectedIndex > 0
            ? _classes[ClassPicker.SelectedIndex - 1].Id
            : null;

        DateTime? from = null;
        DateTime? to = null;

        if (MonthPicker.SelectedIndex > 0)
        {
            var firstOfMonth = DateTime.Parse(_months[MonthPicker.SelectedIndex - 1] + "-01");
            from = firstOfMonth;
            to = firstOfMonth.AddMonths(1).AddDays(-1);
        }

        List<AttendanceSummary> records = AppData.Attendance.GetDailySummaries(from, to, classGroupId);

        BindableLayout.SetItemsSource(RowList, Pager.Page(records));

        bool anyFound = records.Count > 0;
        CountLabel.Text = anyFound ? Pager.RangeText("records") : "No attendance has been marked for this selection yet.";

        RangeLabel.Text = anyFound
            ? $"{records.Min(r => r.Date):MMM d} - {records.Max(r => r.Date):MMM d}"
            : "";
    }
}
