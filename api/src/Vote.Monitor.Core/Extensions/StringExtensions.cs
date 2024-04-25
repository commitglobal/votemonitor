using System.Text;

namespace Vote.Monitor.Core.Extensions;

public static class StringToMemoryStreamExtensions
{
    public static MemoryStream ToMemoryStream(this string input)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(input ?? ""));
    }
}
