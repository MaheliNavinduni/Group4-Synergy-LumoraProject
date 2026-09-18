using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class StudentRegistrationPage : ContentPage
{
    public StudentRegistrationPage()
    {
        InitializeComponent();

        BloodGroupPicker.ItemsSource = new List<string> { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
        GenderPicker.ItemsSource = new List<string> { "Female", "Male", "Other" };
        GradePicker.ItemsSource = new List<string> { "Grade 6", "Grade 7", "Grade 8", "Grade 9", "Grade 10", "Grade 11", "Grade 12", "Grade 13" };
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FullNameEntry.Text))
        {
            await DisplayAlert("Student Registration", "Please enter the student's full name.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(GuardianEntry.Text) || string.IsNullOrWhiteSpace(PhoneEntry.Text))
        {
            await DisplayAlert("Student Registration", "Please enter the parent/guardian name and phone number.", "OK");
            return;
        }

        // TODO: save the new student to the database.
        await DisplayAlert("Student Registration", "Student registered (demo only, not stored yet).", "OK");
        await AppNavigation.GoBackAsync();
    }
}
