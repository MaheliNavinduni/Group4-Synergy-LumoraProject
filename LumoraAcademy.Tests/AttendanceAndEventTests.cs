using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Services;

namespace LumoraAcademy.Tests;

// Attendance is marked per class in the office. Nothing defaults to Present.
public class AttendanceServiceTests
{
    private static ClassGroup ScienceG10(TestBackend t) =>
        t.Backend.Classes.GetAll().First(c => c.SubjectName == "Science" && c.Grade == "10th Grade");

    [Fact]
    public void GetSheet_StartsWithEveryStudentUnmarked()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);

        var sheet = t.Backend.Attendance.GetSheet(science.Id, DateTime.Today);

        Assert.Equal(4, sheet.Count);
        Assert.All(sheet, r => Assert.Equal("", r.Status));
        Assert.All(sheet, r => Assert.False(r.IsMarked));
    }

    [Fact]
    public void GetSheet_ShowsWhatWasSavedEarlier()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);
        var day = DateTime.Today;
        var sheet = t.Backend.Attendance.GetSheet(science.Id, day);

        sheet[0].Status = AttendanceEntry.Present;
        sheet[1].Status = AttendanceEntry.Absent;
        sheet[1].Remarks = "Sick";
        t.Backend.Attendance.SaveSheet(science.Id, day, sheet);

        var reloaded = t.Backend.Attendance.GetSheet(science.Id, day);
        Assert.Equal(AttendanceEntry.Present, reloaded[0].Status);
        Assert.Equal(AttendanceEntry.Absent, reloaded[1].Status);
        Assert.Equal("Sick", reloaded[1].Remarks);
        Assert.Equal("", reloaded[2].Status);          // still not marked
    }

    [Fact]
    public void SaveSheet_DoesNotSaveRowsLeftBlank()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);
        var day = DateTime.Today;
        var sheet = t.Backend.Attendance.GetSheet(science.Id, day);

        sheet[0].Status = AttendanceEntry.Present;     // only one student marked
        t.Backend.Attendance.SaveSheet(science.Id, day, sheet);

        Assert.Single(t.Backend.Attendance.GetForClassOnDay(science.Id, day));
    }

    [Fact]
    public void SaveSheet_ClearingAStatusRemovesTheEarlierMark()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);
        var day = DateTime.Today;

        var sheet = t.Backend.Attendance.GetSheet(science.Id, day);
        sheet[0].Status = AttendanceEntry.Present;
        t.Backend.Attendance.SaveSheet(science.Id, day, sheet);

        sheet = t.Backend.Attendance.GetSheet(science.Id, day);
        sheet[0].Status = "";                          // admin undid it
        t.Backend.Attendance.SaveSheet(science.Id, day, sheet);

        Assert.Empty(t.Backend.Attendance.GetForClassOnDay(science.Id, day));
    }

    [Fact]
    public void SaveSheet_MarkingTwiceUpdatesInsteadOfDuplicating()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);
        var day = DateTime.Today;

        var sheet = t.Backend.Attendance.GetSheet(science.Id, day);
        sheet[0].Status = AttendanceEntry.Absent;
        t.Backend.Attendance.SaveSheet(science.Id, day, sheet);

        sheet = t.Backend.Attendance.GetSheet(science.Id, day);
        sheet[0].Status = AttendanceEntry.Late;
        t.Backend.Attendance.SaveSheet(science.Id, day, sheet);

        var saved = t.Backend.Attendance.GetForClassOnDay(science.Id, day);
        Assert.Single(saved);
        Assert.Equal(AttendanceEntry.Late, saved[0].Status);
    }

    [Fact]
    public void Mark_InvalidStatus_Throws()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);
        var emma = t.Backend.Students.GetByStudentId("STU-2024-0891")!;

        Assert.Throws<ArgumentException>(() => t.Backend.Attendance.Mark(emma.Id, science.Id, DateTime.Today, "Sleeping"));
    }

    [Fact]
    public void IsMarked_TellsWhetherTheClassWasDoneThatDay()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);

        Assert.False(t.Backend.Attendance.IsMarked(science.Id, DateTime.Today));
        t.Backend.Attendance.Mark(t.Backend.Classes.GetStudents(science.Id)[0].Id, science.Id, DateTime.Today, AttendanceEntry.Present);
        Assert.True(t.Backend.Attendance.IsMarked(science.Id, DateTime.Today));
    }

    [Fact]
    public void Summary_CountsMarkedStudentsAndShowsHowManyAreNotRecorded()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);
        var day = DateTime.Today;

        var sheet = t.Backend.Attendance.GetSheet(science.Id, day);
        sheet[0].Status = AttendanceEntry.Present;
        sheet[1].Status = AttendanceEntry.Absent;
        t.Backend.Attendance.SaveSheet(science.Id, day, sheet);

        var summary = t.Backend.Attendance.GetDailySummaries(day, day, science.Id).Single();
        Assert.Equal(4, summary.Enrolled);
        Assert.Equal(2, summary.Marked);
        Assert.Equal(2, summary.NotRecorded);
        Assert.Equal(1, summary.Present);
        Assert.Equal(1, summary.Absent);
    }

    [Fact]
    public void Summary_Status_IsPerfectWhenEveryMarkedStudentIsPresent()
    {
        Assert.Equal("Perfect", new AttendanceSummary { Enrolled = 30, Present = 30 }.Status);
        Assert.Equal("Excellent", new AttendanceSummary { Enrolled = 30, Present = 28, Late = 2 }.Status);
        Assert.Equal("Poor", new AttendanceSummary { Enrolled = 10, Present = 6, Absent = 4 }.Status);
        Assert.Equal("", new AttendanceSummary { Enrolled = 10 }.Status);   // nothing marked yet
    }

    [Fact]
    public void AttendanceRate_CountsLateAsPresent()
    {
        using var t = new TestBackend();
        // Sinhala has no seeded attendance, so this class starts empty.
        var sinhala = t.Backend.Classes.GetAll().First(c => c.SubjectName == "Sinhala");
        var emma = t.Backend.Students.GetByStudentId("STU-2024-0891")!;

        t.Backend.Attendance.Mark(emma.Id, sinhala.Id, new DateTime(2026, 9, 1), AttendanceEntry.Present);
        t.Backend.Attendance.Mark(emma.Id, sinhala.Id, new DateTime(2026, 9, 2), AttendanceEntry.Late);
        t.Backend.Attendance.Mark(emma.Id, sinhala.Id, new DateTime(2026, 9, 3), AttendanceEntry.Absent);
        t.Backend.Attendance.Mark(emma.Id, sinhala.Id, new DateTime(2026, 9, 4), AttendanceEntry.Absent);

        // Present + Late = 2 of 4 days
        Assert.Equal(50, t.Backend.Attendance.AttendanceRateForStudent(emma.Id, sinhala.Id));
    }

    [Fact]
    public void GetStats_ReturnsSeededNumbers()
    {
        using var t = new TestBackend();

        var stats = t.Backend.Attendance.GetStats();

        Assert.True(stats.TotalDaysLogged >= 2);
        Assert.InRange(stats.AverageRate, 0, 100);
    }
}

public class EventServiceTests
{
    [Fact]
    public void Create_Update_Delete_Event()
    {
        using var t = new TestBackend();

        var ev = t.Backend.Events.Create(new SchoolEvent { Title = "Sports Day", Date = new DateTime(2026, 11, 5), Category = "Sports", Location = "Ground" });
        ev.Location = "Main Ground";
        t.Backend.Events.Update(ev);

        Assert.Equal("Main Ground", t.Backend.Events.GetById(ev.Id)!.Location);

        t.Backend.Events.Delete(ev.Id);
        Assert.Null(t.Backend.Events.GetById(ev.Id));
    }

    [Fact]
    public void Create_WithoutTitle_Throws()
    {
        using var t = new TestBackend();

        Assert.Throws<ArgumentException>(() => t.Backend.Events.Create(new SchoolEvent { Date = DateTime.Today }));
    }

    [Fact]
    public void GetUpcoming_ReturnsOnlyFutureEventsSoonestFirst()
    {
        using var t = new TestBackend();

        var upcoming = t.Backend.Events.GetUpcoming();

        Assert.NotEmpty(upcoming);
        Assert.All(upcoming, e => Assert.True(e.Date >= DateTime.Today));
        Assert.Equal(upcoming.OrderBy(e => e.Date).Select(e => e.Id), upcoming.Select(e => e.Id));
    }
}
