using LumoraAcademy.Core.Entities;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// "Teacher" here means the database entity, not the Pages.Teacher folder.
using Teacher = LumoraAcademy.Core.Entities.Teacher;

// Epic 1, User Story 2 - teacher account management.
public partial class AdminTeachersPage : ContentPage
{
    // The teachers the search and filter match. Kept so Export saves what is on screen.
    private List<Teacher> _matches = new();

    public AdminTeachersPage()
    {
        InitializeComponent();

        StatusPicker.ItemsSource = new List<string> { "All statuses", "Active", "On Leave", "Resigned" };
        StatusPicker.SelectedIndex = 0;

        LoadTeachers();
        Loaded += (s, e) => LoadTeachers();
    }

    private void LoadTeachers()
    {
        _matches = AppData.Teachers.Search(SearchEntry.Text ?? "");

        string status = StatusPicker.SelectedItem ?? "All statuses";
        if (status != "All statuses")
        {
            _matches = _matches.Where(t => t.Status == status).ToList();
        }

        BindableLayout.SetItemsSource(RowList, Pager.Page(_matches));

        bool anyFound = _matches.Count > 0;
        EmptyLabel.IsVisible = !anyFound;
        CountLabel.Text = anyFound ? Pager.RangeText("teachers") : "";

        TotalCard.Value = AppData.Teachers.Count().ToString();
        LeaveCard.Value = AppData.Teachers.CountOnLeave().ToString();

        // Two biggest departments
        var departments = AppData.Teachers.DepartmentPercentages().OrderByDescending(d => d.Value).ToList();
        Dept1Name.Text = departments.Count > 0 ? departments[0].Key : "";
        Dept1Value.Text = departments.Count > 0 ? departments[0].Value + "%" : "";
        Dept2Name.Text = departments.Count > 1 ? departments[1].Key : "";
        Dept2Value.Text = departments.Count > 1 ? departments[1].Value + "%" : "";
    }

    // ---------- Search and filter ----------

    // The list updates as the user types.
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        Pager.Reset();
        LoadTeachers();
    }

    private void OnFilterChanged(object sender, EventArgs e)
    {
        Pager.Reset();
        LoadTeachers();
    }

    private void OnPageChanged(object sender, EventArgs e)
    {
        LoadTeachers();
    }

    private async void OnExportClicked(object sender, EventArgs e)
    {
        if (_matches.Count == 0)
        {
            await DisplayAlert("Teachers", "There is nothing to export.", "OK");
            return;
        }

        try
        {
            var headings = new[] { "Teacher ID", "Full Name", "Designation", "Department", "Subjects", "Email", "Phone", "Status" };

            var rows = _matches.Select(t => new[]
            {
                t.TeacherId, t.FullName, t.Designation, t.Department, t.Subjects, t.Email, t.Phone, t.Status
            });

            await ExportService.SaveAndOpenCsvAsync($"teachers-{DateTime.Today:yyyy-MM-dd}.csv", headings, rows);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Teachers", "Could not export the list: " + ex.Message, "OK");
        }
    }

    // ---------- Rows ----------

    private async void OnRegisterTeacherClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new TeacherRegistrationPage());
    }

    private async void OnTeacherRowTapped(object sender, EventArgs e)
    {
        if (sender is Grid row && row.BindingContext is Teacher teacher)
        {
            await AppNavigation.GoToAsync(new TeacherDetailsPage(teacher.Id));
        }
    }

}
