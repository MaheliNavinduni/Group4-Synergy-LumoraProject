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

    // Which of the five brown shades this card uses: 1 is the darkest, 5 the lightest.
    public static readonly BindableProperty AccentProperty =
        BindableProperty.Create(nameof(Accent), typeof(int), typeof(StatCard), 3, propertyChanged: (b, o, n) => ((StatCard)b).ApplyAccent((int)n));

    public int Accent { get => (int)GetValue(AccentProperty); set => SetValue(AccentProperty, value); }

    public StatCard()
    {
        InitializeComponent();
        ApplyAccent(Accent);
    }

    // Paints the strip along the top and the icon chip in one of the five
    // brown shades from Brand.xaml, so a row of cards looks like a set.
    private void ApplyAccent(int accent)
    {
        int shade = Math.Clamp(accent, 1, 5);

        Color strong = SidebarView.GetColor("Chart" + shade);

        // The chip behind the icon is a light tint of the same shade.
        Color chip = shade switch
        {
            1 => SidebarView.GetColor("Brown100"),
            2 => SidebarView.GetColor("Brown100"),
            3 => SidebarView.GetColor("Brown50"),
            4 => SidebarView.GetColor("Brown50"),
            _ => SidebarView.GetColor("AccentGoldSoft"),
        };

        AccentStrip.Color = strong;
        IconChip.Background = chip;

        // Shade 5 is pale, so the icon on it uses a darker brown to stay readable.
        IconLabel.TextColor = shade >= 4 ? SidebarView.GetColor("Brown700") : strong;
    }

    // A dark brown version of the card (used for "Perfect Attendance Days").
    private void ApplyDark(bool isDark)
    {
        if (!isDark) return;

        var brown = SidebarView.GetColor("Brown800");
        CardBorder.Background = brown;
        CardBorder.Stroke = brown;
        AccentStrip.Color = SidebarView.GetColor("AccentGold");
        TitleLabel.TextColor = Colors.White.WithAlpha(0.8f);
        ValueLabel.TextColor = Colors.White;
        SubtitleLabel.TextColor = Colors.White.WithAlpha(0.8f);
        IconChip.Background = Colors.White.WithAlpha(0.14f);
        IconLabel.TextColor = SidebarView.GetColor("AccentGold");
    }
}
