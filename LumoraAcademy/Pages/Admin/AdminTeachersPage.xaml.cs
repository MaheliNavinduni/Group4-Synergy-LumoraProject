using LumoraAcademy.Core.Entities;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// "Teacher" here means the database entity, not the Pages.Teacher folder.
using Teacher = LumoraAcademy.Core.Entities.Teacher;

// Epic 1, User Story 2 - teacher account management.
public partial class AdminTeachersPage : ContentPage
{
    public AdminTeachersPage()
    {
        InitializeComponent();

        LoadTeachers();
        Loaded += (s, e) => LoadTeachers();
    }

    private void LoadTeachers()
    {
        List<Teacher> teachers = AppData.Teachers.GetAll();
        BindableLayout.SetItemsSource(RowList, Pager.Page(teachers));
        CountLabel.Text = Pager.RangeText("teachers");

        TotalCard.Value = AppData.Teachers.Count().ToString();
        LeaveCard.Value = AppData.Teachers.CountOnLeave().ToString();

        // Two biggest departments
        var departments = AppData.Teachers.DepartmentPercentages().OrderByDescending(d => d.Value).ToList();
        Dept1Name.Text = departments.Count > 0 ? departments[0].Key : "";
        Dept1Value.Text = departments.Count > 0 ? departments[0].Value + "%" : "";
        Dept2Name.Text = departments.Count > 1 ? departments[1].Key : "";
        Dept2Value.Text = departments.Count > 1 ? departments[1].Value + "%" : "";
    }

    private void OnPageChanged(object sender, EventArgs e)
    {
        LoadTeachers();
    }

    private async void OnRegisterTeacherClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new TeacherRegistrationPage());
    }

    private async void OnTeacherRowTapped(object sender, EventArgs e)
    {
        if (sender is Grid row && row.BindingContext is Teacher teacher)
        {
            await AppNavigation.GoToAsync(new TeacherDetailsPage(teacher.Id));
        }
    }
}
