using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Services;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// "Teacher" here means the database entity, not the Pages.Teacher folder.
using Teacher = LumoraAcademy.Core.Entities.Teacher;

// Epic 1, User Story 2 - edit teacher account information.
public partial class EditTeacherPage : ContentPage
{
    private readonly Teacher _teacher;

    public EditTeacherPage(int teacherId)
    {
        InitializeComponent();

        _teacher = AppData.Teachers.GetById(teacherId) ?? new Teacher();

        SubtitleLabel.Text = $"Update information for {_teacher.FullName}.";
        FullNameEntry.Text = _teacher.FullName;
        TeacherIdEntry.Text = _teacher.TeacherId;
        ExperienceEntry.Text = _teacher.YearsOfExperience.ToString();
        JoiningDatePicker.Date = _teacher.JoiningDate == default ? DateTime.Today : _teacher.JoiningDate;
        EmailEntry.Text = _teacher.Email;
        PhoneEntry.Text = _teacher.Phone;
        AddressEditor.Text = _teacher.Address;
    }

    private async void OnPhotoClicked(object sender, EventArgs e)
    {
        string? path = await PhotoService.PickAndSaveAsync("teacher");
        if (path == null) return;

        AppData.Teachers.SetPhoto(_teacher.Id, path);
        _teacher.PhotoPath = path;
        await DisplayAlert("Photo", "Photo updated.", "OK");
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string problem = Validation.FirstProblem(
            Validation.Name(FullNameEntry.Text, "Full name"),
            Validation.Required(TeacherIdEntry.Text, "Teacher ID"),
            Validation.WholeNumber(ExperienceEntry.Text, "Years of experience", 0, 60),
            Validation.NotInFuture(JoiningDatePicker.Date, "Date of joining"),
            Validation.Email(EmailEntry.Text),
            Validation.Phone(PhoneEntry.Text),
            Validation.Address(AddressEditor.Text));

        if (problem != "")
        {
            await DisplayAlert("Edit Teacher", problem, "OK");
            return;
        }

        int.TryParse(ExperienceEntry.Text, out int years);

        _teacher.FullName = (FullNameEntry.Text ?? "").Trim();
        _teacher.YearsOfExperience = years;
        _teacher.JoiningDate = JoiningDatePicker.Date;
        _teacher.Email = (EmailEntry.Text ?? "").Trim();
        _teacher.Phone = Validation.CleanPhone(PhoneEntry.Text);
        _teacher.Address = (AddressEditor.Text ?? "").Trim();

        try
        {
            AppData.Teachers.Update(_teacher);
            await DisplayAlert("Edit Teacher", "Changes saved.", "OK");
            await AppNavigation.GoBackAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Edit Teacher", ex.Message, "OK");
        }
    }
}
