using LumoraAcademy.Core.Security;

namespace LumoraAcademy.Tests;

// Epic 1, User Story 1 - secure login.
public class AuthServiceTests
{
    [Fact]
    public void Login_WithCorrectAdminCredentials_ReturnsAdminUser()
    {
        using var t = new TestBackend();

        var user = t.Backend.Auth.Login("admin", "admin123");

        Assert.NotNull(user);
        Assert.Equal("Admin", user!.Role);
    }

    [Fact]
    public void Login_WithCorrectTeacherCredentials_ReturnsTeacherLinkedToTeacherRecord()
    {
        using var t = new TestBackend();

        var user = t.Backend.Auth.Login("teacher", "teacher123");

        Assert.NotNull(user);
        Assert.Equal("Teacher", user!.Role);
        Assert.NotNull(user.TeacherId);
        Assert.Equal("Miss. Aries", t.Backend.Teachers.GetById(user.TeacherId!.Value)!.FullName);
    }

    [Fact]
    public void Login_WithWrongPassword_ReturnsNull()
    {
        using var t = new TestBackend();

        Assert.Null(t.Backend.Auth.Login("admin", "wrong"));
    }

    [Fact]
    public void Login_WithUnknownUser_ReturnsNull()
    {
        using var t = new TestBackend();

        Assert.Null(t.Backend.Auth.Login("nobody", "admin123"));
    }

    [Fact]
    public void Login_WithEmptyFields_ReturnsNull()
    {
        using var t = new TestBackend();

        Assert.Null(t.Backend.Auth.Login("", "admin123"));
        Assert.Null(t.Backend.Auth.Login("admin", ""));
    }

    [Fact]
    public void Login_IsCaseInsensitiveForUsername()
    {
        using var t = new TestBackend();

        Assert.NotNull(t.Backend.Auth.Login("ADMIN", "admin123"));
    }

    [Fact]
    public void Login_DeactivatedAccount_ReturnsNull()
    {
        using var t = new TestBackend();
        var user = t.Backend.Auth.Login("teacher", "teacher123")!;

        t.Backend.Auth.SetActive(user.Id, false);

        Assert.Null(t.Backend.Auth.Login("teacher", "teacher123"));
    }

    [Fact]
    public void CreateUser_DuplicateUsername_Throws()
    {
        using var t = new TestBackend();

        Assert.Throws<InvalidOperationException>(() => t.Backend.Auth.CreateUser("admin", "another1", "Admin", "Dup"));
    }

    [Fact]
    public void CreateUser_ShortPassword_Throws()
    {
        using var t = new TestBackend();

        Assert.Throws<ArgumentException>(() => t.Backend.Auth.CreateUser("newuser", "123", "Teacher", "New"));
    }

    [Fact]
    public void PasswordIsStoredAsHash_NotPlainText()
    {
        using var t = new TestBackend();

        var user = t.Backend.Auth.GetAll().First(u => u.Username == "admin");

        Assert.NotEqual("admin123", user.PasswordHash);
        Assert.True(PasswordHasher.Verify("admin123", user.PasswordHash, user.PasswordSalt));
    }

    [Fact]
    public void ChangePassword_OldPasswordStopsWorking()
    {
        using var t = new TestBackend();
        var user = t.Backend.Auth.Login("admin", "admin123")!;

        t.Backend.Auth.ChangePassword(user.Id, "newpass99");

        Assert.Null(t.Backend.Auth.Login("admin", "admin123"));
        Assert.NotNull(t.Backend.Auth.Login("admin", "newpass99"));
    }
}
