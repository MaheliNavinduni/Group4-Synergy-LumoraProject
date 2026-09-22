namespace LumoraAcademy.Controls;

// Splits a long list into pages.
//
// In XAML:   <controls:PagerView x:Name="Pager" PageChanged="OnPageChanged" />
// In code:   var rows = Pager.Page(allRows);        // rows for the current page
//            private void OnPageChanged(...) => Reload();
public partial class PagerView : ContentView
{
    public int PageSize { get; set; } = 10;
    public int CurrentPage { get; private set; } = 1;
    public int TotalItems { get; private set; }

    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalItems / (double)PageSize));

    // Raised when the user clicks Prev or Next. The page should reload its rows.
    public event EventHandler? PageChanged;

    public PagerView()
    {
        InitializeComponent();
        Refresh();
    }

    // Returns only the items that belong on the current page and updates the label.
    public List<T> Page<T>(List<T> allItems)
    {
        TotalItems = allItems.Count;
        if (CurrentPage > TotalPages) CurrentPage = TotalPages;

        Refresh();
        return allItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
    }

    // "Showing 11 to 20 of 45" - handy for the label next to the pager.
    public string RangeText(string noun)
    {
        if (TotalItems == 0) return $"No {noun} found";
        int first = (CurrentPage - 1) * PageSize + 1;
        int last = Math.Min(CurrentPage * PageSize, TotalItems);
        return $"Showing {first} to {last} of {TotalItems} {noun}";
    }

    // Go back to page 1 (e.g. after a new search).
    public void Reset()
    {
        CurrentPage = 1;
    }

    private void OnPrevClicked(object sender, EventArgs e)
    {
        if (CurrentPage <= 1) return;
        CurrentPage--;
        PageChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnNextClicked(object sender, EventArgs e)
    {
        if (CurrentPage >= TotalPages) return;
        CurrentPage++;
        PageChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Refresh()
    {
        PageLabel.Text = $"Page {CurrentPage} of {TotalPages}";
        PrevButton.IsEnabled = CurrentPage > 1;
        NextButton.IsEnabled = CurrentPage < TotalPages;
        PrevButton.Opacity = PrevButton.IsEnabled ? 1 : 0.4;
        NextButton.Opacity = NextButton.IsEnabled ? 1 : 0.4;
    }
}
