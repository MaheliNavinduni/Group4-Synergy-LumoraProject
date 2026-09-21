using LumoraAcademy.Core.Entities;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class AdminEventsPage : ContentPage
{
    private SchoolEvent? _editing;

    public AdminEventsPage()
    {
        InitializeComponent();

        NewCategoryPicker.ItemsSource = new List<string> { "Academic", "Admin", "Holiday", "Sports" };
        NewCategoryPicker.SelectedIndex = 0;
        NewDatePicker.Date = DateTime.Today;

        LoadEvents();
    }

    private void LoadEvents()
    {
        BindableLayout.SetItemsSource(RowList, AppData.Events.GetAll());
    }

    private async void OnCalendarViewTapped(object sender, EventArgs e)
    {
        // The calendar view is the same one teachers see.
        await AppNavigation.GoToAsync(new Teacher.TeacherEventsPage());
    }

    // Opens the "Edit Event" panel on the right with the clicked event's details.
    private void OnEditEventTapped(object sender, EventArgs e)
    {
        if (sender is Label label && label.BindingContext is SchoolEvent ev)
        {
            _editing = ev;
            EditTitleEntry.Text = ev.Title;
            EditDateEntry.Text = ev.Date.ToString("yyyy-MM-dd");
            EditTimeEntry.Text = ev.Time;
            EditCategoryEntry.Text = ev.Category;
            EditLocationEntry.Text = ev.Location;
            EditDescriptionEditor.Text = ev.Description;
            StaffRadio.IsChecked = ev.Visibility != "Admin Only";
            EditPanel.IsVisible = true;
        }
    }

    private async void OnDeleteEventTapped(object sender, EventArgs e)
    {
        if (sender is Label label && label.BindingContext is SchoolEvent ev)
        {
            bool confirm = await DisplayAlert("Delete Event", $"Delete '{ev.Title}'?", "Delete", "Cancel");
            if (confirm)
            {
                AppData.Events.Delete(ev.Id);
                LoadEvents();
            }
        }
    }

    private void OnCloseEditTapped(object sender, EventArgs e)
    {
        EditPanel.IsVisible = false;
        _editing = null;
    }

    private async void OnSaveEditClicked(object sender, EventArgs e)
    {
        if (_editing == null) return;

        if (!DateTime.TryParse(EditDateEntry.Text, out DateTime date))
        {
            await DisplayAlert("Edit Event", "Please enter the date as yyyy-mm-dd.", "OK");
            return;
        }

        _editing.Title = (EditTitleEntry.Text ?? "").Trim();
        _editing.Date = date;
        _editing.Time = (EditTimeEntry.Text ?? "").Trim();
        _editing.Category = (EditCategoryEntry.Text ?? "").Trim();
        _editing.Location = (EditLocationEntry.Text ?? "").Trim();
        _editing.Description = (EditDescriptionEditor.Text ?? "").Trim();
        _editing.Visibility = StaffRadio.IsChecked ? "Staff" : "Admin Only";

        try
        {
            AppData.Events.Update(_editing);
            EditPanel.IsVisible = false;
            _editing = null;
            LoadEvents();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Edit Event", ex.Message, "OK");
        }
    }

    private async void OnCreateEventClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NewTitleEntry.Text))
        {
            await DisplayAlert("Create Event", "Please enter an event title.", "OK");
            return;
        }

        var ev = new SchoolEvent
        {
            Title = NewTitleEntry.Text.Trim(),
            Category = NewCategoryPicker.SelectedItem as string ?? "Academic",
            Date = NewDatePicker.Date,
            Time = DateTime.Today.Add(NewTimePicker.Time).ToString("hh:mm tt"),
            Location = (NewLocationEntry.Text ?? "").Trim(),
            Description = (NewDescriptionEditor.Text ?? "").Trim(),
        };

        AppData.Events.Create(ev);

        NewTitleEntry.Text = "";
        NewLocationEntry.Text = "";
        NewDescriptionEditor.Text = "";
        LoadEvents();
        await DisplayAlert("Create Event", $"'{ev.Title}' added to the calendar.", "OK");
    }
}
