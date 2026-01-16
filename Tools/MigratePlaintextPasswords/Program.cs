using System.Data;
using Microsoft.Data.SqlClient;
using ITInventoryJLS.Services;

// Usage: dotnet run --project Tools/MigratePlaintextPasswords [connectionString]
// If no connection string provided, reads from appsettings.json in the root folder.

string? connectionString = null;
if (args.Length > 0)
{
    connectionString = args[0];
}
else
{
    // try to read appsettings.json
    var jsonPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "..", "appsettings.json");
    if (System.IO.File.Exists(jsonPath))
    {
        var j = System.Text.Json.JsonDocument.Parse(System.IO.File.ReadAllText(jsonPath));
        if (j.RootElement.TryGetProperty("ConnectionStrings", out var cs) && cs.TryGetProperty("DefaultConnection", out var def))
        {
            connectionString = def.GetString();
        }
    }
}

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Connection string not provided and not found in appsettings.json. Provide it as the first argument.");
    return 1;
}

Console.WriteLine("Using connection string: " + (connectionString.Length > 50 ? connectionString.Substring(0,50) + "..." : connectionString));

using var conn = new SqlConnection(connectionString);
await conn.OpenAsync();

// Check if DBPassword column exists
var cmdCheck = conn.CreateCommand();
cmdCheck.CommandText = @"
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'DBUsers' AND COLUMN_NAME = 'DBPassword'
";
var hasDbPassword = (await cmdCheck.ExecuteScalarAsync()) != null;

if (!hasDbPassword)
{
    Console.WriteLine("No DBPassword column found; nothing to migrate.");
    return 0;
}

// Read users with non-empty DBPassword
var cmd = conn.CreateCommand();
cmd.CommandText = @"SELECT DBUserID, DBPassword FROM DBUsers WHERE DBPassword IS NOT NULL AND LTRIM(RTRIM(DBPassword)) <> ''";
using var reader = await cmd.ExecuteReaderAsync();
var rows = new System.Collections.Generic.List<(int id, string pwd)>();
while (await reader.ReadAsync())
{
    var id = reader.GetInt32(0);
    var pwd = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
    rows.Add((id, pwd));
}
reader.Close();

Console.WriteLine($"Found {rows.Count} users with plaintext passwords to migrate.");

foreach (var (id,pwd) in rows)
{
    PasswordHasher.CreateHash(pwd, out var hash, out var salt);
    var update = conn.CreateCommand();
    update.CommandText = "UPDATE DBUsers SET PasswordHash = @hash, PasswordSalt = @salt WHERE DBUserID = @id";
    update.Parameters.Add(new SqlParameter("@hash", SqlDbType.NVarChar) { Value = hash });
    update.Parameters.Add(new SqlParameter("@salt", SqlDbType.NVarChar) { Value = salt });
    update.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
    var affected = await update.ExecuteNonQueryAsync();
    Console.WriteLine($"Migrated user {id}, rows affected: {affected}");
}

Console.WriteLine("Migration complete. You can later remove the DBPassword column with a migration if desired.");
return 0;
