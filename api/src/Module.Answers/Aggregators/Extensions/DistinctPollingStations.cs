namespace Module.Answers.Aggregators.Extensions;

/// <summary>
/// Collects the distinct polling stations where each option / value was picked.
/// </summary>
internal sealed class DistinctPollingStations<TKey> where TKey : notnull
{
    private readonly Dictionary<TKey, HashSet<Guid>> _pollingStations;

    /// <param name="keys">The options / values, so that the ones never picked are counted as 0.</param>
    public DistinctPollingStations(IEnumerable<TKey> keys)
    {
        _pollingStations = keys.ToDictionary(key => key, _ => new HashSet<Guid>());
    }

    public void Add(TKey key, Guid pollingStationId)
    {
        if (!_pollingStations.TryGetValue(key, out var pollingStations))
        {
            pollingStations = [];
            _pollingStations[key] = pollingStations;
        }

        pollingStations.Add(pollingStationId);
    }

    public IReadOnlyDictionary<TKey, int> ToHistogram() =>
        _pollingStations.ToDictionary(x => x.Key, x => x.Value.Count).AsReadOnly();
}
