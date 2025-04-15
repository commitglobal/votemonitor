namespace Module.Answers.Aggregators.Extensions;

internal static class DictionaryHistogramExtensions
{
    public static void IncrementFor<TId>(this Dictionary<TId, int> histogram, TId key) where TId : notnull
    {
        histogram.TryAdd(key, 0);
        histogram[key] += 1;
    }
}
