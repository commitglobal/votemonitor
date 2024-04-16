namespace Vote.Monitor.Answer.Module.Aggregators.Extensions;

internal static class DictionaryHistogramExtensions
{
    public static void IncrementFor<TId>(this Dictionary<TId, int> histogram, TId key) where TId : notnull
    {
        histogram[key] += 1;
    }
}
