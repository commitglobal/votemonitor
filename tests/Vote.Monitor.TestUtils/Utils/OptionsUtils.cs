using Microsoft.Extensions.Options;

namespace Vote.Monitor.TestUtils.Utils;

public static class OptionsUtils
{
    public static IOptions<TOptions> AsIOption<TOptions>(this TOptions optionInstance) where TOptions : class, new()
    {
        return Options.Create(optionInstance);
    }
}
