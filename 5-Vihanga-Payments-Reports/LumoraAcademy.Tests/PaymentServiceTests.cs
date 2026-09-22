using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Services;

namespace LumoraAcademy.Tests;

// Epic 3 - Payment Management.
public class PaymentServiceTests
{
    private static int StudentId(TestBackend t, string code) => t.Backend.Students.GetByStudentId(code)!.Id;

    // ----- US1: record payments -----

    [Fact]
    public void RecordPayment_UpdatesAmountPaidAndDate()
    {
        using var t = new TestBackend();
        var fee = t.Backend.Payments.CreateMonthlyFee(StudentId(t, "STU-8492"), "2026-10", 450, new DateTime(2026, 10, 15));

        var paid = t.Backend.Payments.RecordPayment(fee.Id, 200, new DateTime(2026, 10, 3), "Partial");

        Assert.Equal(200, paid.AmountPaid);
        Assert.Equal(250, paid.Outstanding);
        Assert.Equal(new DateTime(2026, 10, 3), paid.PaymentDate);
        Assert.Equal("Partial", paid.Remarks);
    }

    [Fact]
    public void RecordPayment_AmountMustBePositive()
    {
        using var t = new TestBackend();
        var fee = t.Backend.Payments.CreateMonthlyFee(StudentId(t, "STU-8492"), "2026-10", 450, new DateTime(2026, 10, 15));

        Assert.Throws<ArgumentException>(() => t.Backend.Payments.RecordPayment(fee.Id, 0, DateTime.Today));
        Assert.Throws<ArgumentException>(() => t.Backend.Payments.RecordPayment(fee.Id, 100, default));
    }

    [Fact]
    public void RecordPayment_IsLinkedToCorrectStudent()
    {
        using var t = new TestBackend();
        int marcus = StudentId(t, "STU-8492");
        var fee = t.Backend.Payments.CreateMonthlyFee(marcus, "2026-10", 450, new DateTime(2026, 10, 15));

        t.Backend.Payments.RecordPayment(fee.Id, 450, DateTime.Today);

        var history = t.Backend.Payments.GetForStudent(marcus);
        Assert.Single(history);
        Assert.Equal("Marcus Alvarez", history[0].StudentName);
    }

    // ----- US2: status and balances (FR-06) -----

    [Fact]
    public void Status_Paid_WhenFullyPaid()
    {
        var p = new Payment { AmountDue = 450, AmountPaid = 450, DueDate = new DateTime(2026, 1, 1) };
        Assert.Equal("Paid", PaymentService.StatusFor(p, "Active", new DateTime(2026, 6, 1)));
    }

    [Fact]
    public void Status_Pending_BeforeDueDate()
    {
        var p = new Payment { AmountDue = 450, AmountPaid = 0, DueDate = new DateTime(2026, 6, 30) };
        Assert.Equal("Pending", PaymentService.StatusFor(p, "Active", new DateTime(2026, 6, 1)));
    }

    [Fact]
    public void Status_Overdue_AfterDueDate()
    {
        var p = new Payment { AmountDue = 450, AmountPaid = 100, DueDate = new DateTime(2026, 5, 30) };
        Assert.Equal("Overdue", PaymentService.StatusFor(p, "Active", new DateTime(2026, 6, 1)));
    }

    [Fact]
    public void Status_ExtensionGranted_WhileExtensionIsValid()
    {
        var p = new Payment { AmountDue = 450, AmountPaid = 0, DueDate = new DateTime(2026, 5, 30), ExtensionUntil = new DateTime(2026, 7, 30) };
        Assert.Equal("Extension Granted", PaymentService.StatusFor(p, "Active", new DateTime(2026, 6, 15)));
        Assert.Equal("Overdue", PaymentService.StatusFor(p, "Active", new DateTime(2026, 8, 1)));
    }

    [Fact]
    public void Status_Dropout_WhenStudentLeftWithBalance()
    {
        var p = new Payment { AmountDue = 450, AmountPaid = 0, DueDate = new DateTime(2026, 1, 1) };
        Assert.Equal("Dropout", PaymentService.StatusFor(p, "Dropout", new DateTime(2026, 6, 1)));
    }

    [Fact]
    public void GrantExtension_MovesDeadlineBy1To3Months()
    {
        using var t = new TestBackend();
        var fee = t.Backend.Payments.CreateMonthlyFee(StudentId(t, "STU-8492"), "2026-10", 450, new DateTime(2026, 10, 15));

        t.Backend.Payments.GrantExtension(fee.Id, 2, "Approved by admin");

        var reloaded = t.Backend.Payments.GetById(fee.Id)!;
        Assert.Equal(new DateTime(2026, 12, 15), reloaded.ExtensionUntil);
        Assert.Throws<ArgumentException>(() => t.Backend.Payments.GrantExtension(fee.Id, 4));
    }

    [Fact]
    public void OutstandingForStudent_SumsAllUnpaidFees()
    {
        using var t = new TestBackend();
        int marcus = StudentId(t, "STU-8492");
        t.Backend.Payments.CreateMonthlyFee(marcus, "2026-09", 450, new DateTime(2026, 9, 15));
        var oct = t.Backend.Payments.CreateMonthlyFee(marcus, "2026-10", 450, new DateTime(2026, 10, 15));
        t.Backend.Payments.RecordPayment(oct.Id, 150, DateTime.Today);

        Assert.Equal(750, t.Backend.Payments.OutstandingForStudent(marcus));
    }

    [Fact]
    public void Summary_TotalsMatchIndividualPayments()
    {
        using var t = new TestBackend();

        var all = t.Backend.Payments.GetAll();
        var summary = t.Backend.Payments.GetSummary();

        Assert.Equal(all.Sum(p => p.AmountDue), summary.TotalExpected);
        Assert.True(summary.Collected <= summary.TotalExpected);
        Assert.Contains(all, p => p.Status == "Extension Granted");
    }
}
