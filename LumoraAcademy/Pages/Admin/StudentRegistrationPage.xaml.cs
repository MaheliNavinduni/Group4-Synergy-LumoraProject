using LumoraAcademy.Core.Entities;
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
        EnrollmentDatePicker.Date = DateTime.Today;
    }

    // Lets the admin choose a JPG/PNG from the computer.
    private async void OnPhotoTapped(object sender, EventArgs e)
    {
        string? path = await PhotoService.PickAndSaveAsync("student");
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

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Mandatory fields
        if (string.IsNullOrWhiteSpace(FullNameEntry.Text))
        {
            await DisplayAlert("Student Registration", "Please enter the student's full name.", "OK");
            return;
        }
        if (GradePicker.SelectedIndex < 0)
        {
            await DisplayAlert("Student Registration", "Please select the grade / class.", "OK");
            return;
        }
        if (string.IsNullOrWhiteSpace(GuardianEntry.Text) || string.IsNullOrWhiteSpace(PhoneEntry.Text))
        {
            await DisplayAlert("Student Registration", "Please enter the parent/guardian name and phone number.", "OK");
            return;
        }

        var student = new Student
        {
            FullName = FullNameEntry.Text.Trim(),
            DateOfBirth = DobPicker.Date,
            BloodGroup = BloodGroupPicker.SelectedItem as string ?? "",
            Gender = GenderPicker.SelectedItem as string ?? "",
            Grade = (string)GradePicker.SelectedItem,
            GuardianName = GuardianEntry.Text.Trim(),
            GuardianPhone = PhoneEntry.Text.Trim(),
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
            await DisplayAlert("Student Registration", ex.Message, "OK");
        }
    }
}
