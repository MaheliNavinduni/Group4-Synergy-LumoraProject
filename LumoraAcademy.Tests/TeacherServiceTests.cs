using LumoraAcademy.Core.Entities;

namespace LumoraAcademy.Tests;

// Epic 1, User Story 2 - manage teacher accounts.
public class TeacherServiceTests
{
    private static Teacher NewTeacher() => new()
    {
        FullName = "New Teacher",
        Designation = "Lecturer",
        Department = "STEM",
        Email = "new.teacher@eduadmin.com",
        Phone = "0771112222",
        JoiningDate = new DateTime(2026, 1, 1),
        YearsOfExperience = 3,
    };

    [Fact]
    public void Register_CreatesTeacherAndLoginAccount()
    {
        using var t = new TestBackend();

        var teacher = t.Backend.Teachers.Register(NewTeacher(), "newteacher", "secret99");

        Assert.StartsWith("TCH-", teacher.TeacherId);
        var user = t.Backend.Auth.Login("newteacher", "secret99");
        Assert.NotNull(user);
        Assert.Equal("Teacher", user!.Role);
        Assert.Equal(teacher.Id, user.TeacherId);
    }

    [Fact]
    public void Register_DuplicateUsername_ThrowsAndDoesNotSaveTeacher()
    {
        using var t = new TestBackend();
        int before = t.Backend.Teachers.GetAll().Count;

        Assert.Throws<InvalidOperationException>(() => t.Backend.Teachers.Register(NewTeacher(), "admin", "secret99"));
        Assert.Equal(before, t.Backend.Teachers.GetAll().Count);
    }

    [Fact]
    public void Register_MissingEmail_Throws()
    {
        using var t = new TestBackend();
        var teacher = NewTeacher();
        teacher.Email = "";

        Assert.Throws<ArgumentException>(() => t.Backend.Teachers.Register(teacher, "x1", "secret99"));
    }

    [Fact]
    public void Update_SavesChangesAndSyncsLoginDisplayName()
    {
        using var t = new TestBackend();
        var teacher = t.Backend.Teachers.Register(NewTeacher(), "upd", "secret99");

        teacher.FullName = "Renamed Teacher";
        teacher.Phone = "0779999999";
        t.Backend.Teachers.Update(teacher);

        Assert.Equal("Renamed Teacher", t.Backend.Teachers.GetById(teacher.Id)!.FullName);
        Assert.Equal("Renamed Teacher", t.Backend.Auth.GetByTeacherId(teacher.Id)!.DisplayName);
    }

    [Fact]
    public void ProcessResignation_MarksResignedAndBlocksLogin()
    {
        using var t = new TestBackend();
        var teacher = t.Backend.Teachers.Register(NewTeacher(), "leaving", "secret99");

        t.Backend.Teachers.ProcessResignation(teacher.Id, new DateTime(2026, 10, 31), "Relocation", "Moving to Kandy");

        var reloaded = t.Backend.Teachers.GetById(teacher.Id)!;
        Assert.Equal("Resigned", reloaded.Status);
        Assert.Equal(new DateTime(2026, 10, 31), reloaded.LastWorkingDay);
        Assert.Null(t.Backend.Auth.Login("leaving", "secret99"));
    }

    [Fact]
    public void Counts_And_DepartmentPercentages_AreComputed()
    {
        using var t = new TestBackend();

        Assert.True(t.Backend.Teachers.Count() > 0);
        Assert.Equal(1, t.Backend.Teachers.CountOnLeave());

        var pct = t.Backend.Teachers.DepartmentPercentages();
        Assert.True(pct.ContainsKey("STEM"));
        Assert.InRange(pct.Values.Sum(), 98, 102);   // rounding
    }

    [Fact]
    public void Search_FindsByNameOrId()
    {
        using var t = new TestBackend();

        Assert.Contains(t.Backend.Teachers.Search("chen"), x => x.FullName == "Robert Chen");
        Assert.Contains(t.Backend.Teachers.Search("TCH-1085"), x => x.FullName == "Elena Lopez");
    }
}
