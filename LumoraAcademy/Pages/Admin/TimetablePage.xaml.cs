using LumoraAcademy.Core.Entities;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// Lets the Admin build a teacher's weekly timetable.
public partial class TimetablePage : ContentPage
{
    private readonly int _teacherId;
    private readonly List<Subject> _subjects;
    private static readonly string[] Days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

    public TimetablePage(int teacherId)
    {
        InitializeComponent();
        _teacherId = teacherId;

        var teacher = AppData.Teachers.GetById(teacherId);
        SubtitleLabel.Text = teacher == null ? "" : $"Lessons for {teacher.FullName} ({teacher.TeacherId})";

        _subjects = AppData.Academics.GetSubjects();
        SubjectPicker.ItemsSource = _subjects.Select(s => s.DisplayName).ToList();
        if (_subjects.Count > 0) SubjectPicker.SelectedIndex = 0;

        ClassPicker.ItemsSource = new List<string> { "6th Grade", "7th Grade", "8th Grade", "9th Grade", "10th Grade", "11th Grade", "12th Grade", "13th Grade" };
        ClassPicker.SelectedIndex = 4;

        DayPicker.ItemsSource = Days.ToList();
        DayPicker.SelectedIndex = 1;   // Monday

        LoadLessons();
    }

    private void LoadLessons()
    {
        var lessons = AppData.Schedule.GetWeekForTeacher(_teacherId);
        BindableLayout.SetItemsSource(RowList, lessons);
        EmptyLabel.IsVisible = lessons.Count == 0;
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        if (SubjectPicker.SelectedIndex < 0 || ClassPicker.SelectedIndex < 0 || DayPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Timetable", "Please choose the subject, class and day.", "OK");
            return;
        }

        var lesson = new ClassSession
        {
            TeacherId = _teacherId,
            SubjectId = _subjects[SubjectPicker.SelectedIndex].Id,
            ClassName = (string)ClassPicker.SelectedItem,
            DayOfWeek = DayPicker.SelectedIndex,
            StartTime = StartPicker.Time,
            EndTime = EndPicker.Time,
            Room = (RoomEntry.Text ?? "").Trim(),
        };

        try
        {
            AppData.Schedule.Add(lesson);   // checks the times and that the teacher is free
            LoadLessons();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Timetable", ex.Message, "OK");
        }
    }

    private async void OnDeleteTapped(object sender, EventArgs e)
    {
        if (sender is Label label && label.BindingContext is ClassSession lesson)
        {
            bool confirm = await DisplayAlert("Delete Lesson", $"Remove {lesson.SubjectName} on {lesson.DayName} at {lesson.StartText}?", "Delete", "Cancel");
            if (confirm)
            {
                AppData.Schedule.Delete(lesson.Id);
                LoadLessons();
            }
        }
    }

    private async void OnGoBackClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }
}
