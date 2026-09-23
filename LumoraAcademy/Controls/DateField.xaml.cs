using System.Globalization;

namespace LumoraAcademy.Controls;

// A date box that accepts both ways of entering a date:
//   - type it, for example 25/09/2010
//   - or click the small calendar icon and pick it
//
// In a page you use it like this:
//     DobField.Date = new DateTime(2010, 9, 25);
//     DateTime dob = DobField.Date;
public partial class DateField : ContentView
{
    // The date formats the user is allowed to type.
    private static readonly string[] AllowedFormats =
    {
        "dd/MM/yyyy", "d/M/yyyy",
        "dd-MM-yyyy", "d-M-yyyy",
        "yyyy-MM-dd", "dd.MM.yyyy"
    };

    // Raised when the date changes, either by typing or by picking.
    public event EventHandler? DateSelected;

    private bool _updatingText;

    public DateField()
    {
        InitializeComponent();
        ShowDate();
    }

    // ---------- The date itself ----------

    public static readonly BindableProperty DateProperty =
        BindableProperty.Create(nameof(Date), typeof(DateTime), typeof(DateField), DateTime.Today,
            propertyChanged: (b, o, n) => ((DateField)b).ShowDate());

    public DateTime Date
    {
        get => (DateTime)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }

    public DateTime MinimumDate
    {
        get => Picker.MinimumDate;
        set => Picker.MinimumDate = value;
    }

    public DateTime MaximumDate
    {
        get => Picker.MaximumDate;
        set => Picker.MaximumDate = value;
    }

    // ---------- Appearance ----------

    public static readonly BindableProperty BoxHeightProperty =
        BindableProperty.Create(nameof(BoxHeight), typeof(double), typeof(DateField), 42.0,
            propertyChanged: (b, o, n) => ((DateField)b).Box.HeightRequest = (double)n);

    public double BoxHeight
    {
        get => (double)GetValue(BoxHeightProperty);
        set => SetValue(BoxHeightProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(DateField), 13.0,
            propertyChanged: (b, o, n) => ((DateField)b).TextBox.FontSize = (double)n);

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    // ---------- Showing and reading the date ----------

    // Writes the current date into the text box and the calendar.
    private void ShowDate()
    {
        _updatingText = true;

        TextBox.Text = Date.ToString("dd/MM/yyyy");
        TextBox.TextColor = SidebarView.GetColor("TextHeading");

        if (Date >= Picker.MinimumDate && Date <= Picker.MaximumDate)
        {
            Picker.Date = Date;
        }

        _updatingText = false;
    }

    // Runs while the user is typing. A half-typed date is not an error yet,
    // so we only turn the text red once it is clearly wrong.
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_updatingText) return;

        string text = e.NewTextValue ?? "";

        if (TryRead(text, out DateTime typed))
        {
            TextBox.TextColor = SidebarView.GetColor("TextHeading");

            if (typed.Date != Date.Date)
            {
                SetValue(DateProperty, typed);
                Picker.Date = typed;
                DateSelected?.Invoke(this, EventArgs.Empty);
            }
        }
        else if (text.Length >= 10)
        {
            TextBox.TextColor = SidebarView.GetColor("StatusRedText");
        }
    }

    // When the user clicks away, put the box back to a date we understand.
    private void OnTextUnfocused(object sender, FocusEventArgs e)
    {
        if (!TryRead(TextBox.Text ?? "", out _))
        {
            ShowDate();
        }
    }

    private void OnDatePicked(object sender, DateChangedEventArgs e)
    {
        SetValue(DateProperty, e.NewDate);
        ShowDate();
        DateSelected?.Invoke(this, EventArgs.Empty);
    }

    // Tries to understand what the user typed.
    private static bool TryRead(string text, out DateTime date)
    {
        return DateTime.TryParseExact(text.Trim(), AllowedFormats,
            CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }

    // ---------- Hover effect ----------

    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        Box.Stroke = SidebarView.GetColor("BrandPrimary");
        CalendarIcon.TextColor = SidebarView.GetColor("BrandPrimary");
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        Box.Stroke = SidebarView.GetColor("FieldBorder");
        CalendarIcon.TextColor = SidebarView.GetColor("TextMuted");
    }
}
