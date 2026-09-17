using LumoraAcademy.Data;
using LumoraAcademy.Models;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class AdminEventsPage : ContentPage
{
    public AdminEventsPage()
    {
        InitializeComponent();

        NewCategoryPicker.ItemsSource = new List<string> { "Academic", "Admin", "Holiday", "Sports" };
        NewCategoryPicker.SelectedIndex = 0;

        BindableLayout.SetItemsSource(RowList, SampleData.Events);
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
            EditTitleEntry.Text = ev.Name;
            EditDateEntry.Text = ev.Date;
            EditTimeEntry.Text = ev.Time;
            EditCategoryEntry.Text = ev.Category;
            EditLocationEntry.Text = ev.Location;
            EditDescriptionEditor.Text = "Annual showcase of student science projects across all grade levels. Setup begins at 7 AM.";
            EditPanel.IsVisible = true;
        }
    }

    private async void OnDeleteEventTapped(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Delete Event", "Are you sure you want to delete this event?", "Delete", "Cancel");
        if (confirm)
        {
            // TODO: delete from the database.
            await DisplayAlert("Delete Event", "Event deleted (demo only).", "OK");
        }
    }

    private void OnCloseEditTapped(object sender, EventArgs e)
    {
        EditPanel.IsVisible = false;
    }

    private async void OnSaveEditClicked(object sender, EventArgs e)
    {
        // TODO: save changes to the database.
        await DisplayAlert("Edit Event", "Changes saved (demo only).", "OK");
        EditPanel.IsVisible = false;
    }

    private async void OnCreateEventClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NewTitleEntry.Text))
        {
            await DisplayAlert("Create Event", "Please enter an event title.", "OK");
            return;
        }

        // TODO: save the new event to the database.
        await DisplayAlert("Create Event", "Event created (demo only).", "OK");
    }
}
