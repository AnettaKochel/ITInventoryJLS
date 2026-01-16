using System;
using System.Security.Cryptography;

namespace ITInventoryJLS.Services
{
    public static class PasswordHasher
    {
        // PBKDF2 with HMACSHA256
        public static void CreateHash(string password, out string base64Hash, out string base64Salt, int iterations = 100_000, int saltSize = 16, int hashSize = 32)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[saltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(hashSize);

            // Store as Base64 strings
            base64Salt = Convert.ToBase64String(salt);
            base64Hash = Convert.ToBase64String(hash);
            // Also include iteration count metadata if desired in future. For now we store raw base64 values.
        }

        public static bool Verify(string password, string base64Hash, string base64Salt, int iterations = 100_000)
        {
            if (string.IsNullOrEmpty(base64Hash) || string.IsNullOrEmpty(base64Salt)) return false;

            var salt = Convert.FromBase64String(base64Salt);
            var expectedHash = Convert.FromBase64String(base64Hash);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var actualHash = pbkdf2.GetBytes(expectedHash.Length);

            // Constant-time comparison
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}
