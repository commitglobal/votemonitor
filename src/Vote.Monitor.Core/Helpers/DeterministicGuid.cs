using System.Security.Cryptography;
using System.Text;

namespace Vote.Monitor.Core.Helpers;

public class DeterministicGuid
{
    public static Guid Create(string text)
    {
        using var hasher = SHA256.Create();

        var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(text));

        var guid = new Guid(hash.Take(16).ToArray());

        return guid;
    }
}
