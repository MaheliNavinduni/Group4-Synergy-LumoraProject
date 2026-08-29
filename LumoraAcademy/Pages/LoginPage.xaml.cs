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
        string username = UsernameEntry.Text ?? string.Empty;
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

        // TODO: check the username and password against the database,
        // then open the Admin or Teacher dashboard.
        await DisplayAlert("Login", "Login details accepted. The dashboards are not built yet.", "OK");
    }
}
