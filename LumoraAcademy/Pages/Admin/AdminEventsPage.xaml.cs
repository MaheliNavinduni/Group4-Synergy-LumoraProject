using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Services;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// The institute calendar. It opens on the calendar, and the List switch shows
// the same events as a table. Creating and editing both use the same form,
// which opens over the page.
public partial class AdminEventsPage : ContentPage
{
    private static readonly List<string> Categories = new() { "Academic", "Admin", "Holiday", "Sports" };

    // The event being edited, or null when a new one is being created.
    private SchoolEvent? _editing;

    public AdminEventsPage()
    {
        InitializeComponent();

        CategoryPicker.ItemsSource = Categories;
        CategoryPicker.SelectedIndex = 0;

        ShowCalendar();
        LoadEvents();
    }

    // ---------- Switching between the calendar and the list ----------

    private void OnShowCalendar(object sender, EventArgs e) => ShowCalendar();

    private void OnShowList(object sender, EventArgs e) => ShowList();

    private void ShowCalendar()
    {
        CalendarView.IsVisible = true;
        ListView.IsVisible = false;
        PaintTabs(calendarActive: true);
    }

    private void ShowList()
    {
        CalendarView.IsVisible = false;
        ListView.IsVisible = true;
        PaintTabs(calendarActive: false);
    }

    // The side you are on is filled in; the other side stays plain, so it is
    // clear you can click it to go back.
    private void PaintTabs(bool calendarActive)
    {
        var activeBg = Controls.SidebarView.GetColor("BrandCream");
        var activeText = Controls.SidebarView.GetColor("Brown700");
        var plainText = Controls.SidebarView.GetColor("TextMuted");

        CalendarTab.Background = calendarActive ? activeBg : Colors.Transparent;
        CalendarTabText.TextColor = calendarActive ? activeText : plainText;
        CalendarTabIcon.TextColor = calendarActive ? activeText : plainText;
        CalendarTabText.FontAttributes = calendarActive ? FontAttributes.Bold : FontAttributes.None;

        ListTab.Background = calendarActive ? Colors.Transparent : activeBg;
        ListTabText.TextColor = calendarActive ? plainText : activeText;
        ListTabIcon.TextColor = calendarActive ? plainText : activeText;
        ListTabText.FontAttributes = calendarActive ? FontAttributes.None : FontAttributes.Bold;
    }

    // ---------- Loading ----------

    private void LoadEvents()
    {
        var all = AppData.Events.GetAll();
        BindableLayout.SetItemsSource(RowList, all);
        NoEventsLabel.IsVisible = all.Count == 0;

        var upcoming = AppData.Events.GetUpcoming(6);
        BindableLayout.SetItemsSource(UpcomingList, upcoming);
        NoUpcomingLabel.IsVisible = upcoming.Count == 0;

        Calendar.Refresh();
    }

    // ---------- The form ----------

    private void OnOpenCreateClicked(object sender, EventArgs e)
    {
        _editing = null;

        PopupTitle.Text = "Create New Event";
        SaveButton.Text = "Save Event";

        TitleEntry.Text = "";
        CategoryPicker.SelectedIndex = 0;
        DatePicker.Date = DateTime.Today;
        FromTimePicker.Time = new TimeSpan(8, 0, 0);
        ToTimePicker.Time = new TimeSpan(10, 0, 0);
        LocationEntry.Text = "";
        DescriptionEditor.Text = "";
        StaffRadio.IsChecked = true;

        ShowProblem("");
        EventPopup.IsVisible = true;
    }

    private void OnEditEventTapped(object sender, EventArgs e)
    {
        if (sender is not Label label || label.BindingContext is not SchoolEvent ev) return;

        _editing = ev;

        PopupTitle.Text = "Edit Event";
        SaveButton.Text = "Save Changes";

        TitleEntry.Text = ev.Title;
        CategoryPicker.SelectedItem = ev.Category;
        DatePicker.Date = ev.Date;

        var (from, to) = SplitTime(ev.Time);
        FromTimePicker.Time = from;
        ToTimePicker.Time = to;

        LocationEntry.Text = ev.Location;
        DescriptionEditor.Text = ev.Description;
        StaffRadio.IsChecked = ev.Visibility != "Admin Only";

        ShowProblem("");
        EventPopup.IsVisible = true;
    }

    private void OnClosePopup(object sender, EventArgs e)
    {
        EventPopup.IsVisible = false;
        _editing = null;
    }

    private async void OnSaveEventClicked(object sender, EventArgs e)
    {
        string problem = Validation.FirstProblem(
            Validation.Required(TitleEntry.Text, "Event title"),
            CategoryPicker.SelectedIndex < 0 ? "Please select the category." : "",
            Validation.Required(LocationEntry.Text, "Location"),
            ToTimePicker.Time <= FromTimePicker.Time ? "The finish time must be after the start time." : "",
            // Only a brand new event has to be in the future; an old one can be corrected.
            _editing == null ? Validation.NotInPast(DatePicker.Date, "Event date") : "");

        if (ShowProblem(problem)) return;

        string duration = $"{FromTimePicker.Text} - {ToTimePicker.Text}";
        string name = TitleEntry.Text.Trim();
        bool wasNew = _editing == null;

        try
        {
            if (_editing == null)
            {
                AppData.Events.Create(new SchoolEvent
                {
                    Title = name,
                    Category = CategoryPicker.SelectedItem ?? "Academic",
                    Date = DatePicker.Date,
                    Time = duration,
                    Location = (LocationEntry.Text ?? "").Trim(),
                    Description = (DescriptionEditor.Text ?? "").Trim(),
                    Visibility = StaffRadio.IsChecked ? "Staff" : "Admin Only",
                });
            }
            else
            {
                _editing.Title = name;
                _editing.Category = CategoryPicker.SelectedItem ?? "Academic";
                _editing.Date = DatePicker.Date;
                _editing.Time = duration;
                _editing.Location = (LocationEntry.Text ?? "").Trim();
                _editing.Description = (DescriptionEditor.Text ?? "").Trim();
                _editing.Visibility = StaffRadio.IsChecked ? "Staff" : "Admin Only";

                AppData.Events.Update(_editing);
            }

            EventPopup.IsVisible = false;
            _editing = null;
            LoadEvents();

            await DisplayAlert("Events", wasNew ? $"'{name}' added to the calendar." : "Changes saved.", "OK");
        }
        catch (Exception ex)
        {
            ShowProblem(ex.Message);
        }
    }

    private async void OnDeleteEventTapped(object sender, EventArgs e)
    {
        if (sender is not Label label || label.BindingContext is not SchoolEvent ev) return;

        bool confirm = await DisplayAlert("Delete Event", $"Delete '{ev.Title}'?", "Delete", "Cancel");
        if (!confirm) return;

        AppData.Events.Delete(ev.Id);
        LoadEvents();
    }

    // ---------- Helpers ----------

    // Reads a stored duration such as "08:00 AM - 10:00 AM" back into two times.
    private static (TimeSpan From, TimeSpan To) SplitTime(string? stored)
    {
        var start = new TimeSpan(8, 0, 0);
        var finish = new TimeSpan(10, 0, 0);

        if (string.IsNullOrWhiteSpace(stored)) return (start, finish);

        var parts = stored.Split('-', 2);

        TimeSpan from = ParseTime(parts[0]) ?? start;
        TimeSpan to = (parts.Length > 1 ? ParseTime(parts[1]) : null) ?? from.Add(TimeSpan.FromHours(2));

        return (from, to);
    }

    private static TimeSpan? ParseTime(string text)
    {
        return DateTime.TryParse(text.Trim(), out DateTime parsed) ? parsed.TimeOfDay : null;
    }

    // Shows the message in the red bar inside the form.
    private bool ShowProblem(string message)
    {
        bool hasProblem = !string.IsNullOrEmpty(message);

        PopupErrorLabel.Text = message;
        PopupErrorBox.IsVisible = hasProblem;

        return hasProblem;
    }
}
