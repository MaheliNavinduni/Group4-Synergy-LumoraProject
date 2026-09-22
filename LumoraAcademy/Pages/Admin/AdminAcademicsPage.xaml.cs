using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// Subject management (supports Epic 4 - marks are recorded per subject).
public partial class AdminAcademicsPage : ContentPage
{
    public AdminAcademicsPage()
    {
        InitializeComponent();

        LoadSubjects();
        Loaded += (s, e) => LoadSubjects();
    }

    private void LoadSubjects()
    {
        BindableLayout.SetItemsSource(SubjectList, AppData.Academics.GetSubjects());
    }

    private async void OnAddSubjectClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new AddSubjectPage());
    }

    private async void OnGoBackClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToMenuItemAsync("Dashboard");
    }
}
