using LumoraAcademy.Controls;
using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Services;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// Attendance is marked in the office: pick the class, then mark each student as they arrive.
// Nothing is assumed - every student starts unmarked.
public partial class MarkAttendancePage : ContentPage
{
    private ClassGroup? _class;
    private List<AttendanceRow> _rows = new();

    public MarkAttendancePage()
    {
        InitializeComponent();

        DayPicker.Date = DateTime.Today;
        LoadClasses();
    }

    // ---------- Step 1: the day's classes ----------

    private void LoadClasses()
    {
        var date = DayPicker.Date;
        var sessions = AppData.Classes.GetSessionsForDay(date);

        // Show whether each class has already been marked that day.
        foreach (var s in sessions)
        {
            s.IsMarked = AppData.Attendance.IsMarked(s.ClassGroupId, date);
        }

        BindableLayout.SetItemsSource(ClassList, sessions);
        ClassListTitle.Text = $"Classes on {date:dddd, MMM dd}";
        NoClassesLabel.IsVisible = sessions.Count == 0;
    }

    private void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        ShowClassList();
        LoadClasses();
    }

    private void OnClassTapped(object sender, EventArgs e)
    {
        if (sender is Grid row && row.BindingContext is ClassSession session)
        {
            OpenSheet(session);
        }
    }

    // ---------- Step 2: the student sheet ----------

    private void OpenSheet(ClassSession session)
    {
        _class = AppData.Classes.GetById(session.ClassGroupId);
        if (_class == null) return;

        _rows = AppData.Attendance.GetSheet(_class.Id, DayPicker.Date);

        SheetTitle.Text = _class.Name;
        SheetSubtitle.Text = $"{session.TimeRange} • {_class.TeacherName} • {DayPicker.Date:dddd, MMM dd}";

        BindableLayout.SetItemsSource(RowList, _rows);
        NoStudentsLabel.IsVisible = _rows.Count == 0;

        ClassListCard.IsVisible = false;
        SheetSection.IsVisible = true;

        RefreshButtons();
        UpdateCounts();
    }

    private void ShowClassList()
    {
        SheetSection.IsVisible = false;
        ClassListCard.IsVisible = true;
        _class = null;
    }

    private void OnBackToClassesClicked(object sender, EventArgs e)
    {
        ShowClassList();
        LoadClasses();
    }

    // ---------- Marking ----------

    // Runs when Present / Absent / Late / Clear is clicked on a row.
    private void OnStatusClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.BindingContext is not AttendanceRow row) return;

        string choice = (string)button.CommandParameter;
        row.Status = choice == "Clear" ? "" : choice;

        if (button.Parent is HorizontalStackLayout buttons)
        {
            ColourButtons(buttons, row.Status);
        }
        UpdateCounts();
    }

    private void OnMarkAllPresentClicked(object sender, EventArgs e)
    {
        foreach (var row in _rows) row.Status = AttendanceEntry.Present;
        RefreshButtons();
        UpdateCounts();
    }

    // Everyone still unmarked becomes Absent - used at the end of the class.
    private void OnRestAbsentClicked(object sender, EventArgs e)
    {
        foreach (var row in _rows.Where(r => !r.IsMarked)) row.Status = AttendanceEntry.Absent;
        RefreshButtons();
        UpdateCounts();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_class == null) return;

        if (_rows.Count == 0)
        {
            await DisplayAlert("Mark Attendance", "There are no students in this class.", "OK");
            return;
        }

        AppData.Attendance.SaveSheet(_class.Id, DayPicker.Date, _rows, AppData.CurrentTeacherId);

        int marked = _rows.Count(r => r.IsMarked);
        await DisplayAlert("Mark Attendance", $"Saved. {marked} of {_rows.Count} students marked.", "OK");

        ShowClassList();
        LoadClasses();
    }

    // ---------- Display helpers ----------

    private void UpdateCounts()
    {
        EnrolledCard.Value = _rows.Count.ToString();
        PresentCard.Value = _rows.Count(r => r.Status == AttendanceEntry.Present).ToString();
        AbsentCard.Value = _rows.Count(r => r.Status == AttendanceEntry.Absent).ToString();
        NotMarkedCard.Value = _rows.Count(r => !r.IsMarked).ToString();
    }

    // Colours every row's buttons to match its status.
    private void RefreshButtons()
    {
        foreach (var child in RowList.Children)
        {
            if (child is VerticalStackLayout rowLayout && rowLayout.BindingContext is AttendanceRow row)
            {
                foreach (var buttons in FindButtonGroups(rowLayout))
                {
                    ColourButtons(buttons, row.Status);
                }
            }
        }
    }

    // Finds the row of Present / Absent / Late / Clear buttons.
    private static IEnumerable<HorizontalStackLayout> FindButtonGroups(Layout layout)
    {
        foreach (var child in layout.Children)
        {
            if (child is HorizontalStackLayout h && h.Children.Count == 4 && h.Children[0] is Button)
            {
                yield return h;
            }
            else if (child is Layout inner)
            {
                foreach (var found in FindButtonGroups(inner)) yield return found;
            }
        }
    }

    // Highlights the chosen button. When nothing is chosen, every button stays plain.
    private static void ColourButtons(HorizontalStackLayout buttons, string status)
    {
        foreach (var child in buttons.Children)
        {
            if (child is not Button b) continue;

            bool selected = !string.IsNullOrWhiteSpace(status) && b.Text == status;

            string colourName = b.Text switch
            {
                AttendanceEntry.Present => "Green",
                AttendanceEntry.Absent => "Red",
                AttendanceEntry.Late => "Yellow",
                _ => "Gray"
            };

            b.BackgroundColor = selected ? SidebarView.GetColor("Status" + colourName + "Bg") : SidebarView.GetColor("CardBackground");
            b.TextColor = selected ? SidebarView.GetColor("Status" + colourName + "Text") : SidebarView.GetColor("TextMuted");
            b.BorderColor = selected ? SidebarView.GetColor("Status" + colourName + "Bg") : SidebarView.GetColor("FieldBorder");
        }
    }
}
