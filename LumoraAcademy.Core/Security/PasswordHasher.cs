using System.Security.Cryptography;

namespace LumoraAcademy.Core.Security;

// Turns a password into a hash so the real password is never stored (NFR-01).
// Uses PBKDF2 with a random salt, which is built into .NET.
public static class PasswordHasher
{
    private const int Iterations = 100_000;
    private const int HashSize = 32;

    // Returns (hash, salt) as Base64 strings ready to store in the Users table.
    public static (string Hash, string Salt) Hash(string password)
    {
        byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
        byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithmName.SHA256, HashSize);
        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    // True when the typed password matches the stored hash.
    public static bool Verify(string password, string storedHash, string storedSalt)
    {
        if (string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(storedSalt)) return false;

        byte[] saltBytes = Convert.FromBase64String(storedSalt);
        byte[] expected = Convert.FromBase64String(storedHash);
        byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithmName.SHA256, HashSize);

        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
