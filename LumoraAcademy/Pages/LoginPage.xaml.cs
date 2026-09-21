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

        // Both fields are mandatory.
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

        // Check the username and password against the Users table.
        // Demo accounts: admin / admin123 and teacher / teacher123.
        var user = AppData.Auth.Login(username, password);

        if (user == null)
        {
            await DisplayAlert("Login", "Incorrect username or password.", "OK");
            return;
        }

        AppNavigation.CurrentRole = user.Role;
        AppNavigation.CurrentUserName = user.DisplayName;
        AppData.CurrentTeacherId = user.TeacherId;

        PasswordEntry.Text = "";

        if (user.Role == "Admin")
        {
            await AppNavigation.GoToAsync(new Admin.AdminDashboardPage());
        }
        else
        {
            await AppNavigation.GoToAsync(new Teacher.TeacherDashboardPage());
        }
    }
}
