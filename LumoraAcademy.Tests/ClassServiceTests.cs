using LumoraAcademy.Core.Entities;

namespace LumoraAcademy.Tests;

// Classes, their weekly slots, and who is enrolled.
public class ClassServiceTests
{
    private static ClassGroup ScienceG10(TestBackend t) =>
        t.Backend.Classes.GetAll().First(c => c.SubjectName == "Science" && c.Grade == "10th Grade");

    [Fact]
    public void SeededClasses_HaveSubjectTeacherFeeAndStudentCount()
    {
        using var t = new TestBackend();

        var science = ScienceG10(t);

        Assert.Equal("Science - 10th Grade", science.Name);
        Assert.Equal("Miss. Aries", science.TeacherName);
        Assert.Equal(2500, science.MonthlyFee);
        Assert.Equal(4, science.StudentCount);
        Assert.Equal("Mon, Wed, Fri", science.Days);
    }

    [Fact]
    public void GetStudents_ReturnsOnlyTheStudentsEnrolledInThatClass()
    {
        using var t = new TestBackend();

        var names = t.Backend.Classes.GetStudents(ScienceG10(t).Id).Select(s => s.FullName).ToList();

        Assert.Contains("Emma Stone", names);
        Assert.Contains("Mia Patel", names);
        Assert.DoesNotContain("Eleanor Vance", names);   // she is in the Grade 11 classes
    }

    [Fact]
    public void GetClassesForStudent_ListsEveryClassTheyAttend()
    {
        using var t = new TestBackend();
        var emma = t.Backend.Students.GetByStudentId("STU-2024-0891")!;

        var classes = t.Backend.Classes.GetClassesForStudent(emma.Id).Select(c => c.Name).ToList();

        Assert.Contains("Science - 10th Grade", classes);
        Assert.Contains("Sinhala - 10th Grade", classes);
        Assert.Equal(2, classes.Count);
    }

    [Fact]
    public void Enroll_AddsStudentAndRaisesTheCount()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);
        var eleanor = t.Backend.Students.GetByStudentId("STU-2023-8941")!;
        int before = science.StudentCount;

        t.Backend.Classes.Enroll(eleanor.Id, science.Id);

        Assert.Equal(before + 1, t.Backend.Classes.CountStudents(science.Id));
        Assert.True(t.Backend.Classes.IsEnrolled(eleanor.Id, science.Id));
    }

    [Fact]
    public void Enroll_SameStudentTwice_Throws()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);
        var emma = t.Backend.Students.GetByStudentId("STU-2024-0891")!;

        Assert.Throws<InvalidOperationException>(() => t.Backend.Classes.Enroll(emma.Id, science.Id));
    }

    [Fact]
    public void Unenroll_RemovesFromClassButKeepsHistory()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);
        var emma = t.Backend.Students.GetByStudentId("STU-2024-0891")!;
        int attendanceBefore = t.Backend.Attendance.GetForStudent(emma.Id).Count;

        t.Backend.Classes.Unenroll(emma.Id, science.Id);

        Assert.False(t.Backend.Classes.IsEnrolled(emma.Id, science.Id));
        Assert.Equal(attendanceBefore, t.Backend.Attendance.GetForStudent(emma.Id).Count);
    }

    [Fact]
    public void Enroll_AfterLeaving_ReusesTheSameRow()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);
        var emma = t.Backend.Students.GetByStudentId("STU-2024-0891")!;

        t.Backend.Classes.Unenroll(emma.Id, science.Id);
        t.Backend.Classes.Enroll(emma.Id, science.Id);

        Assert.True(t.Backend.Classes.IsEnrolled(emma.Id, science.Id));
        Assert.Equal(4, t.Backend.Classes.CountStudents(science.Id));
    }

    [Fact]
    public void GetSessionsForDay_ReturnsTodaysClassesEarliestFirst()
    {
        using var t = new TestBackend();

        // Monday has Science 08:00, Maths 09:45 and Sinhala 13:00
        var monday = t.Backend.Classes.GetSessionsForDay(new DateTime(2026, 9, 21));   // a Monday

        Assert.Equal(3, monday.Count);
        Assert.Equal("Science", monday[0].SubjectName);
        Assert.Equal("08:00 AM - 09:30 AM", monday[0].TimeRange);
        Assert.Equal(monday.OrderBy(s => s.StartTime).Select(s => s.Id), monday.Select(s => s.Id));
    }

    [Fact]
    public void AddSession_ClashingWithSameTeacher_Throws()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);   // Miss. Aries, Monday 08:00-09:30

        var clash = new ClassSession { ClassGroupId = science.Id, DayOfWeek = 1, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0) };

        Assert.Throws<InvalidOperationException>(() => t.Backend.Classes.AddSession(clash));
    }

    [Fact]
    public void AddSession_EndBeforeStart_Throws()
    {
        using var t = new TestBackend();
        var bad = new ClassSession { ClassGroupId = ScienceG10(t).Id, DayOfWeek = 0, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(9, 0, 0) };

        Assert.Throws<ArgumentException>(() => t.Backend.Classes.AddSession(bad));
    }

    [Fact]
    public void Create_DuplicateClassForSameTeacherSubjectGrade_Throws()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);

        var duplicate = new ClassGroup { SubjectId = science.SubjectId, TeacherId = science.TeacherId, Grade = science.Grade, MonthlyFee = 1000 };

        Assert.Throws<InvalidOperationException>(() => t.Backend.Classes.Create(duplicate));
    }

    [Fact]
    public void Deactivate_ClosesTheClassAndItsEnrolments()
    {
        using var t = new TestBackend();
        var science = ScienceG10(t);

        t.Backend.Classes.Deactivate(science.Id);

        Assert.DoesNotContain(t.Backend.Classes.GetAll(), c => c.Id == science.Id);
        Assert.Equal(0, t.Backend.Classes.CountStudents(science.Id));
    }
}
