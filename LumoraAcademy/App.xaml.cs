using LumoraAcademy.Pages;

namespace LumoraAcademy;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // The design uses light colours only, so we keep the app in light mode.
        UserAppTheme = AppTheme.Light;

        // The app starts on the Home page. The navigation bar is hidden
        // because our pages draw their own headers.
        var homePage = new HomePage();
        NavigationPage.SetHasNavigationBar(homePage, false);

        MainPage = new NavigationPage(homePage);
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);

        // A comfortable default size for the desktop window.
        window.Title = "Lumora Academy";
        window.Width = 1100;
        window.Height = 760;

        return window;
    }
}
