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

    public StudentDirectoryView()
    {
        InitializeComponent();

        GradePicker.ItemsSource = new List<string> { "All Grades", "9th Grade", "10th Grade", "11th Grade", "12th Grade" };
        GradePicker.SelectedIndex = 0;

        LoadStudents();

        // Reload when we come back to this page (e.g. after registering a student).
        Loaded += (s, e) => LoadStudents();
    }

    // Loads the students from the database using the current filter settings.
    private void LoadStudents()
    {
        string search = SearchEntry.Text ?? "";
        string grade = GradePicker.SelectedItem as string ?? "All Grades";

        var statuses = new List<string>();
        if (ActiveCheck.IsChecked) statuses.Add("Active");
        if (PendingCheck.IsChecked) statuses.Add("Pending");
        if (InactiveCheck.IsChecked) { statuses.Add("Inactive"); statuses.Add("Dropout"); }

        List<Student> students = AppData.Students.Search(search, grade, statuses);

        // Only the rows for the current page are shown
        BindableLayout.SetItemsSource(RowList, Pager.Page(students));
        CountLabel.Text = Pager.RangeText("students");
        TotalLabel.Text = AppData.Students.Count().ToString("N0");
    }

    private void OnApplyFiltersClicked(object sender, EventArgs e)
    {
        Pager.Reset();   // a new search starts from page 1
        LoadStudents();
    }

    private void OnPageChanged(object sender, EventArgs e)
    {
        LoadStudents();
    }

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
