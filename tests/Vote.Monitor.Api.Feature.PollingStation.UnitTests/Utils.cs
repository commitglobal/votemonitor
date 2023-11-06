using System.Text;

namespace Vote.Monitor.Api.Feature.PollingStation.UnitTests;

public static class Utils
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

    public static IOptions<TOptions> AsIOption<TOptions>(this TOptions optionInstance) where TOptions : class, new()
    {
        return Microsoft.Extensions.Options.Options.Create(optionInstance);
    }

    public static IEnumerable<T> Repeat<T>(this T value, int times)
    {
        return Enumerable.Range(1, times).Select(_ => value);
    }
}
