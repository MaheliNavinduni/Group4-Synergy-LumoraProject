using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Services;
using LumoraAcademy.Services;

namespace LumoraAcademy.Pages.Admin;

// Epic 3 - the fee register. All fees are cash, taken at the office.
// Pick a class and a month, then tick off the students who have paid.
public partial class AdminPaymentsPage : ContentPage
{
    private readonly List<string> _months = new();
    private ClassGroup? _class;

    public AdminPaymentsPage()
    {
        InitializeComponent();

        // The last 12 months, newest first.
        for (int i = 0; i < 12; i++)
        {
            _months.Add(PaymentService.MonthKey(DateTime.Today.AddMonths(-i)));
        }
        MonthPicker.ItemsSource = _months.Select(m => DateTime.Parse(m + "-01").ToString("MMMM yyyy")).ToList();
        MonthPicker.SelectedIndex = 0;

        LoadClasses();
    }

    private string SelectedMonth => _months[Math.Max(0, MonthPicker.SelectedIndex)];

    // ---------- Step 1: the classes ----------

    private void LoadClasses()
    {
        var rows = new List<ClassFeeRow>();

        foreach (var c in AppData.Classes.GetAll())
        {
            var register = AppData.Payments.GetRegister(c.Id, SelectedMonth);
            rows.Add(new ClassFeeRow
            {
                ClassGroupId = c.Id,
                ClassName = c.Name,
                TeacherName = c.TeacherName,
                FeeText = c.MonthlyFee.ToString("N2"),
                StudentCount = c.StudentCount,
                PaidCount = register.Count(p => p.Status == "Paid"),
                Collected = register.Sum(p => Math.Min(p.AmountPaid, p.AmountDue)),
            });
        }

        BindableLayout.SetItemsSource(ClassList, rows);
        NoClassesLabel.IsVisible = rows.Count == 0;

        LoadTotals();
    }

    private void LoadTotals()
    {
        var summary = AppData.Payments.GetSummary(SelectedMonth);
        ExpectedCard.Value = summary.TotalExpected.ToString("N2");
        CollectedCard.Value = summary.Collected.ToString("N2");
        CollectedCard.Subtitle = $"{summary.CollectedPercent}% of expected";
        OutstandingCard.Value = summary.Pending.ToString("N2");
        OutstandingCard.Subtitle = $"{summary.UnpaidCount} students";
        OverdueCard.Value = summary.Overdue.ToString("N2");
    }

    private void OnMonthChanged(object sender, EventArgs e)
    {
        if (_class == null)
        {
            LoadClasses();
        }
        else
        {
            LoadRegister();
            LoadTotals();
        }
    }

    private void OnClassTapped(object sender, EventArgs e)
    {
        if (sender is Grid row && row.BindingContext is ClassFeeRow classRow)
        {
            _class = AppData.Classes.GetById(classRow.ClassGroupId);
            Pager.Reset();
            LoadRegister();

            ClassListCard.IsVisible = false;
            RegisterSection.IsVisible = true;
        }
    }

    private void OnBackToClassesClicked(object sender, EventArgs e)
    {
        _class = null;
        RegisterSection.IsVisible = false;
        ClassListCard.IsVisible = true;
        LoadClasses();
    }

    // ---------- Step 2: the register ----------

    private void LoadRegister()
    {
        if (_class == null) return;

        var register = AppData.Payments.GetRegister(_class.Id, SelectedMonth);

        RegisterTitle.Text = _class.Name;
        RegisterSubtitle.Text = $"{DateTime.Parse(SelectedMonth + "-01"):MMMM yyyy} • Fee {_class.MonthlyFee:N2} • {register.Count(p => p.Status == "Paid")} of {register.Count} paid";

        BindableLayout.SetItemsSource(RowList, Pager.Page(register));
        CountLabel.Text = Pager.RangeText("students");
        NoStudentsLabel.IsVisible = register.Count == 0;
    }

    private void OnPageChanged(object sender, EventArgs e) => LoadRegister();

    private void OnMarkPaidClicked(object sender, EventArgs e)
    {
        if (sender is Button b && b.BindingContext is Payment payment)
        {
            AppData.Payments.MarkPaid(payment.Id);
            LoadRegister();
            LoadTotals();
        }
    }

    private async void OnUndoClicked(object sender, EventArgs e)
    {
        if (sender is not Button b || b.BindingContext is not Payment payment) return;

        bool confirm = await DisplayAlert("Undo Payment", $"Mark {payment.StudentName} as not paid for {payment.MonthText}?", "Undo", "Cancel");
        if (!confirm) return;

        AppData.Payments.MarkUnpaid(payment.Id);
        LoadRegister();
        LoadTotals();
    }

    // Part payment, extension or a note - the less common actions.
    private async void OnMoreClicked(object sender, EventArgs e)
    {
        if (sender is not Button b || b.BindingContext is not Payment payment) return;

        string choice = await DisplayActionSheet($"{payment.StudentName} - {payment.MonthText}", "Cancel", null,
            "Record part payment", "Grant extension", "Add a note");

        switch (choice)
        {
            case "Record part payment":
                await RecordPartPaymentAsync(payment);
                break;
            case "Grant extension":
                await GrantExtensionAsync(payment);
                break;
            case "Add a note":
                await AddNoteAsync(payment);
                break;
        }
    }

    private async Task RecordPartPaymentAsync(Payment payment)
    {
        string entered = await DisplayPromptAsync("Part Payment",
            $"How much did {payment.StudentName} pay? (Fee {payment.AmountDue:N2}, still owing {payment.Outstanding:N2})",
            "Save", "Cancel", "0.00", keyboard: Keyboard.Numeric);

        if (string.IsNullOrWhiteSpace(entered)) return;

        if (!decimal.TryParse(entered, out decimal amount))
        {
            await DisplayAlert("Part Payment", "Please enter a number.", "OK");
            return;
        }

        try
        {
            AppData.Payments.RecordPayment(payment.Id, amount, DateTime.Today);
            LoadRegister();
            LoadTotals();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Part Payment", ex.Message, "OK");
        }
    }

    private async Task GrantExtensionAsync(Payment payment)
    {
        string choice = await DisplayActionSheet("Extend the deadline by", "Cancel", null, "1 month", "2 months", "3 months");
        if (choice is null or "Cancel") return;

        int months = int.Parse(choice.Substring(0, 1));
        AppData.Payments.GrantExtension(payment.Id, months, $"Extension of {months} month(s) approved by Admin.");

        LoadRegister();
        await DisplayAlert("Extension", $"Deadline extended by {months} month(s).", "OK");
    }

    private async Task AddNoteAsync(Payment payment)
    {
        string note = await DisplayPromptAsync("Note", "Add a note for this fee record:", "Save", "Cancel", payment.Remarks);
        if (note == null) return;

        payment.Remarks = note;
        AppData.Payments.Update(payment);
        LoadRegister();
    }
}

// One line on the class list of the fee register.
public class ClassFeeRow
{
    public int ClassGroupId { get; set; }
    public string ClassName { get; set; } = "";
    public string TeacherName { get; set; } = "";
    public string FeeText { get; set; } = "";
    public int StudentCount { get; set; }
    public int PaidCount { get; set; }
    public decimal Collected { get; set; }

    public string PaidText => $"{PaidCount} / {StudentCount}";
    public string CollectedText => Collected.ToString("N2");
}
