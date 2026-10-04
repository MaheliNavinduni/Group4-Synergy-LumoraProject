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

    // ================= Editing a login (client request) =================
    // The admin can change a teacher's username and password, and their own.

    [Fact]
    public void ChangeUsername_LetsThePersonSignInWithTheNewName()
    {
        using var t = new TestBackend();
        var user = t.Backend.Auth.Login("teacher", "teacher123")!;

        t.Backend.Auth.ChangeUsername(user.Id, "k.fernando");

        Assert.Null(t.Backend.Auth.Login("teacher", "teacher123"));
        Assert.NotNull(t.Backend.Auth.Login("k.fernando", "teacher123"));
    }

    [Fact]
    public void ChangeUsername_IsStoredInLowerCase()
    {
        using var t = new TestBackend();
        var user = t.Backend.Auth.Login("teacher", "teacher123")!;

        t.Backend.Auth.ChangeUsername(user.Id, "K.Fernando");

        Assert.Equal("k.fernando", t.Backend.Auth.GetById(user.Id)!.Username);
        Assert.NotNull(t.Backend.Auth.Login("k.fernando", "teacher123"));
    }

    [Fact]
    public void ChangeUsername_ToANameAlreadyTaken_Throws()
    {
        using var t = new TestBackend();
        var teacher = t.Backend.Auth.Login("teacher", "teacher123")!;

        Assert.Throws<InvalidOperationException>(() => t.Backend.Auth.ChangeUsername(teacher.Id, "admin"));

        // the old name still works, so nothing was half changed
        Assert.NotNull(t.Backend.Auth.Login("teacher", "teacher123"));
    }

    [Fact]
    public void ChangeUsername_ToTheSameName_IsAllowedAndChangesNothing()
    {
        using var t = new TestBackend();
        var user = t.Backend.Auth.Login("admin", "admin123")!;

        t.Backend.Auth.ChangeUsername(user.Id, "admin");

        Assert.NotNull(t.Backend.Auth.Login("admin", "admin123"));
    }

    [Theory]
    [InlineData("abc")]            // too short
    [InlineData("has space")]      // not allowed
    [InlineData("")]               // empty
    public void ChangeUsername_BadName_Throws(string name)
    {
        using var t = new TestBackend();
        var user = t.Backend.Auth.Login("admin", "admin123")!;

        Assert.ThrowsAny<ArgumentException>(() => t.Backend.Auth.ChangeUsername(user.Id, name));
    }

    [Fact]
    public void ChangePassword_WeakPassword_Throws()
    {
        using var t = new TestBackend();
        var user = t.Backend.Auth.Login("admin", "admin123")!;

        // no number in it
        Assert.Throws<ArgumentException>(() => t.Backend.Auth.ChangePassword(user.Id, "password"));

        // the old one still works
        Assert.NotNull(t.Backend.Auth.Login("admin", "admin123"));
    }

    [Fact]
    public void ChangeOwnPassword_WithTheCorrectCurrentPassword_Works()
    {
        using var t = new TestBackend();
        var user = t.Backend.Auth.Login("admin", "admin123")!;

        t.Backend.Auth.ChangeOwnPassword(user.Id, "admin123", "lumora99");

        Assert.Null(t.Backend.Auth.Login("admin", "admin123"));
        Assert.NotNull(t.Backend.Auth.Login("admin", "lumora99"));
    }

    [Fact]
    public void ChangeOwnPassword_WithTheWrongCurrentPassword_Throws()
    {
        using var t = new TestBackend();
        var user = t.Backend.Auth.Login("admin", "admin123")!;

        Assert.Throws<InvalidOperationException>(
            () => t.Backend.Auth.ChangeOwnPassword(user.Id, "wrongpass", "lumora99"));

        Assert.NotNull(t.Backend.Auth.Login("admin", "admin123"));
    }

    [Fact]
    public void ChangeOwnPassword_ReusingTheSamePassword_Throws()
    {
        using var t = new TestBackend();
        var user = t.Backend.Auth.Login("admin", "admin123")!;

        Assert.Throws<ArgumentException>(
            () => t.Backend.Auth.ChangeOwnPassword(user.Id, "admin123", "admin123"));
    }

    [Fact]
    public void GetByTeacherId_FindsTheLoginToEdit()
    {
        using var t = new TestBackend();
        var teacherUser = t.Backend.Auth.Login("teacher", "teacher123")!;

        var found = t.Backend.Auth.GetByTeacherId(teacherUser.TeacherId!.Value);

        Assert.NotNull(found);
        Assert.Equal(teacherUser.Id, found!.Id);
    }
}
