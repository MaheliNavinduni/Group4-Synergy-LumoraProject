using LumoraAcademy.Core;
using LumoraAcademy.Core.Services;

namespace LumoraAcademy.Tests;

// Epic 5 - Reporting.
public class ReportServiceTests
{
    [Fact]
    public void StudentReport_MatchesTheDesignedReportCard()
    {
        using var t = new TestBackend();
        var alex = t.Backend.Students.GetByStudentId("SEA-24-0891")!;

        var report = t.Backend.Reports.BuildStudentReport(alex.Id);

        Assert.Equal(5, report.Rows.Count);
        var maths = report.Rows.First(r => r.Subject == "Mathematics");
        Assert.Equal(88, maths.MonthlyTest);
        Assert.Equal(92, maths.TermExam);
        Assert.Equal(90, maths.FinalScore);
        Assert.Equal("A", maths.Grade);
        Assert.Single(report.CambridgeResults);
        Assert.True(report.Gpa > 3.5);
        Assert.InRange(report.TotalPercent, 90, 95);
    }

    [Fact]
    public void StudentReport_ForStudentWithNoMarks_IsEmptyNotError()
    {
        using var t = new TestBackend();
        var tyrell = t.Backend.Students.GetByStudentId("STU-1198")!;

        var report = t.Backend.Reports.BuildStudentReport(tyrell.Id);

        Assert.Empty(report.Rows);
        Assert.Equal(0, report.Gpa);
    }

    [Fact]
    public void PaymentReport_ListsEveryStudentWithFees_AndCanFilterByStatus()
    {
        using var t = new TestBackend();

        var all = t.Backend.Reports.BuildPaymentReport();
        var paidOnly = t.Backend.Reports.BuildPaymentReport("Paid");

        Assert.True(all.Count >= 5);
        Assert.NotEmpty(paidOnly);
        Assert.All(paidOnly, r => Assert.Equal("Paid", r.Status));
    }

    [Fact]
    public void AcademicReport_CanFilterByGrade()
    {
        using var t = new TestBackend();

        var grade10 = t.Backend.Reports.BuildAcademicReport("10th Grade");

        Assert.NotEmpty(grade10);
        Assert.All(grade10, r => Assert.Equal("10th Grade", r.Grade));
    }

    [Fact]
    public void AverageBySubject_HasOneEntryPerSubjectWithMarks()
    {
        using var t = new TestBackend();

        var averages = t.Backend.Reports.AverageBySubject();

        Assert.True(averages.ContainsKey("Mathematics"));
        Assert.All(averages.Values, v => Assert.InRange(v, 0, 100));
    }

    [Fact]
    public void GpaFor_MapsPercentToFourPointScale()
    {
        Assert.Equal(4.0, ReportService.GpaFor(95));
        Assert.Equal(3.0, ReportService.GpaFor(77));
        Assert.Equal(0.0, ReportService.GpaFor(20));
    }
}

// Epic 6 - Database Management.
public class DatabaseTests
{
    [Fact]
    public void FreshDatabase_IsSeededOnce_AndNotAgainOnReopen()
    {
        string path = Path.Combine(Path.GetTempPath(), "lumora-reopen-" + Guid.NewGuid().ToString("N") + ".db");
        try
        {
            var first = new Backend(path);
            int students = first.Students.Count();
            first.Database.Connection.Close();

            var second = new Backend(path);      // reopening must not duplicate the demo rows
            Assert.Equal(students, second.Students.Count());
            second.Database.Connection.Close();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void DataSurvivesReopen()
    {
        string path = Path.Combine(Path.GetTempPath(), "lumora-persist-" + Guid.NewGuid().ToString("N") + ".db");
        try
        {
            var first = new Backend(path, seedDemoData: false);
            first.Academics.AddSubject("Persist", "PERS1", "");
            first.Database.Connection.Close();

            var second = new Backend(path, seedDemoData: false);
            Assert.Contains(second.Academics.GetSubjects(), s => s.Code == "PERS1");
            second.Database.Connection.Close();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Backup_CreatesACopyOfTheDatabaseFile()
    {
        using var t = new TestBackend();
        string folder = Path.Combine(Path.GetTempPath(), "lumora-backups-" + Guid.NewGuid().ToString("N"));
        try
        {
            string file = t.Backend.Database.Backup(folder);
            Assert.True(File.Exists(file));
            Assert.True(new FileInfo(file).Length > 0);
        }
        finally
        {
            if (Directory.Exists(folder)) Directory.Delete(folder, true);
        }
    }

    [Fact]
    public void ClearAll_EmptiesEveryTable()
    {
        using var t = new TestBackend();

        t.Backend.Database.ClearAll();

        Assert.True(t.Backend.Database.IsEmpty());
        Assert.Equal(0, t.Backend.Students.Count());
        Assert.Empty(t.Backend.Academics.GetSubjects());
    }
}

public class StudentReportPdfTests
{
    [Fact]
    public void Save_WritesAValidPdfFile()
    {
        using var t = new TestBackend();
        var alex = t.Backend.Students.GetByStudentId("SEA-24-0891")!;
        var report = t.Backend.Reports.BuildStudentReport(alex.Id);
        string path = Path.Combine(Path.GetTempPath(), "lumora-report-" + Guid.NewGuid().ToString("N") + ".pdf");

        try
        {
            LumoraAcademy.Core.Reports.StudentReportPdf.Save(report, path);

            Assert.True(File.Exists(path));
            var bytes = File.ReadAllBytes(path);
            Assert.True(bytes.Length > 1000);
            Assert.Equal("%PDF", System.Text.Encoding.ASCII.GetString(bytes, 0, 4));   // every PDF starts with this
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void ToBytes_WorksForStudentWithNoMarks()
    {
        using var t = new TestBackend();
        var tyrell = t.Backend.Students.GetByStudentId("STU-1198")!;

        var bytes = LumoraAcademy.Core.Reports.StudentReportPdf.ToBytes(t.Backend.Reports.BuildStudentReport(tyrell.Id));

        Assert.True(bytes.Length > 500);
    }
}
