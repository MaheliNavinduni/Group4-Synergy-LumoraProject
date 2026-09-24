using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class AddSubjectPage : ContentPage
{
    public AddSubjectPage()
    {
        InitializeComponent();

        CurriculumPicker.ItemsSource = new List<string> { "National Curriculum", "Cambridge Curriculum", "English", "Second Language Tamil", "Other" };
        CurriculumPicker.SelectedIndex = 0;

        TotalCard.Value = AppData.Academics.GetSubjects().Count.ToString();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SubjectNameEntry.Text) || string.IsNullOrWhiteSpace(SubjectCodeEntry.Text))
        {
            await DisplayAlert("Add Subject", "Please enter both the subject name and code.", "OK");
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
