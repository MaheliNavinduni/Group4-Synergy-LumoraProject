namespace LumoraAcademy.Controls;

public partial class AvatarView : ContentView
{
    // Usage:  <controls:AvatarView Initials="MA" Size="40" />
    public static readonly BindableProperty InitialsProperty =
        BindableProperty.Create(nameof(Initials), typeof(string), typeof(AvatarView), "", propertyChanged: (b, o, n) => ((AvatarView)b).InitialsLabel.Text = (string)n);

    public static readonly BindableProperty SizeProperty =
        BindableProperty.Create(nameof(Size), typeof(double), typeof(AvatarView), 32.0, propertyChanged: (b, o, n) => ((AvatarView)b).ApplySize((double)n));

    // Full path of a photo file. When set (and the file exists) the photo replaces the initials.
    public static readonly BindableProperty ImagePathProperty =
        BindableProperty.Create(nameof(ImagePath), typeof(string), typeof(AvatarView), "", propertyChanged: (b, o, n) => ((AvatarView)b).ApplyImage((string)n));

    public string ImagePath { get => (string)GetValue(ImagePathProperty); set => SetValue(ImagePathProperty, value); }

    private void ApplyImage(string path)
    {
        bool hasPhoto = !string.IsNullOrWhiteSpace(path) && File.Exists(path);
        Photo.Source = hasPhoto ? ImageSource.FromFile(path) : null;
        Photo.IsVisible = hasPhoto;
        InitialsLabel.IsVisible = !hasPhoto;
    }

    public string Initials { get => (string)GetValue(InitialsProperty); set => SetValue(InitialsProperty, value); }
    public double Size { get => (double)GetValue(SizeProperty); set => SetValue(SizeProperty, value); }

    public AvatarView()
    {
        InitializeComponent();
    }

    private void ApplySize(double size)
    {
        Circle.WidthRequest = size;
        Circle.HeightRequest = size;
        InitialsLabel.FontSize = size / 2.8;
    }
}
