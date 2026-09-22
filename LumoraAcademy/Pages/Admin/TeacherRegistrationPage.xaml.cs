using LumoraAcademy.Core.Entities;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// "Teacher" here means the database entity, not the Pages.Teacher folder.
using Teacher = LumoraAcademy.Core.Entities.Teacher;

// Epic 1, User Story 2 - create a teacher account (teacher record + login).
public partial class TeacherRegistrationPage : ContentPage
{
    private string _photoPath = "";

    public TeacherRegistrationPage()
    {
        InitializeComponent();

        RolePicker.ItemsSource = new List<string> { "Teacher", "Senior Teacher", "Head of Department", "Visiting Lecturer" };
        JoiningDatePicker.Date = DateTime.Today;
    }

    private async void OnPhotoTapped(object sender, EventArgs e)
    {
        string? path = await PhotoService.PickAndSaveAsync("teacher");
        if (path == null) return;

        _photoPath = path;
        PhotoPreview.ImagePath = path;
        PhotoPreview.IsVisible = true;
        PhotoIcon.IsVisible = false;
        PhotoLabel.Text = "Change Photo";
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FullNameEntry.Text))
        {
            await DisplayAlert("Teacher Registration", "Please enter the teacher's full name.", "OK");
            return;
        }
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PhoneEntry.Text))
        {
            await DisplayAlert("Teacher Registration", "Please enter the email address and phone number.", "OK");
            return;
        }
        if (string.IsNullOrWhiteSpace(UsernameEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Teacher Registration", "Please enter a username and password for the teacher's login.", "OK");
            return;
        }

        int.TryParse(ExperienceEntry.Text, out int years);

        var teacher = new Teacher
        {
            FullName = FullNameEntry.Text.Trim(),
            Designation = RolePicker.SelectedItem as string ?? "Teacher",
            YearsOfExperience = years,
            JoiningDate = JoiningDatePicker.Date,
            Email = EmailEntry.Text.Trim(),
            Phone = PhoneEntry.Text.Trim(),
            Address = (AddressEditor.Text ?? "").Trim(),
            Status = "Active",
            PhotoPath = _photoPath,
        };

        try
        {
            var saved = AppData.Teachers.Register(teacher, UsernameEntry.Text.Trim(), PasswordEntry.Text);
            await DisplayAlert("Teacher Registration", $"{saved.FullName} registered with ID {saved.TeacherId}. They can now log in as '{UsernameEntry.Text.Trim().ToLower()}'.", "OK");
            await AppNavigation.GoBackAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Teacher Registration", ex.Message, "OK");
        }
    }
}
