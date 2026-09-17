using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class AddSubjectPage : ContentPage
{
    public AddSubjectPage()
    {
        InitializeComponent();
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

        // TODO: save the subject to the database.
        await DisplayAlert("Add Subject", "Subject saved (demo only, not stored yet).", "OK");
        await AppNavigation.GoBackAsync();
    }
}
