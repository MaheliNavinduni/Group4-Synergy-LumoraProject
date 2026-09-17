using LumoraAcademy.Data;

namespace LumoraAcademy.Pages.Teacher;

public partial class TeacherEventsPage : ContentPage
{
    public TeacherEventsPage()
    {
        InitializeComponent();

        BindableLayout.SetItemsSource(UpcomingList, SampleData.UpcomingThisWeek);
    }
}
