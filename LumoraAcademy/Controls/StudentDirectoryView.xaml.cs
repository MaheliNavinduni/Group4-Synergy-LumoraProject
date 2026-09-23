using LumoraAcademy.Core.Entities;
using LumoraAcademy.Services;

namespace LumoraAcademy.Controls;

public partial class StudentDirectoryView : ContentView
{
    // Admin pages set ShowRegisterButton="True" to show the "Register New Student" button.
    public static readonly BindableProperty ShowRegisterButtonProperty =
        BindableProperty.Create(nameof(ShowRegisterButton), typeof(bool), typeof(StudentDirectoryView), false,
            propertyChanged: (b, o, n) => ((StudentDirectoryView)b).RegisterButton.IsVisible = (bool)n);

    public bool ShowRegisterButton
    {
        get => (bool)GetValue(ShowRegisterButtonProperty);
        set => SetValue(ShowRegisterButtonProperty, value);
    }

    // The students the current filters match. Kept so Export saves what is on screen.
    private List<Student> _matches = new();

    // True while the filters are being reset, so the list only reloads once at the end.
    private bool _resetting;

    public StudentDirectoryView()
    {
        InitializeComponent();

        GradePicker.ItemsSource = new List<string> { "All Grades", "6th Grade", "7th Grade", "8th Grade", "9th Grade", "10th Grade", "11th Grade", "12th Grade", "13th Grade" };
        GradePicker.SelectedIndex = 0;

        LoadStudents();

        // Reload when we come back to this page (e.g. after registering a student).
        Loaded += (s, e) => LoadStudents();
    }

    // ---------- Loading ----------

    // Loads the students from the database using the current filter settings.
    private void LoadStudents()
    {
        if (_resetting) return;

        string search = (SearchEntry.Text ?? "").Trim();
        string grade = GradePicker.SelectedItem ?? "All Grades";

        _matches = AppData.Students.Search(search, grade, ChosenStatuses());

        // Only the rows for the current page are shown
        BindableLayout.SetItemsSource(RowList, Pager.Page(_matches));

        bool anyFound = _matches.Count > 0;
        EmptyPanel.IsVisible = !anyFound;
        CountLabel.Text = anyFound ? Pager.RangeText("students") : "";

        ClearSearchIcon.IsVisible = search.Length > 0;
        FilterSummaryLabel.Text = DescribeFilters(search, grade);

        TotalLabel.Text = AppData.Students.Count().ToString("N0");

        int active = AppData.Students.Search("", "All Grades", new List<string> { "Active" }).Count;
        ActiveCountLabel.Text = active + " active right now";
    }

    // Turns the chosen radio button into the list of statuses the backend expects.
    // An empty list means "do not filter by status".
    private List<string> ChosenStatuses()
    {
        if (ActiveRadio.IsChecked) return new List<string> { "Active" };
        if (PendingRadio.IsChecked) return new List<string> { "Pending" };
        if (InactiveRadio.IsChecked) return new List<string> { "Inactive", "Dropout" };
        return new List<string>();
    }

    // A short line under the table heading, so the user can see what is being shown.
    private string DescribeFilters(string search, string grade)
    {
        var parts = new List<string>();

        if (search.Length > 0) parts.Add("matching \"" + search + "\"");
        if (grade != "All Grades") parts.Add("in " + grade);

        if (ActiveRadio.IsChecked) parts.Add("Active only");
        else if (PendingRadio.IsChecked) parts.Add("Pending only");
        else if (InactiveRadio.IsChecked) parts.Add("Inactive or Dropout only");

        return parts.Count == 0 ? "All students" : string.Join(" - ", parts);
    }

    // ---------- Filters ----------

    // The list updates as the user types - there is no need to press a button.
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        Pager.Reset();
        LoadStudents();
    }

    private void OnClearSearchTapped(object sender, EventArgs e)
    {
        SearchEntry.Text = "";
    }

    private void OnFilterChanged(object sender, EventArgs e)
    {
        Pager.Reset();
        LoadStudents();
    }

    // This runs twice each time the user picks a status (one button turns off and
    // another turns on), so the list is only reloaded for the one that turned on.
    private void OnStatusChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value) return;

        Pager.Reset();
        LoadStudents();
    }

    private void OnClearFiltersClicked(object sender, EventArgs e)
    {
        _resetting = true;
        SearchEntry.Text = "";
        GradePicker.SelectedIndex = 0;
        AllStatusRadio.IsChecked = true;
        _resetting = false;

        Pager.Reset();
        LoadStudents();
    }

    private void OnRefreshClicked(object sender, EventArgs e)
    {
        LoadStudents();
    }

    private void OnPageChanged(object sender, EventArgs e)
    {
        LoadStudents();
    }

    // ---------- Export ----------

    // Saves everything the filters match (not just this page) as a CSV file.
    private async void OnExportClicked(object sender, EventArgs e)
    {
        if (_matches.Count == 0)
        {
            await ShowMessage("There is nothing to export. Change the filters and try again.");
            return;
        }

        try
        {
            var headings = new[] { "Student ID", "Full Name", "Grade", "Assigned Teacher", "Guardian", "Phone", "Email", "Status" };

            var rows = _matches.Select(s => new[]
            {
                s.StudentId,
                s.FullName,
                s.Grade,
                s.AssignedTeacherName,
                s.GuardianName,
                s.GuardianPhone,
                s.Email,
                s.Status
            });

            await ExportService.SaveAndOpenCsvAsync($"students-{DateTime.Today:yyyy-MM-dd}.csv", headings, rows);
        }
        catch (Exception ex)
        {
            await ShowMessage("Could not export the list: " + ex.Message);
        }
    }

    private static Task ShowMessage(string message)
    {
        var page = Application.Current?.MainPage;
        return page == null ? Task.CompletedTask : page.DisplayAlert("Student Directory", message, "OK");
    }

    // ---------- Rows ----------

    // Opens the details page for the student whose row was clicked.
    private async void OnStudentRowTapped(object sender, EventArgs e)
    {
        if (sender is Grid row && row.BindingContext is Student student)
        {
            await AppNavigation.GoToAsync(new Pages.Shared.StudentDetailsPage(student.Id));
        }
    }


    private async void OnAtRiskClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new Pages.Shared.AtRiskStudentsPage());
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new Pages.Admin.StudentRegistrationPage());
    }
}
