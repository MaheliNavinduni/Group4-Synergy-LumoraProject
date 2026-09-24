using LumoraAcademy.Core.Entities;
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
        if (!int.TryParse(ExperienceEntry.Text, out int years) || years < 0)
        {
            await DisplayAlert("Edit Teacher", "Please enter the years of experience as a whole number.", "OK");
            return;
        }

        _teacher.FullName = (FullNameEntry.Text ?? "").Trim();
        _teacher.YearsOfExperience = years;
        _teacher.JoiningDate = JoiningDatePicker.Date;
        _teacher.Email = (EmailEntry.Text ?? "").Trim();
        _teacher.Phone = (PhoneEntry.Text ?? "").Trim();
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
