using LumoraAcademy.Data;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class AdminTeachersPage : ContentPage
{
    public AdminTeachersPage()
    {
        InitializeComponent();

        BindableLayout.SetItemsSource(RowList, SampleData.Teachers);
    }

    private async void OnRegisterTeacherClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new TeacherRegistrationPage());
    }

    private async void OnTeacherRowTapped(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new TeacherDetailsPage());
    }
}
