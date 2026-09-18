using LumoraAcademy.Data;
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

        BindableLayout.SetItemsSource(RowList, SampleData.Students);
    }

    // Runs when "Apply Filters" is clicked.
    // Keeps only the students that match the search text, grade and status boxes.
    private void OnApplyFiltersClicked(object sender, EventArgs e)
    {
        string search = (SearchEntry.Text ?? "").Trim().ToLower();
        string grade = GradePicker.SelectedItem as string ?? "All Grades";

        // Which statuses are ticked
        var allowedStatuses = new List<string>();
        if (ActiveCheck.IsChecked) allowedStatuses.Add("Active");
        if (PendingCheck.IsChecked) allowedStatuses.Add("Pending");
        if (InactiveCheck.IsChecked) allowedStatuses.Add("Inactive");

        var filtered = new List<Models.Student>();

        foreach (var student in SampleData.Students)
        {
            bool matchesSearch = search == ""
                || student.Name.ToLower().Contains(search)
                || student.Id.ToLower().Contains(search)
                || student.Email.ToLower().Contains(search);

            bool matchesGrade = grade == "All Grades" || student.Grade == grade;

            bool matchesStatus = allowedStatuses.Contains(student.Status);

            if (matchesSearch && matchesGrade && matchesStatus)
            {
                filtered.Add(student);
            }
        }

        BindableLayout.SetItemsSource(RowList, filtered);
        CountLabel.Text = $"Showing {filtered.Count} of {SampleData.Students.Count} students";
    }

    private async void OnStudentRowTapped(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new Pages.Shared.StudentDetailsPage());
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
