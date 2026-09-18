using LumoraAcademy.Controls;
using LumoraAcademy.Data;
using LumoraAcademy.Models;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class MarkAttendancePage : ContentPage
{
    public MarkAttendancePage()
    {
        InitializeComponent();

        ClassPicker.ItemsSource = new List<string> { "11th Grade, Section B", "10th Grade, Section A", "9th Grade, Section C" };
        ClassPicker.SelectedIndex = 0;

        SubjectPicker.ItemsSource = new List<string> { "Mathematics", "Science", "ICT", "English", "Sinhala", "Tamil" };
        SubjectPicker.SelectedIndex = 0;

        BindableLayout.SetItemsSource(RowList, SampleData.MarkAttendanceRows);

        // Colour the buttons to match each student's current status.
        Loaded += (s, e) => RefreshAllRows();
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
        }
    }

    private void OnMarkAllPresentTapped(object sender, EventArgs e)
    {
        foreach (var row in SampleData.MarkAttendanceRows)
        {
            row.Status = "Present";
        }
        RefreshAllRows();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // TODO: save the attendance to the database.
        await DisplayAlert("Mark Attendance", "Attendance saved (demo only, not stored yet).", "OK");
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
