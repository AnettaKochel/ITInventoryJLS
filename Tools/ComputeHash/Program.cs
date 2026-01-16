using System;
using System.Security.Cryptography;

// Usage: dotnet run --project Tools/ComputeHash "password"

if (args.Length < 1)
{
    Console.WriteLine("Usage: dotnet run --project Tools/ComputeHash <password>");
    return 1;
}
var password = args[0];
int iterations = 100_000;
int saltSize = 16;
int hashSize = 32;
using var rng = RandomNumberGenerator.Create();
var salt = new byte[saltSize];
rng.GetBytes(salt);
using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
var hash = pbkdf2.GetBytes(hashSize);
var bSalt = Convert.ToBase64String(salt);
var bHash = Convert.ToBase64String(hash);
Console.WriteLine(bHash);
Console.WriteLine(bSalt);
Console.WriteLine(iterations);
return 0;