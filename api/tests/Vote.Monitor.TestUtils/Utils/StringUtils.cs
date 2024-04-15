using System.Text;

namespace Vote.Monitor.TestUtils.Utils;

public static class StringUtils
{
    public static string Repeat(this string seed, int times)
    {
        var result = new StringBuilder();
        for (var i = 0; i < times; i++)
        {
            result.Append(seed);
        }
        return result.ToString();
    }
}
