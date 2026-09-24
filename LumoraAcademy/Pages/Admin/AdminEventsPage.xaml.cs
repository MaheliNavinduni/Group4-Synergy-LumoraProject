using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Services;
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
            EditDatePicker.Date = ev.Date;
            EditTimeEntry.Text = ev.Time;
            EditCategoryPicker.ItemsSource = new List<string> { "Academic", "Admin", "Holiday", "Sports" };
            EditCategoryPicker.SelectedItem = ev.Category;
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

        string problem = Validation.FirstProblem(
            Validation.Required(EditTitleEntry.Text, "Event title"),
            EditCategoryPicker.SelectedIndex < 0 ? "Please select the category." : "",
            Validation.Required(EditLocationEntry.Text, "Location"));

        if (problem != "")
        {
            await DisplayAlert("Edit Event", problem, "OK");
            return;
        }

        _editing.Title = (EditTitleEntry.Text ?? "").Trim();
        _editing.Date = EditDatePicker.Date;
        _editing.Time = (EditTimeEntry.Text ?? "").Trim();
        _editing.Category = EditCategoryPicker.SelectedItem ?? "Academic";
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
        string problem = Validation.FirstProblem(
            Validation.Required(NewTitleEntry.Text, "Event title"),
            NewCategoryPicker.SelectedIndex < 0 ? "Please select the category." : "",
            Validation.Required(NewLocationEntry.Text, "Location"),
            Validation.NotInPast(NewDatePicker.Date, "Event date"));

        if (problem != "")
        {
            await DisplayAlert("Create Event", problem, "OK");
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
