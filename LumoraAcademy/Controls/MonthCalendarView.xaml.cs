using Microsoft.Maui.Controls.Shapes;

namespace LumoraAcademy.Controls;

public partial class MonthCalendarView : ContentView
{
    // Sample events shown on the calendar (day number -> event name, colour name).
    private static readonly Dictionary<int, (string Title, string Colour)> SampleEvents = new()
    {
        { 5, ("Faculty Meeting", "Blue") },
        { 10, ("Mid-Terms Begin", "Blue") },
        { 12, ("Conferences", "Red") },
    };

    private const int TodayDay = 11;

    public MonthCalendarView()
    {
        InitializeComponent();
        BuildDays();
    }

    // Creates one cell for each day of October 2023 (which starts on a Sunday).
    private void BuildDays()
    {
        int daysInMonth = 31;
        int firstDayColumn = 0;   // 0 = Sunday

        var divider = SidebarView.GetColor("DividerLine");
        var heading = SidebarView.GetColor("TextHeading");
        var brand = SidebarView.GetColor("BrandPrimary");

        for (int day = 1; day <= daysInMonth; day++)
        {
            int index = firstDayColumn + day - 1;
            int row = index / 7;
            int column = index % 7;

            bool isToday = day == TodayDay;

            var content = new VerticalStackLayout { Spacing = 4, Padding = new Thickness(6, 4) };

            var dayLabel = new Label
            {
                Text = day.ToString(),
                FontSize = 11,
                FontAttributes = isToday ? FontAttributes.Bold : FontAttributes.None,
                TextColor = isToday ? SidebarView.GetColor("TextLink") : heading,
            };
            content.Children.Add(dayLabel);

            if (isToday)
            {
                content.Children.Add(new Ellipse
                {
                    WidthRequest = 5,
                    HeightRequest = 5,
                    Fill = new SolidColorBrush(SidebarView.GetColor("TextLink")),
                    HorizontalOptions = LayoutOptions.End,
                });
            }

            if (SampleEvents.TryGetValue(day, out var ev))
            {
                content.Children.Add(new Border
                {
                    Background = new SolidColorBrush(SidebarView.GetColor("Status" + ev.Colour + "Bg")),
                    StrokeThickness = 0,
                    Padding = new Thickness(5, 2),
                    StrokeShape = new RoundRectangle { CornerRadius = 3 },
                    Content = new Label
                    {
                        Text = ev.Title,
                        FontSize = 8,
                        TextColor = SidebarView.GetColor("Status" + ev.Colour + "Text"),
                        LineBreakMode = LineBreakMode.TailTruncation,
                    },
                });
            }

            // Each cell has a thin border on the right and bottom.
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
}
