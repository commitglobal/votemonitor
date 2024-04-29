namespace Vote.Monitor.Core.Extensions;

public static class StringToMemoryStreamExtensions
{
    public static MemoryStream ToMemoryStream(this string input)
    {
        return new MemoryStream(Convert.FromBase64String(input));
    }
}
