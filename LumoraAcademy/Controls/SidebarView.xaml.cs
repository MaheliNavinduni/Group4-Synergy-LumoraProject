using LumoraAcademy.Data;
using LumoraAcademy.Services;

namespace LumoraAcademy.Controls;

public partial class SidebarView : ContentView
{
    // Which menu item should be highlighted. Set from each page's XAML, e.g. ActiveItem="Students".
    public static readonly BindableProperty ActiveItemProperty =
        BindableProperty.Create(nameof(ActiveItem), typeof(string), typeof(SidebarView), "", propertyChanged: OnActiveItemChanged);

    public string ActiveItem
    {
        get => (string)GetValue(ActiveItemProperty);
        set => SetValue(ActiveItemProperty, value);
    }

    public SidebarView()
    {
        InitializeComponent();
        BuildMenu();
    }

    private static void OnActiveItemChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((SidebarView)bindable).BuildMenu();
    }

    // Creates the menu items for the logged-in role.
    private void BuildMenu()
    {
        MenuList.Children.Clear();

        bool isAdmin = AppNavigation.CurrentRole == "Admin";

        if (isAdmin)
        {
            UserNameLabel.Text = string.IsNullOrEmpty(AppNavigation.CurrentUserName) ? "Admin" : AppNavigation.CurrentUserName;
            RoleLabel.Text = "Administrator";
            RoleLabel.IsVisible = true;

            AddMenuItem(SampleData.Icons.Dashboard, "Dashboard");
            AddMenuItem(SampleData.Icons.Students, "Students");
            AddMenuItem(SampleData.Icons.Teachers, "Teachers");
            AddMenuItem(SampleData.Icons.Payments, "Payments");
            AddMenuItem(SampleData.Icons.Academics, "Academics");
            AddMenuItem(SampleData.Icons.Attendance, "Attendance");
            AddMenuItem(SampleData.Icons.Events, "Upcoming Events");
        }
        else
        {
            UserNameLabel.Text = string.IsNullOrEmpty(AppNavigation.CurrentUserName) ? "Miss. Aries" : AppNavigation.CurrentUserName;
            RoleLabel.Text = "Teacher";
            RoleLabel.IsVisible = true;

            AddMenuItem(SampleData.Icons.Dashboard, "Dashboard");
            AddMenuItem(SampleData.Icons.Subjects, "My Subjects");
            AddMenuItem(SampleData.Icons.Attendance, "Attendance");
            AddMenuItem(SampleData.Icons.Students, "Students");
            AddMenuItem(SampleData.Icons.Events, "Upcoming Events");
        }
    }

    // Builds one row of the menu: a small icon and the item name.
    private void AddMenuItem(string iconGlyph, string title)
    {
        bool isActive = title == ActiveItem;

        // The menu sits on the dark brown panel, so the wording is cream and
        // the item you are on is a cream pill with dark brown wording.
        var cream = GetColor("BrandCream");
        var menuText = GetColor("SidebarText");
        var activeText = GetColor("Brown800");
        var gold = GetColor("AccentGold");

        var icon = new Label
        {
            Text = iconGlyph,
            FontFamily = "Segoe MDL2 Assets",
            FontSize = 14,
            TextColor = isActive ? activeText : menuText,
            VerticalOptions = LayoutOptions.Center,
        };

        var text = new Label
        {
            Text = title,
            FontSize = 13,
            FontAttributes = isActive ? FontAttributes.Bold : FontAttributes.None,
            TextColor = isActive ? activeText : menuText,
            VerticalOptions = LayoutOptions.Center,
        };

        var row = new HorizontalStackLayout { Spacing = 12, Padding = new Thickness(12, 10) };
        row.Children.Add(icon);
        row.Children.Add(text);

        // The active item gets a cream pill with a gold bar down its left edge.
        var container = new Grid { ColumnDefinitions = { new ColumnDefinition(4), new ColumnDefinition(GridLength.Star) } };
        container.Add(new BoxView { Color = isActive ? gold : Colors.Transparent }, 0, 0);
        container.Add(row, 1, 0);

        var border = new Border
        {
            Background = isActive ? cream : Colors.Transparent,
            StrokeThickness = 0,
            Padding = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            Content = container,
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += async (s, e) =>
        {
            if (!isActive)
            {
                await AppNavigation.GoToMenuItemAsync(title);
            }
        };
        border.GestureRecognizers.Add(tap);

        // Hover effect: the item the mouse is over lights up, so the menu
        // feels like something you can click rather than a list of words.
        if (!isActive)
        {
            var hover = new PointerGestureRecognizer();

            hover.PointerEntered += (s, e) =>
            {
                border.Background = GetColor("SidebarHover");
                icon.TextColor = gold;
                text.TextColor = cream;
            };

            hover.PointerExited += (s, e) =>
            {
                border.Background = Colors.Transparent;
                icon.TextColor = menuText;
                text.TextColor = menuText;
            };

            border.GestureRecognizers.Add(hover);
        }

        MenuList.Children.Add(border);
    }

    // Reads a colour from Brand.xaml by its key.
    public static Color GetColor(string key)
    {
        if (Application.Current!.Resources.TryGetValue(key, out var value) && value is Color color)
        {
            return color;
        }
        return Colors.Black;
    }

    private async void OnLogoutTapped(object sender, EventArgs e)
    {
        await AppNavigation.LogoutAsync();
    }

    private void OnLogoutPointerEntered(object sender, PointerEventArgs e)
    {
        LogoutBox.BackgroundColor = GetColor("SidebarHover");
        LogoutIcon.TextColor = GetColor("AccentGold");
        LogoutLabel.TextColor = GetColor("BrandCream");
    }

    private void OnLogoutPointerExited(object sender, PointerEventArgs e)
    {
        LogoutBox.BackgroundColor = Colors.Transparent;
        LogoutIcon.TextColor = GetColor("SidebarText");
        LogoutLabel.TextColor = GetColor("SidebarText");
    }
}
