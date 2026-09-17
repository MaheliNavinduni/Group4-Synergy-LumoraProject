using LumoraAcademy.Services;

namespace LumoraAcademy.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    // Runs when the user clicks the "Login" button.
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = (UsernameEntry.Text ?? string.Empty).Trim();
        string password = PasswordEntry.Text ?? string.Empty;

        // Simple checks before we do anything else.
        if (string.IsNullOrWhiteSpace(username))
        {
            await DisplayAlert("Login", "Please enter your username.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Login", "Please enter your password.", "OK");
            return;
        }

        // TEMPORARY demo accounts so the screens can be tested without a database.
        // TODO: replace this with a real check against the database (with hashed passwords).
        if (username == "admin" && password == "admin123")
        {
            AppNavigation.CurrentRole = "Admin";
            AppNavigation.CurrentUserName = "Admin";
            await AppNavigation.GoToAsync(new Admin.AdminDashboardPage());
            return;
        }

        if (username == "teacher" && password == "teacher123")
        {
            AppNavigation.CurrentRole = "Teacher";
            AppNavigation.CurrentUserName = "Miss. Aries";
            await AppNavigation.GoToAsync(new Teacher.TeacherDashboardPage());
            return;
        }

        await DisplayAlert("Login", "Incorrect username or password.", "OK");
    }
}
