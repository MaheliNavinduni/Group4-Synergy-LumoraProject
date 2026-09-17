using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class EditTeacherPage : ContentPage
{
    public EditTeacherPage()
    {
        InitializeComponent();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FullNameEntry.Text))
        {
            await DisplayAlert("Edit Teacher", "The full name cannot be empty.", "OK");
            return;
        }

        // TODO: update the teacher record in the database.
        await DisplayAlert("Edit Teacher", "Changes saved (demo only, not stored yet).", "OK");
        await AppNavigation.GoBackAsync();
    }
}
