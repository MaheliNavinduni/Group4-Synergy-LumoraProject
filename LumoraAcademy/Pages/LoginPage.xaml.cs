using LumoraAcademy.Services;

namespace LumoraAcademy.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    // Pressing Enter in the username box moves down to the password box.
    private void OnUsernameCompleted(object sender, EventArgs e)
    {
        PasswordEntry.Focus();
    }

    // Hides the old error message as soon as the user starts correcting it.
    private void OnTypingChanged(object sender, TextChangedEventArgs e)
    {
        ErrorBox.IsVisible = false;
    }

    // Lets the user check what they typed if the login keeps failing.
    private void OnTogglePasswordTapped(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        ShowPasswordIcon.Text = PasswordEntry.IsPassword ? "" : "";
    }

    // Runs when the user clicks "Login", or presses Enter in the password box.
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = (UsernameEntry.Text ?? string.Empty).Trim();
        string password = PasswordEntry.Text ?? string.Empty;

        // Both fields are mandatory.
        if (string.IsNullOrWhiteSpace(username))
        {
            ShowError("Please enter your username.");
            UsernameEntry.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ShowError("Please enter your password.");
            PasswordEntry.Focus();
            return;
        }

        // The button is turned off while we check, so it cannot be clicked twice.
        SubmitButton.IsEnabled = false;
        SubmitButton.Text = "Signing in...";

        try
        {
            // Check the username and password against the Users table.
            // Demo accounts: admin / admin123 and teacher / teacher123.
            var user = AppData.Auth.Login(username, password);

            if (user == null)
            {
                ShowError("Incorrect username or password.");
                PasswordEntry.Text = "";
                PasswordEntry.Focus();
                return;
            }

            AppNavigation.CurrentRole = user.Role;
            AppNavigation.CurrentUserName = user.DisplayName;
            AppData.CurrentTeacherId = user.TeacherId;

            PasswordEntry.Text = "";
            ErrorBox.IsVisible = false;

            if (user.Role == "Admin")
            {
                await AppNavigation.GoToAsync(new Admin.AdminDashboardPage());
            }
            else
            {
                await AppNavigation.GoToAsync(new Teacher.TeacherDashboardPage());
            }
        }
        finally
        {
            SubmitButton.IsEnabled = true;
            SubmitButton.Text = "Login";
        }
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorBox.IsVisible = true;
    }
}
