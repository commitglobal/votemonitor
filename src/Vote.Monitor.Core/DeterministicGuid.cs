using System.Security.Cryptography;
using System.Text;

namespace Vote.Monitor.Core;

public class DeterministicGuid
{
    public static Guid Create(string text)
    {
        var hash = SHA256
            .Create()
            .ComputeHash(Encoding.UTF8.GetBytes(text));

        var guid = new Guid(hash.Take(16).ToArray());

        return guid;
    }
}
