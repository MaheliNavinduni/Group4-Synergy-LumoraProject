using LumoraAcademy.Data;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class AdminAcademicsPage : ContentPage
{
    public AdminAcademicsPage()
    {
        InitializeComponent();

        BindableLayout.SetItemsSource(SubjectList, SampleData.Subjects);
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
