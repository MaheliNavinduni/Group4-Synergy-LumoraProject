using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class StudentDeparturePage : ContentPage
{
    public StudentDeparturePage()
    {
        InitializeComponent();

        ReasonPicker.ItemsSource = new List<string> { "Relocation", "Transfer to another school", "Financial reasons", "Completed studies", "Other" };
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        if (ReasonPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Student Departure", "Please select a reason.", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Student Departure", "Confirm this student's departure? This cannot be undone.", "Confirm", "Cancel");
        if (confirm)
        {
            // TODO: mark the student as departed in the database.
            await DisplayAlert("Student Departure", "Departure recorded (demo only).", "OK");
            await AppNavigation.GoBackAsync();
        }
    }
}
