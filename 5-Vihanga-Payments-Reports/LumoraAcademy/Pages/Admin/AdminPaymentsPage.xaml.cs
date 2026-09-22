using LumoraAcademy.Core.Entities;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// Epic 3, User Story 2 - track payment status and outstanding balances.
public partial class AdminPaymentsPage : ContentPage
{
    public AdminPaymentsPage()
    {
        InitializeComponent();

        LoadPayments();
        Loaded += (s, e) => LoadPayments();
    }

    private void LoadPayments()
    {
        List<Payment> payments = AppData.Payments.GetAll();
        BindableLayout.SetItemsSource(RowList, Pager.Page(payments));
        CountLabel.Text = Pager.RangeText("fee records");

        var summary = AppData.Payments.GetSummary();
        ExpectedCard.Value = summary.TotalExpected.ToString("$#,##0");
        CollectedCard.Value = summary.Collected.ToString("$#,##0");
        CollectedCard.Subtitle = $"{summary.CollectedPercent}% of total";
        PendingCard.Value = summary.Pending.ToString("$#,##0");
        PendingCard.Subtitle = $"{summary.PendingCount} records";
        OverdueCard.Value = summary.Overdue.ToString("$#,##0");
    }

    private void OnPageChanged(object sender, EventArgs e)
    {
        LoadPayments();
    }

    // "Edit Payment" button at the top: pick which student's fee to edit.
    private async void OnEditPaymentClicked(object sender, EventArgs e)
    {
        var payments = AppData.Payments.GetAll();
        string[] names = payments.Select(p => $"{p.StudentName} - {p.Month} ({p.Status})").ToArray();
        if (names.Length == 0) return;

        string choice = await DisplayActionSheet("Select a fee record", "Cancel", null, names);
        int index = Array.IndexOf(names, choice);
        if (index >= 0)
        {
            await AppNavigation.GoToAsync(new EditPaymentPage(payments[index].Id));
        }
    }

    // "Edit" link on a row.
    private async void OnRowActionTapped(object sender, EventArgs e)
    {
        if (sender is Label label && label.BindingContext is Payment payment)
        {
            await AppNavigation.GoToAsync(new EditPaymentPage(payment.Id));
        }
    }
}
