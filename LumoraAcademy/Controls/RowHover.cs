namespace LumoraAcademy.Controls;

// Highlights a table row when the mouse is over it, so the user can see
// that the row can be clicked.
//
// Use it in XAML on any layout that has a TapGestureRecognizer:
//     <Grid controls:RowHover.Enabled="True" ...>
//
// Writing it once here means every table in the system behaves the same way,
// instead of each page repeating the same two event handlers.
public static class RowHover
{
    public static readonly BindableProperty EnabledProperty =
        BindableProperty.CreateAttached(
            "Enabled",
            typeof(bool),
            typeof(RowHover),
            false,
            propertyChanged: OnEnabledChanged);

    public static bool GetEnabled(BindableObject view) => (bool)view.GetValue(EnabledProperty);

    public static void SetEnabled(BindableObject view, bool value) => view.SetValue(EnabledProperty, value);

    private static void OnEnabledChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not View view) return;
        if (newValue is not bool enabled || !enabled) return;

        var hover = new PointerGestureRecognizer();

        hover.PointerEntered += (s, e) => view.BackgroundColor = SidebarView.GetColor("RowHover");
        hover.PointerExited += (s, e) => view.BackgroundColor = Colors.Transparent;

        view.GestureRecognizers.Add(hover);
    }
}
