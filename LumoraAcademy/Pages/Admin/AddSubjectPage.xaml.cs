using LumoraAcademy.Core.Services;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class AddSubjectPage : ContentPage
{
    public AddSubjectPage()
    {
        InitializeComponent();

        CurriculumPicker.ItemsSource = new List<string> { "National Curriculum", "Cambridge Curriculum", "English", "Second Language Tamil", "Other" };
        CurriculumPicker.SelectedIndex = 0;

        LoadSubjects();
    }

    // Shows what is already set up, so the same subject is not added twice.
    private void LoadSubjects()
    {
        var subjects = AppData.Academics.GetSubjects();

        TotalCard.Value = subjects.Count.ToString();
        BindableLayout.SetItemsSource(SubjectList, subjects);
        NoSubjectsLabel.IsVisible = subjects.Count == 0;
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string problem = Validation.FirstProblem(
            Validation.Required(SubjectNameEntry.Text, "Subject name"),
            Validation.Required(SubjectCodeEntry.Text, "Subject code"),
            CurriculumPicker.SelectedIndex < 0 ? "Please select the curriculum." : "");

        if (problem != "")
        {
            await DisplayAlert("Add Subject", problem, "OK");
            return;
        }

        try
        {
            var subject = AppData.Academics.AddSubject(SubjectNameEntry.Text, SubjectCodeEntry.Text, CurriculumPicker.SelectedItem as string ?? "");
            await DisplayAlert("Add Subject", $"{subject.DisplayName} saved.", "OK");
            await AppNavigation.GoBackAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Add Subject", ex.Message, "OK");
        }
    }
}
