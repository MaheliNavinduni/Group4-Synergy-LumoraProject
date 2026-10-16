using LumoraAcademy.Core.Services;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages;

// Shown the first time a production build is opened, when the database is
// empty and there is no account to sign in with. The person setting the
// system up creates the administrator account here.
public partial class FirstRunSetupPage : ContentPage
{
    public FirstRunSetupPage()
    {
        InitializeComponent();
    }

    // True when nobody has an account yet, so the setup page is needed.
    public static bool IsNeeded()
    {
        try
        {
            return AppData.Auth.GetAll().Count == 0;
        }
        catch
        {
            // If the database cannot be read we let the normal login page
            // report the problem rather than hiding it behind setup.
            return false;
        }
    }

    private void OnTypingChanged(object sender, TextChangedEventArgs e)
    {
        ErrorBox.IsVisible = false;
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        string name = (NameEntry.Text ?? "").Trim();
        string username = (UsernameEntry.Text ?? "").Trim();
        string password = PasswordEntry.Text ?? "";
        string confirm = ConfirmEntry.Text ?? "";

        string problem = Validation.FirstProblem(
            Validation.Required(name, "Your name"),
            Validation.Required(username, "Username"),
            username.Length is > 0 and < 3 ? "The username must be at least 3 characters." : "",
            Validation.Required(password, "Password"),
            password.Length is > 0 and < 6 ? "The password must be at least 6 characters." : "",
            password != confirm ? "The two passwords do not match." : "");

        if (ShowProblem(problem)) return;

        CreateButton.IsEnabled = false;
        CreateButton.Text = "Creating...";

        try
        {
            AppData.Auth.CreateUser(username, password, "Admin", name);

            await DisplayAlert("Account created",
                $"The administrator account '{username}' has been created. Please sign in to continue.",
                "Sign in");

            // Replace this page with the login page, so Back cannot return to setup.
            await AppNavigation.GoToAsync(new LoginPage());
        }
        catch (Exception ex)
        {
            ShowProblem(ex.Message);
        }
        finally
        {
            CreateButton.IsEnabled = true;
            CreateButton.Text = "Create administrator account";
        }
    }

    private bool ShowProblem(string message)
    {
        bool hasProblem = !string.IsNullOrEmpty(message);

        ErrorLabel.Text = message;
        ErrorBox.IsVisible = hasProblem;

        return hasProblem;
    }
}
