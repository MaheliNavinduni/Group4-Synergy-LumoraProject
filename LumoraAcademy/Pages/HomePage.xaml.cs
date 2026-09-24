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
        var loginPage = new LoginPage();
        NavigationPage.SetHasNavigationBar(loginPage, false);

        await Navigation.PushAsync(loginPage);
    }
}
