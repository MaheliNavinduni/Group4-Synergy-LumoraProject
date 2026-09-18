using LumoraAcademy.Data;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Shared;

public partial class AtRiskStudentsPage : ContentPage
{
    public AtRiskStudentsPage()
    {
        InitializeComponent();

        ClassPicker.ItemsSource = new List<string> { "All Classes", "Grade 9", "Grade 10", "Grade 11", "Grade 12" };
        ClassPicker.SelectedIndex = 0;

        SubjectPicker.ItemsSource = new List<string> { "All Subjects", "Mathematics", "Science", "ICT", "English", "Sinhala", "Tamil" };
        SubjectPicker.SelectedIndex = 0;

        ThresholdPicker.ItemsSource = new List<string> { "Below 60% (Failing)", "Below 70%", "Below 80%" };
        ThresholdPicker.SelectedIndex = 0;

        BindableLayout.SetItemsSource(RowList, SampleData.AtRiskStudents);
    }

    // Runs when "Apply Filters" is clicked.
    private void OnApplyFiltersClicked(object sender, EventArgs e)
    {
        string classFilter = ClassPicker.SelectedItem as string ?? "All Classes";
        string subjectFilter = SubjectPicker.SelectedItem as string ?? "All Subjects";
        string thresholdText = ThresholdPicker.SelectedItem as string ?? "Below 60% (Failing)";

        // "Below 60% (Failing)" -> 60
        int threshold = int.Parse(thresholdText.Replace("Below ", "").Substring(0, 2));

        var filtered = new List<Models.AtRiskStudent>();

        foreach (var student in SampleData.AtRiskStudents)
        {
            bool matchesClass = classFilter == "All Classes" || student.Cohort.StartsWith(classFilter);

            bool matchesSubject = subjectFilter == "All Subjects"
                || student.Subject1 == subjectFilter
                || student.Subject2 == subjectFilter;

            // "54%" -> 54
            int average = int.Parse(student.Average.Replace("%", ""));
            bool matchesThreshold = average < threshold;

            if (matchesClass && matchesSubject && matchesThreshold)
            {
                filtered.Add(student);
            }
        }

        BindableLayout.SetItemsSource(RowList, filtered);
        FlaggedTitle.Text = $"Flagged Students ({filtered.Count})";
        CountLabel.Text = $"Showing {filtered.Count} of {SampleData.AtRiskStudents.Count} students";
    }

    private async void OnViewDetailsClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new StudentDetailsPage());
    }
}
