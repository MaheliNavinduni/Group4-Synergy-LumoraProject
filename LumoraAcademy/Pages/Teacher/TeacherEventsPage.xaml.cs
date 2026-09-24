using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Teacher;

public partial class TeacherEventsPage : ContentPage
{
    public TeacherEventsPage()
    {
        InitializeComponent();

        // Teachers see staff events only (not "Admin Only" ones).
        BindableLayout.SetItemsSource(UpcomingList, AppData.Events.GetUpcoming(5, includeAdminOnly: false));
    }
}
