using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Services;

namespace LumoraAcademy.Tests;

// Epic 3 - the fee register. All payments are cash, recorded per class per month.
public class PaymentServiceTests
{
    private static ClassGroup ScienceG10(TestBackend t) =>
        t.Backend.Classes.GetAll().First(c => c.SubjectName == "Science" && c.Grade == "10th Grade");

    private static string ThisMonth => DateTime.Today.ToString("yyyy-MM");

    // ----- the register -----

    [Fact]
    public void GetRegister_HasOneRowPerEnrolledStudentWithTheClassFee()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);

        var register = t.Backend.Payments.GetRegister(science.Id, ThisMonth);

        Assert.Equal(4, register.Count);
        Assert.All(register, p => Assert.Equal(2500, p.AmountDue));
        Assert.All(register, p => Assert.Equal("Science - 10th Grade", p.ClassName));
    }

    [Fact]
    public void GetRegister_CreatesMissingRowsOnceOnly()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);
        string month = "2027-01";

        var first = t.Backend.Payments.GetRegister(science.Id, month);
        var second = t.Backend.Payments.GetRegister(science.Id, month);

        Assert.Equal(first.Count, second.Count);
        Assert.Equal(first.Select(p => p.Id).OrderBy(x => x), second.Select(p => p.Id).OrderBy(x => x));
    }

    [Fact]
    public void GetRegister_ShowsTheSeededPaidAndUnpaidStudents()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);

        var register = t.Backend.Payments.GetRegister(science.Id, ThisMonth);

        Assert.Contains(register, p => p.StudentName == "Emma Stone" && p.Status == "Paid");
        Assert.Contains(register, p => p.StudentName == "Mia Patel" && p.Status != "Paid");
    }

    // ----- marking paid (the main action) -----

    [Fact]
    public void MarkPaid_SetsTheFullFeeTodayAndGivesAReceiptNumber()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);
        var row = t.Backend.Payments.GetRegister(science.Id, ThisMonth).First(p => p.Status != "Paid");

        var paid = t.Backend.Payments.MarkPaid(row.Id);

        Assert.Equal(paid.AmountDue, paid.AmountPaid);
        Assert.Equal(DateTime.Today, paid.PaymentDate);
        Assert.Equal("Paid", paid.Status);
        Assert.StartsWith("RCP-", paid.ReceiptNumber);
        Assert.Equal(0, paid.Outstanding);
    }

    // ----- what the dashboard shows as recent activity -----

    [Fact]
    public void GetRecentlyPaid_ReturnsOnlyPaidRowsNewestFirst()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);
        var unpaid = t.Backend.Payments.GetRegister(science.Id, ThisMonth).Where(p => p.Status != "Paid").ToList();

        var older = t.Backend.Payments.RecordPayment(unpaid[0].Id, unpaid[0].AmountDue, DateTime.Today.AddDays(-5));
        var newer = t.Backend.Payments.RecordPayment(unpaid[1].Id, unpaid[1].AmountDue, DateTime.Today);

        var recent = t.Backend.Payments.GetRecentlyPaid(10);

        Assert.All(recent, p => Assert.True(p.PaymentDate.HasValue));
        Assert.All(recent, p => Assert.True(p.AmountPaid > 0));

        int newerIndex = recent.FindIndex(p => p.Id == newer.Id);
        int olderIndex = recent.FindIndex(p => p.Id == older.Id);

        Assert.True(newerIndex >= 0 && olderIndex >= 0);
        Assert.True(newerIndex < olderIndex, "the newest payment should come first");
    }

    [Fact]
    public void GetRecentlyPaid_RespectsTheCount()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);

        foreach (var row in t.Backend.Payments.GetRegister(science.Id, ThisMonth).Where(p => p.Status != "Paid"))
        {
            t.Backend.Payments.MarkPaid(row.Id);
        }

        Assert.True(t.Backend.Payments.GetRecentlyPaid(2).Count <= 2);
    }

    [Fact]
    public void MarkUnpaid_UndoesIt()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);
        var row = t.Backend.Payments.GetRegister(science.Id, ThisMonth).First(p => p.Status == "Paid");

        var undone = t.Backend.Payments.MarkUnpaid(row.Id);

        Assert.Equal(0, undone.AmountPaid);
        Assert.Null(undone.PaymentDate);
        Assert.NotEqual("Paid", undone.Status);
    }

    [Fact]
    public void RecordPayment_PartAmountLeavesABalance()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);
        var row = t.Backend.Payments.GetRegister(science.Id, ThisMonth).First(p => p.Status != "Paid");

        var after = t.Backend.Payments.RecordPayment(row.Id, 1000, DateTime.Today, "Part payment");

        Assert.Equal(1000, after.AmountPaid);
        Assert.Equal(1500, after.Outstanding);
        Assert.Equal("Partial", after.Status);
    }

    [Fact]
    public void RecordPayment_AmountAndDateAreMandatory()
    {
        using var t = new TestBackend();
        var row = t.Backend.Payments.GetRegister(ScienceG10(t).Id, ThisMonth)[0];

        Assert.Throws<ArgumentException>(() => t.Backend.Payments.RecordPayment(row.Id, 0, DateTime.Today));
        Assert.Throws<ArgumentException>(() => t.Backend.Payments.RecordPayment(row.Id, 500, default));
    }

    // ----- status rules (FR-06) -----

    [Fact]
    public void Status_Paid_WhenTheFullFeeIsIn()
    {
        var p = new Payment { AmountDue = 2500, AmountPaid = 2500, DueDate = new DateTime(2026, 1, 10) };
        Assert.Equal("Paid", PaymentService.StatusFor(p, new DateTime(2026, 6, 1)));
    }

    [Fact]
    public void Status_Pending_BeforeTheDueDate()
    {
        var p = new Payment { AmountDue = 2500, AmountPaid = 0, DueDate = new DateTime(2026, 6, 10) };
        Assert.Equal("Pending", PaymentService.StatusFor(p, new DateTime(2026, 6, 1)));
    }

    [Fact]
    public void Status_Overdue_AfterTheDueDate()
    {
        var p = new Payment { AmountDue = 2500, AmountPaid = 0, DueDate = new DateTime(2026, 5, 10) };
        Assert.Equal("Overdue", PaymentService.StatusFor(p, new DateTime(2026, 6, 1)));
    }

    [Fact]
    public void Status_Partial_WhenSomeMoneyIsIn()
    {
        var p = new Payment { AmountDue = 2500, AmountPaid = 1000, DueDate = new DateTime(2026, 5, 10) };
        Assert.Equal("Partial", PaymentService.StatusFor(p, new DateTime(2026, 6, 1)));
    }

    [Fact]
    public void Status_ExtensionGranted_WhileTheExtensionLasts()
    {
        var p = new Payment { AmountDue = 2500, AmountPaid = 0, DueDate = new DateTime(2026, 5, 10), ExtensionUntil = new DateTime(2026, 7, 10) };
        Assert.Equal("Extension Granted", PaymentService.StatusFor(p, new DateTime(2026, 6, 15)));
        Assert.Equal("Overdue", PaymentService.StatusFor(p, new DateTime(2026, 8, 1)));
    }

    [Fact]
    public void GrantExtension_MovesTheDeadlineBy1To3Months()
    {
        using var t = new TestBackend();
        var row = t.Backend.Payments.GetRegister(ScienceG10(t).Id, ThisMonth)[0];
        var due = row.DueDate;

        t.Backend.Payments.GrantExtension(row.Id, 2, "Approved by admin");

        Assert.Equal(due.AddMonths(2), t.Backend.Payments.GetById(row.Id)!.ExtensionUntil);
        Assert.Throws<ArgumentException>(() => t.Backend.Payments.GrantExtension(row.Id, 4));
    }

    // ----- totals -----

    [Fact]
    public void OutstandingForStudent_AddsUpEveryClassTheyTake()
    {
        using var t = new TestBackend();
        var eleanor = t.Backend.Students.GetByStudentId("STU-2023-8941")!;

        // Eleanor takes ICT (paid this month) and Maths (not paid)
        decimal owed = t.Backend.Payments.OutstandingForStudent(eleanor.Id);

        Assert.Equal(3000, owed);
    }

    [Fact]
    public void Summary_AddsUpThisMonthsRegister()
    {
        using var t = new TestBackend();

        var summary = t.Backend.Payments.GetSummary(ThisMonth);

        Assert.True(summary.TotalExpected > 0);
        Assert.True(summary.Collected <= summary.TotalExpected);
        Assert.True(summary.PaidCount > 0);
        Assert.True(summary.UnpaidCount > 0);
        Assert.InRange(summary.CollectedPercent, 0, 100);
    }

    [Fact]
    public void GetOutstanding_ListsOnlyStudentsWhoStillOweMoney()
    {
        using var t = new TestBackend();

        var owing = t.Backend.Payments.GetOutstanding(ThisMonth);

        Assert.NotEmpty(owing);
        Assert.All(owing, p => Assert.True(p.Outstanding > 0));
    }

    [Fact]
    public void DueDateFor_IsTheTenthOfTheMonth()
    {
        Assert.Equal(new DateTime(2026, 9, 10), PaymentService.DueDateFor("2026-09"));
    }
}
