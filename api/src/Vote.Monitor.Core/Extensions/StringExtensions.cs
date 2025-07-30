namespace Vote.Monitor.Core.Extensions;

public static class StringExtensions
{
    public static string ToBase64String(this string str)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(str);
        return System.Convert.ToBase64String(plainTextBytes);
    }
}
