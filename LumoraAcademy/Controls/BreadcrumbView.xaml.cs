using LumoraAcademy.Services;

namespace LumoraAcademy.Controls;

// Shows where you are and lets you click your way back, for example:
//
//     Teachers  >  Dr. Richards
//
// In a page you use it like this:
//     <controls:BreadcrumbView Section="Teachers" Current="Dr. Richards" />
//
// and for a page three levels deep:
//     <controls:BreadcrumbView Section="Academics" Parent="Classes" Current="Manage Class" />
//
// Clicking the Section takes you back to that section's main page, closing any
// pages opened in between. Clicking the Parent goes back one page.
public partial class BreadcrumbView : ContentView
{
    public BreadcrumbView()
    {
        InitializeComponent();
        Build();
    }

    // The sidebar section this page belongs to, e.g. "Teachers".
    public static readonly BindableProperty SectionProperty =
        BindableProperty.Create(nameof(Section), typeof(string), typeof(BreadcrumbView), "",
            propertyChanged: (b, o, n) => ((BreadcrumbView)b).Build());

    // An optional middle step, for pages that are three levels deep.
    public static readonly BindableProperty ParentProperty =
        BindableProperty.Create(nameof(Parent), typeof(string), typeof(BreadcrumbView), "",
            propertyChanged: (b, o, n) => ((BreadcrumbView)b).Build());

    // The page you are on now. This one is not clickable.
    public static readonly BindableProperty CurrentProperty =
        BindableProperty.Create(nameof(Current), typeof(string), typeof(BreadcrumbView), "",
            propertyChanged: (b, o, n) => ((BreadcrumbView)b).Build());

    public string Section
    {
        get => (string)GetValue(SectionProperty);
        set => SetValue(SectionProperty, value);
    }

    public new string Parent
    {
        get => (string)GetValue(ParentProperty);
        set => SetValue(ParentProperty, value);
    }

    public string Current
    {
        get => (string)GetValue(CurrentProperty);
        set => SetValue(CurrentProperty, value);
    }

    private void Build()
    {
        Trail.Children.Clear();

        if (!string.IsNullOrWhiteSpace(Section))
        {
            AddCrumb(Section, async () => await AppNavigation.GoToSectionAsync(Section));
        }

        if (!string.IsNullOrWhiteSpace(Parent))
        {
            AddSeparator();
            AddCrumb(Parent, async () => await AppNavigation.GoBackAsync());
        }

        if (!string.IsNullOrWhiteSpace(Current))
        {
            AddSeparator();
            Trail.Children.Add(new Label
            {
                Text = Current,
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                TextColor = SidebarView.GetColor("TextHeading"),
                VerticalOptions = LayoutOptions.Center,
            });
        }
    }

    private void AddSeparator()
    {
        Trail.Children.Add(new Label
        {
            Text = "",                       // a small chevron
            FontFamily = "Segoe MDL2 Assets",
            FontSize = 9,
            TextColor = SidebarView.GetColor("TextLight"),
            VerticalOptions = LayoutOptions.Center,
        });
    }

    // One clickable step, which underlines and turns brown when hovered.
    private void AddCrumb(string text, Func<Task> go)
    {
        var label = new Label
        {
            Text = text,
            FontSize = 12,
            TextColor = SidebarView.GetColor("TextMuted"),
            VerticalOptions = LayoutOptions.Center,
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += async (s, e) => await go();
        label.GestureRecognizers.Add(tap);

        var hover = new PointerGestureRecognizer();
        hover.PointerEntered += (s, e) =>
        {
            label.TextColor = SidebarView.GetColor("BrandPrimary");
            label.TextDecorations = TextDecorations.Underline;
        };
        hover.PointerExited += (s, e) =>
        {
            label.TextColor = SidebarView.GetColor("TextMuted");
            label.TextDecorations = TextDecorations.None;
        };
        label.GestureRecognizers.Add(hover);

        Trail.Children.Add(label);
    }
}
