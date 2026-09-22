using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Services;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Shared;

// Epic 4, User Stories 1 and 2 - record exam marks and Cambridge results.
public partial class EnterMarksPage : ContentPage
{
    private readonly int _studentId;
    private readonly List<Subject> _subjects;

    public EnterMarksPage(int studentId)
    {
        InitializeComponent();
        _studentId = studentId;

        var student = AppData.Students.GetById(studentId);
        SubtitleLabel.Text = student == null ? "" : $"{student.FullName} ({student.StudentId}) - {student.Grade}";

        _subjects = AppData.Academics.GetSubjects();
        var subjectNames = _subjects.Select(s => $"{s.Name} - {s.Code} ({s.Curriculum})").ToList();
        SubjectPicker.ItemsSource = subjectNames;
        CambridgeSubjectPicker.ItemsSource = subjectNames;

        ExamTypePicker.ItemsSource = new List<string> { "Monthly", "Term", "YearEnd" };
        ExamTypePicker.SelectedIndex = 0;

        LoadResults();
    }

    private void LoadResults()
    {
        var results = AppData.Academics.GetResultsForStudent(_studentId);
        BindableLayout.SetItemsSource(RowList, results);
        EmptyLabel.IsVisible = results.Count == 0;
    }

    private async void OnSaveMarkClicked(object sender, EventArgs e)
    {
        if (SubjectPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Enter Marks", "Please select a subject.", "OK");
            return;
        }
        if (!double.TryParse(MarksEntry.Text, out double marks))
        {
            await DisplayAlert("Enter Marks", "Please enter the marks as a number between 0 and 100.", "OK");
            return;
        }

        try
        {
            AppData.Academics.RecordMark(
                _studentId,
                _subjects[SubjectPicker.SelectedIndex].Id,
                (string)ExamTypePicker.SelectedItem,
                (ExamNameEntry.Text ?? "").Trim(),
                marks,
                (RemarksEntry.Text ?? "").Trim(),
                AppData.CurrentTeacherId);

            MarksEntry.Text = "";
            RemarksEntry.Text = "";
            LoadResults();
            await DisplayAlert("Enter Marks", $"Mark saved. Grade: {ExamResult.GradeFor(marks)}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Enter Marks", ex.Message, "OK");
        }
    }

    private async void OnSaveCambridgeClicked(object sender, EventArgs e)
    {
        if (CambridgeSubjectPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Cambridge Result", "Please select a subject.", "OK");
            return;
        }

        try
        {
            AppData.Academics.RecordCambridgeResult(
                _studentId,
                _subjects[CambridgeSubjectPicker.SelectedIndex].Id,
                (CambridgeExamEntry.Text ?? "").Trim(),
                (CambridgeGradeEntry.Text ?? "").Trim(),
                null,
                AppData.CurrentTeacherId);

            CambridgeGradeEntry.Text = "";
            LoadResults();
            await DisplayAlert("Cambridge Result", "Result saved.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Cambridge Result", ex.Message, "OK");
        }
    }

    private async void OnDeleteResultTapped(object sender, EventArgs e)
    {
        if (sender is Label label && label.BindingContext is ExamResult result)
        {
            bool confirm = await DisplayAlert("Delete Result", $"Delete the {result.ExamType} result for {result.SubjectName}?", "Delete", "Cancel");
            if (confirm)
            {
                AppData.Academics.DeleteResult(result.Id);
                LoadResults();
            }
        }
    }

    private async void OnGoBackClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }
}
