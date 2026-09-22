using LumoraAcademy.Core.Entities;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// Marks a student as having left the institute (Inactive / Dropout).
public partial class StudentDeparturePage : ContentPage
{
    private Student? _student;

    // Opened from Student Details (student already chosen) or from the menu (studentId = null).
    public StudentDeparturePage(int? studentId = null)
    {
        InitializeComponent();

        ReasonPicker.ItemsSource = new List<string> { "Relocation", "Transfer to another school", "Financial reasons", "Completed studies", "Dropout", "Other" };
        DepartureDatePicker.Date = DateTime.Today;

        if (studentId.HasValue)
        {
            ShowStudent(AppData.Students.GetById(studentId.Value));
        }
    }

    // Runs when the user presses Enter in the search box.
    private async void OnSearchCompleted(object sender, EventArgs e)
    {
        var matches = AppData.Students.Search(SearchEntry.Text);

        if (matches.Count == 0)
        {
            await DisplayAlert("Find Student", "No student found with that ID or name.", "OK");
            return;
        }

        if (matches.Count == 1)
        {
            ShowStudent(matches[0]);
            return;
        }

        // More than one match - let the user pick.
        string[] names = matches.Select(m => $"{m.FullName} ({m.StudentId})").ToArray();
        string choice = await DisplayActionSheet("Select student", "Cancel", null, names);
        int index = Array.IndexOf(names, choice);
        if (index >= 0) ShowStudent(matches[index]);
    }

    private void ShowStudent(Student? student)
    {
        _student = student;
        if (student == null) return;

        Avatar.Initials = student.Initials;
        NameLabel.Text = student.FullName;
        GradeLabel.Text = $"{student.Grade} • Joined {student.JoiningDate:MMM yyyy}";
        IdLabel.Text = "ID: " + student.StudentId;
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        if (_student == null)
        {
            await DisplayAlert("Student Departure", "Please search for and select a student first.", "OK");
            return;
        }
        if (ReasonPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Student Departure", "Please select a reason.", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Student Departure", $"Confirm {_student.FullName}'s departure? Their record will be kept but marked as inactive.", "Confirm", "Cancel");
        if (!confirm) return;

        string reason = (string)ReasonPicker.SelectedItem;
        AppData.Students.ProcessDeparture(_student.Id, DepartureDatePicker.Date, reason, NotesEditor.Text ?? "", isDropout: reason == "Dropout");

        await DisplayAlert("Student Departure", "Departure recorded.", "OK");
        await AppNavigation.GoBackAsync();
    }
}
