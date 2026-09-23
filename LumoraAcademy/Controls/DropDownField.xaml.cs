namespace LumoraAcademy.Controls;

// A drop-down that works the same way everywhere in the system.
// It is used instead of the plain Picker so that every drop-down
// has a visible arrow, a hover effect and the same look.
//
// In a page you use it like this:
//     GradeField.ItemsSource = new List<string> { "10th Grade", "11th Grade" };
//     GradeField.SelectedIndex = 0;
//     string grade = GradeField.SelectedItem ?? "";
public partial class DropDownField : ContentView
{
    // Raised after the user picks a different choice.
    public event EventHandler? SelectedIndexChanged;

    private IList<string> _items = new List<string>();
    private int _selectedIndex = -1;

    public DropDownField()
    {
        InitializeComponent();
        ShowSelection();
    }

    // ---------- The text shown when nothing is chosen yet ----------

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(DropDownField), "Select",
            propertyChanged: (b, o, n) => ((DropDownField)b).ShowSelection());

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    // ---------- Text size, so small filter boxes can shrink it ----------

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(DropDownField), 13.0,
            propertyChanged: (b, o, n) => ((DropDownField)b).ValueLabel.FontSize = (double)n);

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    // ---------- Box height, so it can match the other fields on a page ----------

    public static readonly BindableProperty BoxHeightProperty =
        BindableProperty.Create(nameof(BoxHeight), typeof(double), typeof(DropDownField), 42.0,
            propertyChanged: (b, o, n) => ((DropDownField)b).Box.HeightRequest = (double)n);

    public double BoxHeight
    {
        get => (double)GetValue(BoxHeightProperty);
        set => SetValue(BoxHeightProperty, value);
    }

    // ---------- The list of choices ----------

    public IList<string> ItemsSource
    {
        get => _items;
        set
        {
            _items = value ?? new List<string>();
            _selectedIndex = -1;
            ShowSelection();
        }
    }

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            int safe = (value >= 0 && value < _items.Count) ? value : -1;
            if (safe == _selectedIndex) return;

            _selectedIndex = safe;
            ShowSelection();
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public string? SelectedItem
    {
        get => (_selectedIndex >= 0 && _selectedIndex < _items.Count) ? _items[_selectedIndex] : null;
        set
        {
            int index = value == null ? -1 : _items.IndexOf(value);
            SelectedIndex = index;
        }
    }

    // ---------- What happens on screen ----------

    // Shows the chosen value, or the title in grey when nothing is chosen.
    private void ShowSelection()
    {
        string? chosen = SelectedItem;

        if (string.IsNullOrEmpty(chosen))
        {
            ValueLabel.Text = Title;
            ValueLabel.TextColor = SidebarView.GetColor("TextLight");
        }
        else
        {
            ValueLabel.Text = chosen;
            ValueLabel.TextColor = SidebarView.GetColor("TextHeading");
        }
    }

    // Opens the list of choices.
    private async void OnTapped(object sender, EventArgs e)
    {
        if (!IsEnabled || _items.Count == 0) return;

        var page = Application.Current?.MainPage;
        if (page == null) return;

        // Blank entries would show as an empty button, so they are given a name.
        string[] choices = _items.Select(i => string.IsNullOrWhiteSpace(i) ? "(none)" : i).ToArray();

        string chosen = await page.DisplayActionSheet(Title, "Cancel", null, choices);

        if (string.IsNullOrEmpty(chosen) || chosen == "Cancel") return;

        int index = Array.IndexOf(choices, chosen);
        if (index >= 0) SelectedIndex = index;
    }

    // ---------- Hover effect ----------

    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        Box.Stroke = SidebarView.GetColor("BrandPrimary");
        Box.Background = SidebarView.GetColor("RowHover");
        Arrow.TextColor = SidebarView.GetColor("BrandPrimary");
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        Box.Stroke = SidebarView.GetColor("FieldBorder");
        Box.Background = SidebarView.GetColor("CardBackground");
        Arrow.TextColor = SidebarView.GetColor("TextMuted");
    }
}
