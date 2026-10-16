namespace LumoraAcademy.Pages;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    // Runs when the user clicks the "Login" button on the hero section.
    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        // On a brand new system there is no account to sign in with yet, so the
        // first thing the user sees is the page that creates the administrator.
        Page next = FirstRunSetupPage.IsNeeded()
            ? new FirstRunSetupPage()
            : new LoginPage();

        NavigationPage.SetHasNavigationBar(next, false);

        await Navigation.PushAsync(next);
    }
}
