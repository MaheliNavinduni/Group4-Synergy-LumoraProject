using LumoraAcademy.Controls;
using LumoraAcademy.Models;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// Marks attendance for one class for today and saves it to the database.
public partial class MarkAttendancePage : ContentPage
{
    private List<MarkAttendanceRow> _rows = new();
    private List<Core.Entities.Subject> _subjects = new();

    public MarkAttendancePage()
    {
        InitializeComponent();

        // Classes = the grades that have active students
        var grades = AppData.Students.GetAll()
            .Where(s => s.Status == "Active")
            .Select(s => s.Grade)
            .Distinct()
            .OrderBy(g => g)
            .ToList();
        ClassPicker.ItemsSource = grades;
        ClassPicker.SelectedIndexChanged += (s, e) => LoadStudents();
        if (grades.Count > 0) ClassPicker.SelectedIndex = 0;

        _subjects = AppData.Academics.GetSubjects();
        SubjectPicker.ItemsSource = _subjects.Select(x => x.DisplayName).ToList();
        if (_subjects.Count > 0) SubjectPicker.SelectedIndex = 0;

        Loaded += (s, e) => RefreshAllRows();
    }

    // Loads the students of the selected class. If attendance was already saved today, it shows those statuses.
    private void LoadStudents()
    {
        string grade = ClassPicker.SelectedItem as string ?? "";
        var students = AppData.Students.GetAll().Where(s => s.Status == "Active" && s.Grade == grade).ToList();
        var saved = AppData.Attendance.GetForClassOnDay(grade, DateTime.Today).ToDictionary(a => a.StudentId);

        _rows = students.Select(s => new MarkAttendanceRow
        {
            StudentDbId = s.Id,
            Name = s.FullName,
            Id = s.StudentId,
            Initials = s.Initials,
            Status = saved.TryGetValue(s.Id, out var entry) ? entry.Status : "Present",
            Remarks = saved.TryGetValue(s.Id, out var e2) ? e2.Remarks : "",
        }).ToList();

        BindableLayout.SetItemsSource(RowList, _rows);
        CountLabel.Text = $"Showing {_rows.Count} students";
        RefreshAllRows();
        UpdateCounts();
    }

    private void UpdateCounts()
    {
        TotalCard.Value = _rows.Count.ToString();
        PresentCard.Value = _rows.Count(r => r.Status == "Present").ToString();
        AbsentCard.Value = _rows.Count(r => r.Status == "Absent").ToString();
        LateCard.Value = _rows.Count(r => r.Status == "Late").ToString();
    }

    // Runs when one of the Present / Absent / Late buttons is clicked.
    private void OnStatusClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is MarkAttendanceRow row)
        {
            row.Status = (string)button.CommandParameter;

            if (button.Parent is HorizontalStackLayout buttons)
            {
                ColourButtons(buttons, row.Status);
            }
            UpdateCounts();
        }
    }

    private void OnMarkAllPresentTapped(object sender, EventArgs e)
    {
        foreach (var row in _rows)
        {
            row.Status = "Present";
        }
        RefreshAllRows();
        UpdateCounts();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_rows.Count == 0)
        {
            await DisplayAlert("Mark Attendance", "There are no students in this class.", "OK");
            return;
        }

        string grade = ClassPicker.SelectedItem as string ?? "";
        int? subjectId = SubjectPicker.SelectedIndex >= 0 ? _subjects[SubjectPicker.SelectedIndex].Id : null;

        AppData.Attendance.MarkClass(
            DateTime.Today,
            grade,
            _rows.Select(r => (r.StudentDbId, r.Status, r.Remarks)),
            subjectId,
            AppData.CurrentTeacherId);

        await DisplayAlert("Mark Attendance", $"Attendance saved for {_rows.Count} students.", "OK");
        await AppNavigation.GoBackAsync();
    }

    // Goes through every row and colours its buttons.
    private void RefreshAllRows()
    {
        foreach (var child in RowList.Children)
        {
            if (child is VerticalStackLayout rowLayout && rowLayout.BindingContext is MarkAttendanceRow row)
            {
                foreach (var buttons in FindButtonGroups(rowLayout))
                {
                    ColourButtons(buttons, row.Status);
                }
            }
        }
    }

    // Finds the HorizontalStackLayout that holds the three status buttons.
    private static IEnumerable<HorizontalStackLayout> FindButtonGroups(Layout layout)
    {
        foreach (var child in layout.Children)
        {
            if (child is HorizontalStackLayout h && h.Children.Count == 3 && h.Children[0] is Button)
            {
                yield return h;
            }
            else if (child is Layout inner)
            {
                foreach (var found in FindButtonGroups(inner))
                {
                    yield return found;
                }
            }
        }
    }

    // Highlights the selected status button (green, red or yellow) and clears the others.
    private static void ColourButtons(HorizontalStackLayout buttons, string status)
    {
        foreach (var child in buttons.Children)
        {
            if (child is not Button b) continue;

            bool selected = b.Text == status;
            string colourName = status switch
            {
                "Present" => "Green",
                "Absent" => "Red",
                _ => "Yellow"
            };

            b.BackgroundColor = selected ? SidebarView.GetColor("Status" + colourName + "Bg") : SidebarView.GetColor("CardBackground");
            b.TextColor = selected ? SidebarView.GetColor("Status" + colourName + "Text") : SidebarView.GetColor("TextMuted");
            b.BorderColor = selected ? SidebarView.GetColor("Status" + colourName + "Bg") : SidebarView.GetColor("FieldBorder");
        }
    }
}
