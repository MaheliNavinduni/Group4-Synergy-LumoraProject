using LumoraAcademy.Core.Entities;

namespace LumoraAcademy.Tests;

// Epic 2 - Student Management.
public class StudentServiceTests
{
    private static Student NewStudent(string name = "Test Student", string id = "") => new()
    {
        StudentId = id,
        FullName = name,
        DateOfBirth = new DateTime(2010, 1, 1),
        Gender = "Female",
        Grade = "10th Grade",
        Section = "A",
        GuardianName = "Parent Name",
        GuardianPhone = "0771234567",
    };

    // ----- US1: register -----

    [Fact]
    public void Register_SavesStudentAndGeneratesUniqueId()
    {
        using var t = new TestBackend();
        int before = t.Backend.Students.Count();

        var saved = t.Backend.Students.Register(NewStudent());

        Assert.Equal(before + 1, t.Backend.Students.Count());
        Assert.StartsWith("STU-", saved.StudentId);
        Assert.NotNull(t.Backend.Students.GetByStudentId(saved.StudentId));
    }

    [Fact]
    public void Register_DuplicateStudentId_Throws()
    {
        using var t = new TestBackend();

        Assert.Throws<InvalidOperationException>(() => t.Backend.Students.Register(NewStudent("Copy", "STU-2023-8941")));
    }

    [Fact]
    public void Register_MissingMandatoryFields_Throws()
    {
        using var t = new TestBackend();
        var s = NewStudent();
        s.FullName = "";

        Assert.Throws<ArgumentException>(() => t.Backend.Students.Register(s));

        var s2 = NewStudent();
        s2.GuardianPhone = "";
        Assert.Throws<ArgumentException>(() => t.Backend.Students.Register(s2));
    }

    [Fact]
    public void Register_TwoStudents_GetDifferentGeneratedIds()
    {
        using var t = new TestBackend();

        var a = t.Backend.Students.Register(NewStudent("A"));
        var b = t.Backend.Students.Register(NewStudent("B"));

        Assert.NotEqual(a.StudentId, b.StudentId);
    }

    // ----- US2: update -----

    [Fact]
    public void Update_ChangesAreSaved()
    {
        using var t = new TestBackend();
        var s = t.Backend.Students.GetByStudentId("STU-2023-8941")!;

        s.Grade = "12th Grade";
        s.GuardianPhone = "0770000000";
        t.Backend.Students.Update(s);

        var reloaded = t.Backend.Students.GetById(s.Id)!;
        Assert.Equal("12th Grade", reloaded.Grade);
        Assert.Equal("0770000000", reloaded.GuardianPhone);
    }

    [Fact]
    public void Update_ToAnotherStudentsId_Throws()
    {
        using var t = new TestBackend();
        var s = t.Backend.Students.GetByStudentId("STU-2023-8941")!;
        s.StudentId = "STU-8492";   // belongs to Marcus

        Assert.Throws<InvalidOperationException>(() => t.Backend.Students.Update(s));
    }

    // ----- US3: search (FR-10) -----

    [Fact]
    public void Search_ByStudentId_FindsStudent()
    {
        using var t = new TestBackend();

        var results = t.Backend.Students.Search("STU-2023-8941");

        Assert.Single(results);
        Assert.Equal("Eleanor Vance", results[0].FullName);
    }

    [Fact]
    public void Search_ByPartialName_IsCaseInsensitive()
    {
        using var t = new TestBackend();

        var results = t.Backend.Students.Search("chen");

        Assert.Contains(results, s => s.FullName == "Emily Chen");
        Assert.Contains(results, s => s.FullName == "James Chen");
        Assert.Contains(results, s => s.FullName == "Liam Chen");
    }

    [Fact]
    public void Search_ByParentContactNumber_FindsStudent()
    {
        using var t = new TestBackend();

        var results = t.Backend.Students.Search("019-2834");

        Assert.Single(results);
        Assert.Equal("Eleanor Vance", results[0].FullName);
    }

    [Fact]
    public void Search_NoMatch_ReturnsEmptyList()
    {
        using var t = new TestBackend();

        Assert.Empty(t.Backend.Students.Search("zzzz-not-a-student"));
    }

    [Fact]
    public void Search_FiltersByGradeAndStatus()
    {
        using var t = new TestBackend();

        var active10 = t.Backend.Students.Search(null, "10th Grade", new[] { "Active" });

        Assert.NotEmpty(active10);
        Assert.All(active10, s => Assert.Equal("10th Grade", s.Grade));
        Assert.All(active10, s => Assert.Equal("Active", s.Status));
    }

    [Fact]
    public void GetAll_FillsAssignedTeacherName()
    {
        using var t = new TestBackend();

        var eleanor = t.Backend.Students.GetByStudentId("STU-2023-8941")!;

        Assert.Equal("Mr. Davis", eleanor.AssignedTeacherName);
        Assert.Equal("EV", eleanor.Initials);
    }

    // ----- departure -----

    [Fact]
    public void ProcessDeparture_MarksInactiveAndKeepsRecord()
    {
        using var t = new TestBackend();
        var s = t.Backend.Students.GetByStudentId("STU-8492")!;

        t.Backend.Students.ProcessDeparture(s.Id, new DateTime(2026, 12, 31), "Relocation", "Moving abroad");

        var reloaded = t.Backend.Students.GetById(s.Id)!;
        Assert.Equal("Inactive", reloaded.Status);
        Assert.Equal(new DateTime(2026, 12, 31), reloaded.DepartureDate);
        Assert.Equal("Relocation", reloaded.DepartureReason);
        Assert.Contains("Moving abroad", reloaded.Notes);
    }
}
