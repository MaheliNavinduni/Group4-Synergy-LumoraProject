using LumoraAcademy.Core.Entities;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// "Teacher" here means the database entity, not the Pages.Teacher folder.
using Teacher = LumoraAcademy.Core.Entities.Teacher;

public partial class TeacherDetailsPage : ContentPage
{
    private readonly int _teacherId;

    public TeacherDetailsPage(int teacherId)
    {
        InitializeComponent();
        _teacherId = teacherId;

        LoadTeacher();
        Loaded += (s, e) => LoadTeacher();   // refresh after editing
    }

    private void LoadTeacher()
    {
        Teacher? teacher = AppData.Teachers.GetById(_teacherId);
        if (teacher == null) return;

        CrumbLabel.Text = teacher.FullName;
        TitleLabel.Text = teacher.FullName;
        Avatar.Initials = teacher.Initials;
        Avatar.ImagePath = teacher.PhotoPath;
        NameLabel.Text = teacher.FullName;
        DesignationLabel.Text = teacher.Designation;
        IdLabel.Text = "ID: " + teacher.TeacherId;
        DobLabel.Text = teacher.DateOfBirth == default ? "-" : teacher.DateOfBirth.ToString("MMM dd, yyyy");
        GenderLabel.Text = string.IsNullOrWhiteSpace(teacher.Gender) ? "-" : teacher.Gender;
        JoinedLabel.Text = teacher.JoiningDate.ToString("MMM dd, yyyy");
        BloodLabel.Text = string.IsNullOrWhiteSpace(teacher.BloodGroup) ? "-" : teacher.BloodGroup;
        EmailLabel.Text = teacher.Email;
        PhoneLabel.Text = teacher.Phone;
        AddressLabel.Text = string.IsNullOrWhiteSpace(teacher.Address) ? "-" : teacher.Address;

        // Statistics from the database
        var students = AppData.Students.GetAll().Where(s => s.AssignedTeacherId == teacher.Id && s.Status == "Active").ToList();
        int subjectCount = teacher.Subjects.Split(',', StringSplitOptions.RemoveEmptyEntries).Length;
        SubjectsCard.Value = subjectCount.ToString();
        StudentsCard.Value = students.Count.ToString();
        ExperienceCard.Value = teacher.YearsOfExperience + " Yrs";

        // Pass rate = share of this teacher's students averaging 50% or more
        var averages = students.Select(s => AppData.Academics.AverageForStudent(s.Id)).Where(a => a > 0).ToList();
        PassCard.Value = averages.Count == 0 ? "-" : $"{100.0 * averages.Count(a => a >= 50) / averages.Count:0}%";

        // Today's lessons from the timetable (falls back to the whole week if nothing today)
        var today = AppData.Schedule.GetDayForTeacher(teacher.Id);
        BindableLayout.SetItemsSource(ScheduleList, today.Count > 0 ? today : AppData.Schedule.GetWeekForTeacher(teacher.Id));

        // Assigned classes: group this teacher's students by grade
        var classes = students
            .GroupBy(s => s.Grade)
            .Select(g => new Models.AssignedClass { Grade = g.Key, Subject = teacher.Subjects, Students = $"{g.Count()} Students" })
            .ToList();
        BindableLayout.SetItemsSource(ClassList, classes);
    }

    private async void OnTimetableClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new TimetablePage(_teacherId));
    }

    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new EditTeacherPage(_teacherId));
    }

    private async void OnResignationClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new TeacherResignationPage(_teacherId));
    }
}
