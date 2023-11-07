using Vote.Monitor.Core.Helpers;

namespace Vote.Monitor.Core.Extensions;

public static class DeterministicGuidExtensions
{
    public static Guid ToGuid(this string text)
    {
        return DeterministicGuid.Create(text);
    }
}
