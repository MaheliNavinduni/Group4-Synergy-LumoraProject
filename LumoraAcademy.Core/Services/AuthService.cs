using LumoraAcademy.Core.Database;
using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Security;

namespace LumoraAcademy.Core.Services;

// Epic 1 - User Authentication & Access Management.
public class AuthService
{
    private readonly AppDatabase _db;

    public AuthService(AppDatabase db)
    {
        _db = db;
    }

    // Returns the user when the username + password are correct and the account is active.
    // Returns null otherwise (the page shows "Incorrect username or password").
    public User? Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        string key = username.Trim().ToLower();
        var user = _db.Connection.Table<User>().FirstOrDefault(u => u.Username == key);

        if (user == null || !user.IsActive)
        {
            return null;
        }

        return PasswordHasher.Verify(password, user.PasswordHash, user.PasswordSalt) ? user : null;
    }

    // Creates a login account. Throws if the username is already taken.
    public User CreateUser(string username, string password, string role, string displayName, int? teacherId = null)
    {
        string key = username.Trim().ToLower();

        if (key.Length < 3) throw new ArgumentException("Username must be at least 3 characters.");
        if (password.Length < 6) throw new ArgumentException("Password must be at least 6 characters.");
        if (UsernameExists(key)) throw new InvalidOperationException($"Username '{key}' is already taken.");

        var (hash, salt) = PasswordHasher.Hash(password);
        var user = new User
        {
            Username = key,
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = role,
            DisplayName = displayName,
            IsActive = true,
            TeacherId = teacherId,
        };
        _db.Connection.Insert(user);
        return user;
    }

    public bool UsernameExists(string username)
    {
        string key = username.Trim().ToLower();
        return _db.Connection.Table<User>().Any(u => u.Username == key);
    }

    public void ChangePassword(int userId, string newPassword)
    {
        if (newPassword.Length < 6) throw new ArgumentException("Password must be at least 6 characters.");

        var user = _db.Connection.Get<User>(userId);
        var (hash, salt) = PasswordHasher.Hash(newPassword);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;
        _db.Connection.Update(user);
    }

    // Blocks (or unblocks) a login without deleting it.
    public void SetActive(int userId, bool isActive)
    {
        var user = _db.Connection.Get<User>(userId);
        user.IsActive = isActive;
        _db.Connection.Update(user);
    }

    public User? GetByTeacherId(int teacherId)
    {
        return _db.Connection.Table<User>().FirstOrDefault(u => u.TeacherId == teacherId);
    }

    public List<User> GetAll()
    {
        return _db.Connection.Table<User>().OrderBy(u => u.Username).ToList();
    }
}
