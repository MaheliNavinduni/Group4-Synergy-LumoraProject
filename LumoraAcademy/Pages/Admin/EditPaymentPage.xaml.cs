using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class EditPaymentPage : ContentPage
{
    public EditPaymentPage()
    {
        InitializeComponent();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Make sure the amounts are real numbers before saving.
        if (!decimal.TryParse(AmountDueEntry.Text, out _) || !decimal.TryParse(AmountPaidEntry.Text, out _))
        {
            await DisplayAlert("Edit Payment", "Please enter valid amounts.", "OK");
            return;
        }

        // TODO: update the payment record in the database.
        await DisplayAlert("Edit Payment", "Payment saved (demo only, not stored yet).", "OK");
        await AppNavigation.GoBackAsync();
    }
}
