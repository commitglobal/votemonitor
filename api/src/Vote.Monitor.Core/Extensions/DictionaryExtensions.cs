namespace Vote.Monitor.Core.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> createFunc)
    {
        if (!dict.TryGetValue(key, out var value))
        {
            value = createFunc();
            dict.Add(key, value);
        }

        return value;
    }

    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> createFunc, Action<TValue> updateFunc)
    {
        if (!dict.TryGetValue(key, out var value))
        {
            value = createFunc();
            dict.Add(key, value);
        }
        else
        {
            updateFunc(value);
        }
    }
}
