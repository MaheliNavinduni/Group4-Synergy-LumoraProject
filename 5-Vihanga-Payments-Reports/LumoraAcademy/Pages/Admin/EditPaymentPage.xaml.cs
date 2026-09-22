using LumoraAcademy.Core.Entities;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// Epic 3, User Story 1 - record a monthly payment (and grant extensions).
public partial class EditPaymentPage : ContentPage
{
    private readonly Payment? _payment;

    public EditPaymentPage(int paymentId)
    {
        InitializeComponent();

        ExtensionPicker.ItemsSource = new List<string> { "None", "1 month", "2 months", "3 months" };
        ExtensionPicker.SelectedIndex = 0;

        _payment = AppData.Payments.GetById(paymentId);
        if (_payment == null) return;

        SubtitleLabel.Text = $"{_payment.StudentName} • {_payment.StudentCode}";
        MonthLabel.Text = $"Fee month: {_payment.Month}  |  Due: {_payment.DueDate:MMM dd, yyyy}";
        NameEntry.Text = _payment.StudentName;
        StatusBadge.Text = _payment.Status;
        AmountDueEntry.Text = _payment.AmountDue.ToString("0.00");
        AmountPaidEntry.Text = _payment.AmountPaid.ToString("0.00");
        PaymentDatePicker.Date = _payment.PaymentDate ?? DateTime.Today;
        NotesEditor.Text = _payment.Remarks;
        ReceiptLabel.Text = string.IsNullOrWhiteSpace(_payment.ReceiptPath) ? "No receipt uploaded" : Path.GetFileName(_payment.ReceiptPath);

        if (_payment.ExtensionUntil.HasValue)
        {
            int months = ((_payment.ExtensionUntil.Value.Year - _payment.DueDate.Year) * 12) + _payment.ExtensionUntil.Value.Month - _payment.DueDate.Month;
            ExtensionPicker.SelectedIndex = Math.Clamp(months, 0, 3);
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AppNavigation.GoBackAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_payment == null) return;

        // Amounts must be real numbers (FR-05: amount and date are mandatory).
        if (!decimal.TryParse(AmountDueEntry.Text, out decimal amountDue) || !decimal.TryParse(AmountPaidEntry.Text, out decimal amountPaid))
        {
            await DisplayAlert("Edit Payment", "Please enter valid amounts.", "OK");
            return;
        }

        _payment.AmountDue = amountDue;
        _payment.AmountPaid = amountPaid;
        _payment.PaymentDate = amountPaid > 0 ? PaymentDatePicker.Date : null;
        _payment.Remarks = NotesEditor.Text ?? "";

        // Extension (FR-06): 0 = none, 1-3 months after the due date
        int months = ExtensionPicker.SelectedIndex;
        _payment.ExtensionUntil = months == 0 ? null : _payment.DueDate.AddMonths(months);

        try
        {
            AppData.Payments.Update(_payment);
            var saved = AppData.Payments.GetById(_payment.Id)!;
            await DisplayAlert("Edit Payment", $"Payment saved. Status is now: {saved.Status}.", "OK");
            await AppNavigation.GoBackAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Edit Payment", ex.Message, "OK");
        }
    }
}
