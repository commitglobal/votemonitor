namespace Vote.Monitor.Core.Models;

/// <summary>
/// A response that may be served from cache. The cache metadata is null when the response was not cached.
/// </summary>
public abstract class CachedResponse
{
    /// <summary>When the cache entry was created (UTC).</summary>
    public DateTime? CachedAt { get; set; }

    /// <summary>When the cache entry expires (UTC).</summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>Minutes left until the cache entry expires, at the time the response is sent; 0 once expired.</summary>
    public int? ValidForMinutes => ExpiresAt is null
        ? null
        : (int)Math.Max(0, Math.Ceiling((ExpiresAt.Value - DateTime.UtcNow).TotalMinutes));

    public void MarkCached(TimeSpan duration)
    {
        CachedAt = DateTime.UtcNow;
        ExpiresAt = CachedAt.Value.Add(duration);
    }

    /// <summary>
    /// Wraps a cache factory so that every response it creates records when its cache entry was created and expires.
    /// </summary>
    public static Func<CancellationToken, Task<TResponse>> Track<TResponse>(
        Func<CancellationToken, Task<TResponse>> factory,
        TimeSpan duration) where TResponse : CachedResponse?
    {
        return async ct =>
        {
            var response = await factory(ct);
            response?.MarkCached(duration);
            return response;
        };
    }
}
