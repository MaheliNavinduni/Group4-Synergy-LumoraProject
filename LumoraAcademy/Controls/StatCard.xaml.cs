namespace LumoraAcademy.Controls;

public partial class StatCard : ContentView
{
    // Usage in XAML:
    // <controls:StatCard Title="TOTAL STUDENTS" Value="2,451" Subtitle="+12 this month" Icon="&#xE716;" />

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(StatCard), "", propertyChanged: (b, o, n) => ((StatCard)b).TitleLabel.Text = (string)n);

    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(string), typeof(StatCard), "", propertyChanged: (b, o, n) => ((StatCard)b).ValueLabel.Text = (string)n);

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(StatCard), "", propertyChanged: (b, o, n) => ((StatCard)b).SubtitleLabel.Text = (string)n);

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(string), typeof(StatCard), "", propertyChanged: (b, o, n) => ((StatCard)b).IconLabel.Text = (string)n);

    public static readonly BindableProperty IsDarkProperty =
        BindableProperty.Create(nameof(IsDark), typeof(bool), typeof(StatCard), false, propertyChanged: (b, o, n) => ((StatCard)b).ApplyDark((bool)n));

    public static readonly BindableProperty ValueColorProperty =
        BindableProperty.Create(nameof(ValueColor), typeof(Color), typeof(StatCard), null, propertyChanged: (b, o, n) => { if (n is Color c) ((StatCard)b).ValueLabel.TextColor = c; });

    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public string Subtitle { get => (string)GetValue(SubtitleProperty); set => SetValue(SubtitleProperty, value); }
    public string Icon { get => (string)GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public bool IsDark { get => (bool)GetValue(IsDarkProperty); set => SetValue(IsDarkProperty, value); }
    public Color? ValueColor { get => (Color?)GetValue(ValueColorProperty); set => SetValue(ValueColorProperty, value); }

    public StatCard()
    {
        InitializeComponent();
    }

    // A dark brown version of the card (used for "Perfect Attendance Days").
    private void ApplyDark(bool isDark)
    {
        if (!isDark) return;

        var brown = SidebarView.GetColor("BrandBrownDeep");
        CardBorder.Background = brown;
        CardBorder.Stroke = brown;
        TitleLabel.TextColor = Colors.White.WithAlpha(0.8f);
        ValueLabel.TextColor = Colors.White;
        SubtitleLabel.TextColor = Colors.White.WithAlpha(0.8f);
        IconLabel.TextColor = Colors.White;
    }
}
