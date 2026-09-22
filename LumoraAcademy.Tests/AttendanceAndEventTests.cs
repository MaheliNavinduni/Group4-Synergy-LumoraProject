using LumoraAcademy.Core.Entities;

namespace LumoraAcademy.Tests;

public class AttendanceServiceTests
{
    private static int StudentId(TestBackend t, string code) => t.Backend.Students.GetByStudentId(code)!.Id;

    [Fact]
    public void Mark_SavesStatus_AndSecondMarkSameDayUpdatesInsteadOfDuplicating()
    {
        using var t = new TestBackend();
        int marcus = StudentId(t, "STU-8492");
        var day = new DateTime(2026, 9, 21);

        t.Backend.Attendance.Mark(marcus, day, "Grade 11 - A", "Absent", "Sick");
        t.Backend.Attendance.Mark(marcus, day, "Grade 11 - A", "Late", "Arrived 9:15");

        var entries = t.Backend.Attendance.GetForClassOnDay("Grade 11 - A", day);
        Assert.Single(entries);
        Assert.Equal("Late", entries[0].Status);
        Assert.Equal("Arrived 9:15", entries[0].Remarks);
    }

    [Fact]
    public void Mark_InvalidStatus_Throws()
    {
        using var t = new TestBackend();

        Assert.Throws<ArgumentException>(() => t.Backend.Attendance.Mark(StudentId(t, "STU-8492"), DateTime.Today, "X", "Sleeping"));
    }

    [Fact]
    public void MarkClass_SavesEveryRowAndSummaryCountsAreRight()
    {
        using var t = new TestBackend();
        var day = new DateTime(2026, 9, 22);
        var rows = new[]
        {
            (StudentId(t, "STU-8492"), "Present", ""),
            (StudentId(t, "STU-3321"), "Absent", "Medical leave"),
            (StudentId(t, "STU-9942"), "Late", ""),
        };

        t.Backend.Attendance.MarkClass(day, "Test Class", rows);

        var summary = t.Backend.Attendance.GetDailySummaries(day, day, "Test Class").Single();
        Assert.Equal(3, summary.Total);
        Assert.Equal(1, summary.Present);
        Assert.Equal(1, summary.Absent);
        Assert.Equal(1, summary.Late);
        Assert.Equal("Poor", summary.Status);
    }

    [Fact]
    public void Summary_Status_IsPerfectWhenEveryoneIsPresent()
    {
        var s = new AttendanceSummary { Total = 30, Present = 30 };
        Assert.Equal("Perfect", s.Status);

        var s2 = new AttendanceSummary { Total = 30, Present = 28, Late = 2 };
        Assert.Equal("Excellent", s2.Status);
    }

    [Fact]
    public void AttendanceRate_CountsLateAsPresent()
    {
        using var t = new TestBackend();
        int marcus = StudentId(t, "STU-8492");
        t.Backend.Attendance.Mark(marcus, new DateTime(2026, 9, 1), "C", "Present");
        t.Backend.Attendance.Mark(marcus, new DateTime(2026, 9, 2), "C", "Late");
        t.Backend.Attendance.Mark(marcus, new DateTime(2026, 9, 3), "C", "Absent");
        t.Backend.Attendance.Mark(marcus, new DateTime(2026, 9, 4), "C", "Absent");

        Assert.Equal(50, t.Backend.Attendance.AttendanceRateForStudent(marcus));
        Assert.Equal(2, t.Backend.Attendance.AbsencesForStudent(marcus));
    }

    [Fact]
    public void GetStats_ReturnsSeededNumbers()
    {
        using var t = new TestBackend();

        var stats = t.Backend.Attendance.GetStats();

        Assert.True(stats.TotalDaysLogged >= 2);
        Assert.True(stats.PerfectDays >= 1);
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
