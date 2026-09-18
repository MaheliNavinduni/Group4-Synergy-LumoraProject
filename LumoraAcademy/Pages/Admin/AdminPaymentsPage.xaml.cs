using LumoraAcademy.Data;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

public partial class AdminPaymentsPage : ContentPage
{
    public AdminPaymentsPage()
    {
        InitializeComponent();

        BindableLayout.SetItemsSource(RowList, SampleData.PaymentAccounts);
    }

    private async void OnEditPaymentClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new EditPaymentPage());
    }

    private async void OnRowActionTapped(object sender, EventArgs e)
    {
        await AppNavigation.GoToAsync(new EditPaymentPage());
    }
}
