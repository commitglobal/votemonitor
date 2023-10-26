namespace Vote.Monitor.Core;

public static class DeterministicGuidExtensions
{
    public static Guid ToGuid(this string text)
    {
        return DeterministicGuid.Create(text);
    }
}
