using System.Security.Cryptography;
using System.Text;

namespace Todo.Domain.Events;

/// <summary>ICS feed token: 32 random bytes as Base64Url (43 characters); only its SHA-256 hex hash is stored (ADR 3.5).</summary>
public static class FeedToken
{
    public static string Generate() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    public static string Hash(string token) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
