using System.Globalization;

namespace LumoraAcademy.Controls;

// A time box that accepts both ways of entering a time:
//   - type it, for example 4:00 PM, 4pm or 16:00
//   - or click the small clock icon and pick it
//
// In a page you use it like this:
//     StartField.Time = new TimeSpan(16, 0, 0);
//     TimeSpan start = StartField.Time;
//     string forSaving = StartField.Text;      // "04:00 PM"
public partial class TimeField : ContentView
{
    // The ways the user is allowed to type a time.
    private static readonly string[] AllowedFormats =
    {
        "h:mm tt", "hh:mm tt", "h:mmtt", "hh:mmtt",
        "h tt", "hh tt", "htt", "hhtt",
        "H:mm", "HH:mm", "H.mm", "HH.mm"
    };

    // Raised when the time changes, either by typing or by picking.
    public event EventHandler? TimeSelected;

    private bool _updatingText;

    public TimeField()
    {
        InitializeComponent();
        ShowTime();
    }

    // ---------- The time itself ----------

    public static readonly BindableProperty TimeProperty =
        BindableProperty.Create(nameof(Time), typeof(TimeSpan), typeof(TimeField), new TimeSpan(9, 0, 0),
            propertyChanged: (b, o, n) => ((TimeField)b).ShowTime());

    public TimeSpan Time
    {
        get => (TimeSpan)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    // The time written the way it is stored on an event, e.g. "04:00 PM".
    public string Text => DateTime.Today.Add(Time).ToString("hh:mm tt", CultureInfo.InvariantCulture);

    // Sets the time from stored text such as "04:00 PM". Ignores anything unreadable.
    public void SetFromText(string? text)
    {
        if (TryRead(text ?? "", out TimeSpan parsed)) Time = parsed;
    }

    // ---------- Appearance ----------

    public static readonly BindableProperty BoxHeightProperty =
        BindableProperty.Create(nameof(BoxHeight), typeof(double), typeof(TimeField), 42.0,
            propertyChanged: (b, o, n) => ((TimeField)b).Box.HeightRequest = (double)n);

    public double BoxHeight
    {
        get => (double)GetValue(BoxHeightProperty);
        set => SetValue(BoxHeightProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(TimeField), 13.0,
            propertyChanged: (b, o, n) => ((TimeField)b).TextBox.FontSize = (double)n);

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    // ---------- Showing and reading the time ----------

    // Writes the current time into the text box and the clock.
    private void ShowTime()
    {
        _updatingText = true;

        TextBox.Text = Text;
        TextBox.TextColor = SidebarView.GetColor("TextHeading");

        if (Picker.Time != Time) Picker.Time = Time;

        _updatingText = false;
    }

    // Runs while the user is typing. A half-typed time is not an error yet,
    // so the text only turns red once it is clearly wrong.
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_updatingText) return;

        string text = e.NewTextValue ?? "";

        if (TryRead(text, out TimeSpan typed))
        {
            TextBox.TextColor = SidebarView.GetColor("TextHeading");

            if (typed != Time)
            {
                SetValue(TimeProperty, typed);
                Picker.Time = typed;
                TimeSelected?.Invoke(this, EventArgs.Empty);
            }
        }
        else if (text.Length >= 5)
        {
            TextBox.TextColor = SidebarView.GetColor("StatusRedText");
        }
    }

    // When the user clicks away, put the box back to a time we understand.
    private void OnTextUnfocused(object sender, FocusEventArgs e)
    {
        if (!TryRead(TextBox.Text ?? "", out _))
        {
            ShowTime();
        }
    }

    // TimePicker has no TimeSelected event, so we watch its Time property.
    private void OnPickerPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Picker.Time)) return;
        if (_updatingText || Picker.Time == Time) return;

        SetValue(TimeProperty, Picker.Time);
        ShowTime();
        TimeSelected?.Invoke(this, EventArgs.Empty);
    }

    // Tries to understand what the user typed.
    private static bool TryRead(string text, out TimeSpan time)
    {
        time = default;

        string cleaned = text.Trim().ToUpperInvariant().Replace(".", ":");
        if (cleaned.Length == 0) return false;

        if (DateTime.TryParseExact(cleaned, AllowedFormats, CultureInfo.InvariantCulture,
                                   DateTimeStyles.None, out DateTime exact))
        {
            time = exact.TimeOfDay;
            return true;
        }

        if (DateTime.TryParse(cleaned, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime loose))
        {
            time = loose.TimeOfDay;
            return true;
        }

        return false;
    }

    // ---------- Hover effect ----------

    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        Box.Stroke = SidebarView.GetColor("BrandPrimary");
        ClockIcon.TextColor = SidebarView.GetColor("BrandPrimary");
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        Box.Stroke = SidebarView.GetColor("FieldBorder");
        ClockIcon.TextColor = SidebarView.GetColor("TextMuted");
    }
}
