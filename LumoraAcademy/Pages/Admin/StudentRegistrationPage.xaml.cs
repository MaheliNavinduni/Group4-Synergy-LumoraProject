using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Services;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// Epic 2, User Story 1 - register a new student.
public partial class StudentRegistrationPage : ContentPage
{
    private string _photoPath = "";

    public StudentRegistrationPage()
    {
        InitializeComponent();

        BloodGroupPicker.ItemsSource = new List<string> { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
        GenderPicker.ItemsSource = new List<string> { "Female", "Male", "Other" };
        GradePicker.ItemsSource = new List<string> { "6th Grade", "7th Grade", "8th Grade", "9th Grade", "10th Grade", "11th Grade", "12th Grade", "13th Grade" };

        DobPicker.Date = DateTime.Today.AddYears(-12);
        DobPicker.MaximumDate = DateTime.Today;

        EnrollmentDatePicker.Date = DateTime.Today;
    }

    // ---------- Photo ----------

    // Lets the admin choose a JPG/PNG from the computer.
    private async void OnPhotoTapped(object sender, EventArgs e)
    {
        string? path = await PhotoService.PickAndSaveAsync("student");
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

    // ---------- Saving ----------

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Every rule lives in Validation, so all the forms behave the same way.
        string problem = Validation.FirstProblem(
            Validation.Name(FullNameEntry.Text, "Full name"),
            Validation.DateOfBirth(DobPicker.Date),
            Validation.Email(StudentEmailEntry.Text, required: false),
            Validation.Name(GuardianEntry.Text, "Parent / guardian name"),
            Validation.Phone(PhoneEntry.Text),
            Validation.Email(EmailEntry.Text, required: false),
            Validation.Address(AddressEditor.Text),
            GradePicker.SelectedIndex < 0 ? "Please select the grade." : "",
            Validation.NotInFuture(EnrollmentDatePicker.Date, "Enrollment date"));

        if (ShowProblem(problem)) return;

        var student = new Student
        {
            FullName = FullNameEntry.Text.Trim(),
            DateOfBirth = DobPicker.Date,
            BloodGroup = BloodGroupPicker.SelectedItem ?? "",
            Gender = GenderPicker.SelectedItem ?? "",
            Email = (StudentEmailEntry.Text ?? "").Trim(),
            School = (SchoolEntry.Text ?? "").Trim(),
            Grade = GradePicker.SelectedItem!,
            Section = (SectionEntry.Text ?? "").Trim().ToUpperInvariant(),
            GuardianName = GuardianEntry.Text.Trim(),
            GuardianPhone = Validation.CleanPhone(PhoneEntry.Text),
            GuardianEmail = (EmailEntry.Text ?? "").Trim(),
            Address = (AddressEditor.Text ?? "").Trim(),
            JoiningDate = EnrollmentDatePicker.Date,
            PreviousSchool = (PreviousSchoolEntry.Text ?? "").Trim(),
            PhotoPath = _photoPath,
            Status = "Active",
        };

        try
        {
            var saved = AppData.Students.Register(student);   // generates a unique Student ID
            await DisplayAlert("Student Registration", $"{saved.FullName} registered with ID {saved.StudentId}.", "OK");
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
