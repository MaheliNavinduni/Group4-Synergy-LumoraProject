using LumoraAcademy.Core.Services;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Shared;

// Epic 4, User Story 3 - identify weak-performing students.
public partial class AtRiskStudentsPage : ContentPage
{
    public AtRiskStudentsPage()
    {
        InitializeComponent();

        ClassPicker.ItemsSource = new List<string> { "All Classes", "9th Grade", "10th Grade", "11th Grade", "12th Grade" };
        ClassPicker.SelectedIndex = 0;

        var subjects = new List<string> { "All Subjects" };
        subjects.AddRange(AppData.Academics.GetSubjects().Select(s => s.Name).Distinct());
        SubjectPicker.ItemsSource = subjects;
        SubjectPicker.SelectedIndex = 0;

        ThresholdPicker.ItemsSource = new List<string> { "Below 60% (Failing)", "Below 70%", "Below 80%" };
        ThresholdPicker.SelectedIndex = 0;

        LoadStudents();
        Loaded += (s, e) => LoadStudents();
    }

    // Asks the backend for students below the chosen threshold.
    private void LoadStudents()
    {
        string thresholdText = ThresholdPicker.SelectedItem as string ?? "Below 60% (Failing)";
        double threshold = double.Parse(thresholdText.Replace("Below ", "").Substring(0, 2));

        List<AtRiskStudent> students = AppData.Academics.GetAtRiskStudents(
            threshold,
            ClassPicker.SelectedItem as string,
            SubjectPicker.SelectedItem as string);

        BindableLayout.SetItemsSource(RowList, Pager.Page(students));
        FlaggedTitle.Text = $"Flagged Students ({students.Count})";
        CountLabel.Text = Pager.RangeText("students");
    }

    private void OnApplyFiltersClicked(object sender, EventArgs e)
    {
        Pager.Reset();
        LoadStudents();
    }

    private void OnPageChanged(object sender, EventArgs e)
    {
        LoadStudents();
    }

    private async void OnViewDetailsClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is AtRiskStudent risk)
        {
            await AppNavigation.GoToAsync(new StudentDetailsPage(risk.Student.Id));
        }
    }
}
