namespace Vote.Monitor.TestUtils.Utils;

public static class EnumerableUtils
{
    public static IEnumerable<T> Repeat<T>(this T value, int times)
    {
        return Enumerable.Range(1, times).Select(_ => value);
    }
}
