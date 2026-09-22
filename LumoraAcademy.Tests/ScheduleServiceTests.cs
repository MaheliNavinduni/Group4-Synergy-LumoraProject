using LumoraAcademy.Core.Entities;

namespace LumoraAcademy.Tests;

public class ScheduleServiceTests
{
    private static int TeacherId(TestBackend t, string code) => t.Backend.Teachers.GetByTeacherId(code)!.Id;

    [Fact]
    public void SeededTimetable_HasAriesLessonsOnMondayOnly()
    {
        using var t = new TestBackend();
        int aries = TeacherId(t, "TCH-1001");

        var monday = t.Backend.Schedule.GetDayForTeacher(aries, DayOfWeek.Monday);
        var sunday = t.Backend.Schedule.GetDayForTeacher(aries, DayOfWeek.Sunday);

        Assert.Equal(2, monday.Count);
        Assert.Equal("Science", monday[0].SubjectName);
        Assert.Equal("08:00 AM - 09:30 AM", monday[0].TimeRange);
        Assert.Empty(sunday);
    }

    [Fact]
    public void Add_SavesLessonWithNamesFilledIn()
    {
        using var t = new TestBackend();
        int lopez = TeacherId(t, "TCH-1085");
        int maths = t.Backend.Academics.GetSubjects().First(s => s.Code == "M2103").Id;

        var lesson = t.Backend.Schedule.Add(new ClassSession { TeacherId = lopez, SubjectId = maths, ClassName = "9th Grade", DayOfWeek = 5, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(9, 0, 0), Room = "101" });

        Assert.Equal("Mathematics", lesson.SubjectName);
        Assert.Equal("Elena Lopez", lesson.TeacherName);
        Assert.Contains(t.Backend.Schedule.GetDayForTeacher(lopez, DayOfWeek.Friday), s => s.Id == lesson.Id);
    }

    [Fact]
    public void Add_OverlappingLessonForSameTeacher_Throws()
    {
        using var t = new TestBackend();
        int aries = TeacherId(t, "TCH-1001");

        // Aries already teaches Monday 08:00-09:30
        var clash = new ClassSession { TeacherId = aries, ClassName = "9th Grade", DayOfWeek = 1, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0) };

        Assert.Throws<InvalidOperationException>(() => t.Backend.Schedule.Add(clash));
    }

    [Fact]
    public void Add_EndBeforeStart_Throws()
    {
        using var t = new TestBackend();
        var bad = new ClassSession { TeacherId = TeacherId(t, "TCH-1001"), ClassName = "9th Grade", DayOfWeek = 0, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(9, 0, 0) };

        Assert.Throws<ArgumentException>(() => t.Backend.Schedule.Add(bad));
    }

    [Fact]
    public void SubjectSummaries_GroupLessonsBySubjectWithDaysAndStudentCount()
    {
        using var t = new TestBackend();
        int aries = TeacherId(t, "TCH-1001");

        var cards = t.Backend.Schedule.GetSubjectSummariesForTeacher(aries);

        var science = cards.First(c => c.SubjectName == "Science");
        Assert.Equal("Mon, Wed, Fri", science.Days);
        Assert.Equal("10th Grade • Room 302", science.Code);
        Assert.True(science.StudentCount >= 3);

        var ict = cards.First(c => c.SubjectName == "ICT");
        Assert.Equal("Tue, Thu", ict.Days);
    }

    [Fact]
    public void GetDayForClass_ReturnsLessonsForThatGrade()
    {
        using var t = new TestBackend();

        var grade10Monday = t.Backend.Schedule.GetDayForClass("10th Grade", DayOfWeek.Monday);

        Assert.True(grade10Monday.Count >= 2);
        Assert.All(grade10Monday, s => Assert.Equal("10th Grade", s.ClassName));
    }

    [Fact]
    public void Delete_RemovesLesson()
    {
        using var t = new TestBackend();
        int aries = TeacherId(t, "TCH-1001");
        var lesson = t.Backend.Schedule.GetDayForTeacher(aries, DayOfWeek.Monday)[0];

        t.Backend.Schedule.Delete(lesson.Id);

        Assert.Null(t.Backend.Schedule.GetById(lesson.Id));
    }
}
