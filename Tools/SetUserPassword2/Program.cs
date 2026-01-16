using System;
using System.Data;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;

// Usage:
// dotnet run --project Tools/SetUserPassword2 <email> <password> [connectionString]

string? email = null;
string? password = null;
string? connectionString = null;

if (args.Length >= 2)
{
    email = args[0];
    password = args[1];
}
if (args.Length >= 3)
{
    connectionString = args[2];
}

if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
{
    Console.WriteLine("Usage: dotnet run --project Tools/SetUserPassword2 <email> <password> [connectionString]");
    return 1;
}

if (string.IsNullOrEmpty(connectionString))
{
    // try to read appsettings.json in repo root (two levels up)
    var jsonPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "..", "appsettings.json");
    if (System.IO.File.Exists(jsonPath))
    {
        try
        {
            var j = System.Text.Json.JsonDocument.Parse(System.IO.File.ReadAllText(jsonPath));
            if (j.RootElement.TryGetProperty("ConnectionStrings", out var cs) && cs.TryGetProperty("DefaultConnection", out var def))
            {
                connectionString = def.GetString();
            }
        }
        catch { }
    }
}

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Connection string not provided and not found in appsettings.json. Provide it as the third argument.");
    return 1;
}

Console.WriteLine($"Setting password for {email}");

string CreateHash(string pwd, out string base64Salt, int iterations = 100_000, int saltSize = 16, int hashSize = 32)
{
    using var rng = RandomNumberGenerator.Create();
    var salt = new byte[saltSize];
    rng.GetBytes(salt);

    using var pbkdf2 = new Rfc2898DeriveBytes(pwd, salt, iterations, HashAlgorithmName.SHA256);
    var hash = pbkdf2.GetBytes(hashSize);

    base64Salt = Convert.ToBase64String(salt);
    return Convert.ToBase64String(hash);
}

try
{
    var hash = CreateHash(password, out var salt);

    using var conn = new SqlConnection(connectionString);
    conn.Open();

    using var cmd = conn.CreateCommand();
    cmd.CommandText = "UPDATE DBUsers SET PasswordHash = @hash, PasswordSalt = @salt, IsActive = 1 WHERE EmailAddress = @email";
    cmd.Parameters.Add(new SqlParameter("@hash", SqlDbType.NVarChar) { Value = hash });
    cmd.Parameters.Add(new SqlParameter("@salt", SqlDbType.NVarChar) { Value = salt });
    cmd.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar) { Value = email });

    var rows = cmd.ExecuteNonQuery();
    if (rows == 0)
    {
        Console.WriteLine("No user updated — check the email address matches a DBUsers.EmailAddress row.");
        return 2;
    }

    Console.WriteLine($"Password set for {email} (rows updated: {rows}).");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
    return 3;
}
