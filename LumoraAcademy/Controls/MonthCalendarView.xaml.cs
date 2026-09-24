using LumoraAcademy.Core.Entities;
using LumoraAcademy.Services;
using Microsoft.Maui.Controls.Shapes;

namespace LumoraAcademy.Controls;

public partial class MonthCalendarView : ContentView
{
    private DateTime _month = new(DateTime.Today.Year, DateTime.Today.Month, 1);

    public MonthCalendarView()
    {
        InitializeComponent();
        BuildDays();
        Loaded += (s, e) => BuildDays();
    }

    // Draws one cell per day of the month, with that day's events from the database.
    private void BuildDays()
    {
        DaysGrid.Children.Clear();
        MonthLabel.Text = _month.ToString("MMMM yyyy");

        int daysInMonth = DateTime.DaysInMonth(_month.Year, _month.Month);
        int firstDayColumn = (int)_month.DayOfWeek;   // 0 = Sunday

        // Some months need a sixth row
        int rowsNeeded = (firstDayColumn + daysInMonth + 6) / 7;
        while (DaysGrid.RowDefinitions.Count < rowsNeeded) DaysGrid.RowDefinitions.Add(new RowDefinition(80));

        var events = AppData.Events.GetForMonth(_month.Year, _month.Month)
            .GroupBy(e => e.Date.Day)
            .ToDictionary(g => g.Key, g => g.ToList());

        var divider = SidebarView.GetColor("DividerLine");
        var heading = SidebarView.GetColor("TextHeading");
        var link = SidebarView.GetColor("TextLink");

        for (int day = 1; day <= daysInMonth; day++)
        {
            int index = firstDayColumn + day - 1;
            int row = index / 7;
            int column = index % 7;

            bool isToday = new DateTime(_month.Year, _month.Month, day) == DateTime.Today;

            var content = new VerticalStackLayout { Spacing = 3, Padding = new Thickness(6, 4) };

            content.Children.Add(new Label
            {
                Text = day.ToString(),
                FontSize = 11,
                FontAttributes = isToday ? FontAttributes.Bold : FontAttributes.None,
                TextColor = isToday ? link : heading,
            });

            if (events.TryGetValue(day, out var dayEvents))
            {
                foreach (var ev in dayEvents.Take(2))
                {
                    string colour = ColourFor(ev.Category);
                    content.Children.Add(new Border
                    {
                        Background = new SolidColorBrush(SidebarView.GetColor("Status" + colour + "Bg")),
                        StrokeThickness = 0,
                        Padding = new Thickness(5, 2),
                        StrokeShape = new RoundRectangle { CornerRadius = 3 },
                        Content = new Label
                        {
                            Text = ev.Title,
                            FontSize = 8,
                            TextColor = SidebarView.GetColor("Status" + colour + "Text"),
                            LineBreakMode = LineBreakMode.TailTruncation,
                        },
                    });
                }
            }

            var cell = new Border
            {
                Stroke = divider,
                StrokeThickness = 0.5,
                Padding = 0,
                StrokeShape = new Rectangle(),
                Content = content,
            };

            DaysGrid.Add(cell, column, row);
        }
    }

    private static string ColourFor(string category) => category switch
    {
        "Holiday" => "Red",
        "Sports" => "Green",
        "Admin" => "Purple",
        _ => "Blue",
    };
}
