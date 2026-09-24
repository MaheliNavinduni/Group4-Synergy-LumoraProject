using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Services;
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
        RolePicker.SelectedIndex = 0;

        JoiningDatePicker.Date = DateTime.Today;
    }

    // ---------- Photo ----------

    private async void OnPhotoTapped(object sender, EventArgs e)
    {
        string? path = await PhotoService.PickAndSaveAsync("teacher");
        if (path == null) return;

        _photoPath = path;
        PhotoPreview.ImagePath = path;
        PhotoPreview.IsVisible = true;
        PhotoIcon.IsVisible = false;
        PhotoLabel.Text = "Click to change the photo";
        RemovePhotoButton.IsVisible = true;
    }

    private void OnRemovePhotoClicked(object sender, EventArgs e)
    {
        _photoPath = "";
        PhotoPreview.IsVisible = false;
        PhotoIcon.IsVisible = true;
        PhotoLabel.Text = "Click to upload a photo";
        RemovePhotoButton.IsVisible = false;
    }

    // Lets the admin check the password they typed before saving it.
    private void OnTogglePasswordTapped(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        ShowPasswordIcon.Text = PasswordEntry.IsPassword ? "" : "";
    }

    // ---------- Saving ----------

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        string problem = Validation.FirstProblem(
            Validation.Name(FullNameEntry.Text, "Full name"),
            RolePicker.SelectedIndex < 0 ? "Please select the designation." : "",
            Validation.WholeNumber(ExperienceEntry.Text, "Years of experience", 0, 60, required: false),
            Validation.NotInFuture(JoiningDatePicker.Date, "Date of joining"),
            Validation.Email(EmailEntry.Text),
            Validation.Phone(PhoneEntry.Text),
            Validation.Address(AddressEditor.Text),
            Validation.Username(UsernameEntry.Text),
            Validation.Password(PasswordEntry.Text));

        if (ShowProblem(problem)) return;

        int.TryParse(ExperienceEntry.Text, out int years);

        var teacher = new Teacher
        {
            FullName = FullNameEntry.Text.Trim(),
            Designation = RolePicker.SelectedItem ?? "Teacher",
            Department = (DepartmentEntry.Text ?? "").Trim(),
            Subjects = (SubjectsEntry.Text ?? "").Trim(),
            YearsOfExperience = years,
            JoiningDate = JoiningDatePicker.Date,
            Email = EmailEntry.Text.Trim(),
            Phone = Validation.CleanPhone(PhoneEntry.Text),
            Address = (AddressEditor.Text ?? "").Trim(),
            Status = "Active",
            PhotoPath = _photoPath,
        };

        try
        {
            string username = UsernameEntry.Text.Trim();
            var saved = AppData.Teachers.Register(teacher, username, PasswordEntry.Text);

            await DisplayAlert("Teacher Registration",
                $"{saved.FullName} registered with ID {saved.TeacherId}. They can now log in as '{username.ToLower()}'.", "OK");
            await AppNavigation.GoBackAsync();
        }
        catch (Exception ex)
        {
            ShowProblem(ex.Message);
        }
    }

    // Shows the message in the red bar at the top of the form.
    // Returns true when there was a problem, so the caller can stop.
    private bool ShowProblem(string message)
    {
        bool hasProblem = !string.IsNullOrEmpty(message);

        ErrorLabel.Text = message;
        ErrorBox.IsVisible = hasProblem;

        return hasProblem;
    }
}
