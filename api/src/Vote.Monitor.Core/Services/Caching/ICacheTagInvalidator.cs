namespace Vote.Monitor.Core.Services.Caching;

/// <summary>
/// Removes the cache entries carrying any of the given tags.
/// </summary>
public interface ICacheTagInvalidator
{
    Task RemoveByTagsAsync(IReadOnlyCollection<string> tags, CancellationToken ct = default);
    void RemoveByTags(IReadOnlyCollection<string> tags);
}

/// <summary>
/// Used where no cache is registered (e.g. background jobs).
/// </summary>
public sealed class NoopCacheTagInvalidator : ICacheTagInvalidator
{
    public Task RemoveByTagsAsync(IReadOnlyCollection<string> tags, CancellationToken ct = default) => Task.CompletedTask;

    public void RemoveByTags(IReadOnlyCollection<string> tags)
    {
    }
}
