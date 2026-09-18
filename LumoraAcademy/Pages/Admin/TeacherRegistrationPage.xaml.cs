using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class TeacherRegistrationPage : ContentPage
{
    public TeacherRegistrationPage()
    {
        InitializeComponent();

        RolePicker.ItemsSource = new List<string> { "Teacher", "Senior Teacher", "Head of Department", "Visiting Lecturer" };
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FullNameEntry.Text))
        {
            await DisplayAlert("Teacher Registration", "Please enter the teacher's full name.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(UsernameEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Teacher Registration", "Please enter a username and password for the teacher's login.", "OK");
            return;
        }

        // TODO: save the new teacher and login details to the database (hash the password).
        await DisplayAlert("Teacher Registration", "Teacher registered (demo only, not stored yet).", "OK");
        await AppNavigation.GoBackAsync();
    }
}
