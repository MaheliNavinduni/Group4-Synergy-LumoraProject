using LumoraAcademy.Data;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class TeacherDetailsPage : ContentPage
{
    public TeacherDetailsPage()
    {
        InitializeComponent();

        BindableLayout.SetItemsSource(ScheduleList, SampleData.TeacherDetailSchedule);
        BindableLayout.SetItemsSource(ClassList, SampleData.AssignedClasses);
    }

    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new EditTeacherPage());
    }

    private async void OnResignationClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new TeacherResignationPage());
    }
}
