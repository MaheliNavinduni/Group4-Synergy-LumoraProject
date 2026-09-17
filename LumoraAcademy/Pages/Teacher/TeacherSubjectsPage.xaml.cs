using LumoraAcademy.Data;

namespace LumoraAcademy.Pages.Teacher;

public partial class TeacherSubjectsPage : ContentPage
{
    public TeacherSubjectsPage()
    {
        InitializeComponent();

        BindableLayout.SetItemsSource(SubjectList, SampleData.TeacherSubjects);
    }
}
