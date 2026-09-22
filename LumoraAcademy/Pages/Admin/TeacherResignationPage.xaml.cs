using LumoraAcademy.Core.Entities;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// "Teacher" here means the database entity, not the Pages.Teacher folder.
using Teacher = LumoraAcademy.Core.Entities.Teacher;

// Epic 1, User Story 2 - deactivate a teacher account (resignation).
public partial class TeacherResignationPage : ContentPage
{
    private Teacher? _teacher;

    public TeacherResignationPage(int? teacherId = null)
    {
        InitializeComponent();

        ReasonPicker.ItemsSource = new List<string> { "Personal reasons", "New position elsewhere", "Relocation", "Retirement", "Health", "Other" };
        LastDayPicker.Date = DateTime.Today;

        if (teacherId.HasValue)
        {
            ShowTeacher(AppData.Teachers.GetById(teacherId.Value));
        }
    }

    private async void OnSearchCompleted(object sender, EventArgs e)
    {
        var matches = AppData.Teachers.Search(SearchEntry.Text ?? "").Where(t => t.Status != "Resigned").ToList();

        if (matches.Count == 0)
        {
            await DisplayAlert("Find Teacher", "No active teacher found with that name or ID.", "OK");
            return;
        }
        if (matches.Count == 1)
        {
            ShowTeacher(matches[0]);
            return;
        }

        string[] names = matches.Select(m => $"{m.FullName} ({m.TeacherId})").ToArray();
        string choice = await DisplayActionSheet("Select teacher", "Cancel", null, names);
        int index = Array.IndexOf(names, choice);
        if (index >= 0) ShowTeacher(matches[index]);
    }

    private void ShowTeacher(Teacher? teacher)
    {
        _teacher = teacher;
        if (teacher == null) return;

        Avatar.Initials = teacher.Initials;
        NameLabel.Text = teacher.FullName;
        IdLabel.Text = $"ID: {teacher.TeacherId} • Joined {teacher.JoiningDate:MMM yyyy}";

        var subjects = teacher.Subjects.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Subject1Badge.Text = subjects.Length > 0 ? subjects[0] : teacher.Department;
        Subject2Badge.Text = subjects.Length > 1 ? subjects[1] : "";
        Subject2Badge.IsVisible = subjects.Length > 1;
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnProcessClicked(object sender, EventArgs e)
    {
        if (_teacher == null)
        {
            await DisplayAlert("Teacher Resignation", "Please search for and select a teacher first.", "OK");
            return;
        }
        if (ReasonPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Teacher Resignation", "Please select a reason for leaving.", "OK");
            return;
        }
        if (!KeysCheck.IsChecked || !GradesCheck.IsChecked || !AssetsCheck.IsChecked || !PayCheck.IsChecked)
        {
            await DisplayAlert("Teacher Resignation", "Please complete every item on the handover checklist first.", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Teacher Resignation", $"Process {_teacher.FullName}'s resignation? Their login will be deactivated.", "Process", "Cancel");
        if (!confirm) return;

        AppData.Teachers.ProcessResignation(_teacher.Id, LastDayPicker.Date, (string)ReasonPicker.SelectedItem, NotesEditor.Text ?? "");

        await DisplayAlert("Teacher Resignation", "Resignation processed and login deactivated.", "OK");
        await AppNavigation.GoBackAsync();
    }
}
