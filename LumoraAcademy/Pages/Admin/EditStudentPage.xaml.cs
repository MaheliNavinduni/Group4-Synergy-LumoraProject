using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Services;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// "Teacher" here means the database entity, not the Pages.Teacher folder.
using Teacher = LumoraAcademy.Core.Entities.Teacher;

// Epic 2, User Story 2 - update a student profile.
public partial class EditStudentPage : ContentPage
{
    private readonly Student _student;
    private readonly List<Teacher> _teachers;

    public EditStudentPage(int studentId)
    {
        InitializeComponent();

        _student = AppData.Students.GetById(studentId) ?? new Student();
        _teachers = AppData.Teachers.GetActive();

        GenderPicker.ItemsSource = new List<string> { "Female", "Male", "Other" };
        BloodGroupPicker.ItemsSource = new List<string> { "", "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
        GradePicker.ItemsSource = new List<string> { "6th Grade", "7th Grade", "8th Grade", "9th Grade", "10th Grade", "11th Grade", "12th Grade", "13th Grade" };
        StatusPicker.ItemsSource = new List<string> { "Active", "Pending", "Inactive", "Dropout" };

        var teacherNames = new List<string> { "(none)" };
        teacherNames.AddRange(_teachers.Select(t => t.FullName));
        TeacherPicker.ItemsSource = teacherNames;

        FillForm();
    }

    // Puts the student's current values into the boxes.
    private void FillForm()
    {
        SubtitleLabel.Text = $"Update information for {_student.FullName}.";

        FullNameEntry.Text = _student.FullName;
        StudentIdEntry.Text = _student.StudentId;
        DobPicker.Date = _student.DateOfBirth == default ? DateTime.Today : _student.DateOfBirth;
        GenderPicker.SelectedItem = _student.Gender;
        BloodGroupPicker.SelectedItem = _student.BloodGroup;
        GradePicker.SelectedItem = _student.Grade;
        SectionEntry.Text = _student.Section;
        StatusPicker.SelectedItem = _student.Status;
        ClassTimeEntry.Text = _student.ClassDayTime;

        int teacherIndex = _teachers.FindIndex(t => t.Id == _student.AssignedTeacherId);
        TeacherPicker.SelectedIndex = teacherIndex < 0 ? 0 : teacherIndex + 1;

        GuardianEntry.Text = _student.GuardianName;
        PhoneEntry.Text = _student.GuardianPhone;
        EmailEntry.Text = _student.GuardianEmail;
        AddressEntry.Text = _student.Address;
    }

    private async void OnPhotoClicked(object sender, EventArgs e)
    {
        string? path = await PhotoService.PickAndSaveAsync("student");
        if (path == null) return;

        AppData.Students.SetPhoto(_student.Id, path);
        _student.PhotoPath = path;
        await DisplayAlert("Photo", "Photo updated.", "OK");
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // The same rules as the registration form, so a record cannot be
        // edited into a state that would not have been accepted in the first place.
        string problem = Validation.FirstProblem(
            Validation.Name(FullNameEntry.Text, "Full name"),
            Validation.Required(StudentIdEntry.Text, "Student ID"),
            Validation.DateOfBirth(DobPicker.Date),
            GradePicker.SelectedIndex < 0 ? "Please select the grade." : "",
            Validation.Name(GuardianEntry.Text, "Parent / guardian name"),
            Validation.Phone(PhoneEntry.Text),
            Validation.Email(EmailEntry.Text, required: false),
            Validation.Address(AddressEntry.Text));

        if (problem != "")
        {
            await DisplayAlert("Edit Student", problem, "OK");
            return;
        }

        _student.FullName = (FullNameEntry.Text ?? "").Trim();
        _student.StudentId = (StudentIdEntry.Text ?? "").Trim().ToUpper();
        _student.DateOfBirth = DobPicker.Date;
        _student.Gender = GenderPicker.SelectedItem as string ?? "";
        _student.BloodGroup = BloodGroupPicker.SelectedItem as string ?? "";
        _student.Grade = GradePicker.SelectedItem as string ?? "";
        _student.Section = (SectionEntry.Text ?? "").Trim();
        _student.Status = StatusPicker.SelectedItem as string ?? "Active";
        _student.ClassDayTime = (ClassTimeEntry.Text ?? "").Trim();
        _student.AssignedTeacherId = TeacherPicker.SelectedIndex <= 0 ? null : _teachers[TeacherPicker.SelectedIndex - 1].Id;
        _student.GuardianName = (GuardianEntry.Text ?? "").Trim();
        _student.GuardianPhone = Validation.CleanPhone(PhoneEntry.Text);
        _student.GuardianEmail = (EmailEntry.Text ?? "").Trim();
        _student.Address = (AddressEntry.Text ?? "").Trim();

        try
        {
            AppData.Students.Update(_student);   // validates and saves
            await DisplayAlert("Edit Student", "Changes saved.", "OK");
            await AppNavigation.GoBackAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Edit Student", ex.Message, "OK");
        }
    }
}
