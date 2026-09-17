namespace LumoraAcademy.Controls;

public partial class StatusBadge : ContentView
{
    // Usage in XAML:  <controls:StatusBadge Text="Active" />
    //             or  <controls:StatusBadge Text="{Binding Status}" />
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(StatusBadge), "", propertyChanged: OnTextChanged);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public StatusBadge()
    {
        InitializeComponent();
    }

    private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var badge = (StatusBadge)bindable;
        string text = (string)newValue ?? "";

        badge.PillText.Text = text;

        // Pick a colour pair based on what the badge says.
        string colourName = text.ToLower() switch
        {
            "active" or "paid" or "excellent" or "perfect" or "present" or "enrolled" or "sports" => "Green",
            "inactive" or "poor" or "dropout" or "overdue" or "absent" or "high priority" or "holiday" => "Red",
            "pending" or "good" or "late" or "extension granted" or "partial" => "Yellow",
            "on leave" or "academic" or "ongoing" => "Blue",
            "admin" => "Purple",
            _ => "Gray"
        };

        badge.Pill.Background = SidebarView.GetColor("Status" + colourName + "Bg");
        badge.PillText.TextColor = SidebarView.GetColor("Status" + colourName + "Text");
    }
}
