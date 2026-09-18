using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class TeacherResignationPage : ContentPage
{
    public TeacherResignationPage()
    {
        InitializeComponent();

        ReasonPicker.ItemsSource = new List<string> { "Personal reasons", "New position elsewhere", "Relocation", "Retirement", "Health", "Other" };
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnProcessClicked(object sender, EventArgs e)
    {
        if (ReasonPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Teacher Resignation", "Please select a reason for leaving.", "OK");
            return;
        }

        bool allDone = KeysCheck.IsChecked && GradesCheck.IsChecked && AssetsCheck.IsChecked && PayCheck.IsChecked;
        if (!allDone)
        {
            await DisplayAlert("Teacher Resignation", "Please complete every item on the handover checklist first.", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Teacher Resignation", "Process this resignation? This cannot be undone.", "Process", "Cancel");
        if (confirm)
        {
            // TODO: mark the teacher as resigned in the database.
            await DisplayAlert("Teacher Resignation", "Resignation processed (demo only).", "OK");
            await AppNavigation.GoBackAsync();
        }
    }
}
