using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Services.Caching;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.Domain;

/// <summary>
/// Cached locations, polling stations and citizen reporting forms are keyed by a version. When a version changes,
/// removes the cache entries tagged with the previous version once the change is saved, instead of leaving them
/// in the cache until they expire.
/// </summary>
public class CacheVersionInterceptor(ICacheTagInvalidator cacheTagInvalidator, ILogger<CacheVersionInterceptor> logger)
    : SaveChangesInterceptor
{
    private readonly List<string> _pendingTags = [];

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        CollectOutdatedVersionTags(eventData.Context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        CollectOutdatedVersionTags(eventData.Context);
        return ValueTask.FromResult(result);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        var tags = TakePendingTags();
        if (tags.Count > 0)
        {
            try
            {
                cacheTagInvalidator.RemoveByTags(tags);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Could not remove outdated cache entries {@Tags}", tags);
            }
        }

        return result;
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = default)
    {
        var tags = TakePendingTags();
        if (tags.Count > 0)
        {
            try
            {
                // The change is already saved, so do not cancel the cache cleanup half way.
                await cacheTagInvalidator.RemoveByTagsAsync(tags, CancellationToken.None);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Could not remove outdated cache entries {@Tags}", tags);
            }
        }

        return result;
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData) => _pendingTags.Clear();

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        _pendingTags.Clear();
        return Task.CompletedTask;
    }

    private void CollectOutdatedVersionTags(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries<ElectionRound>().Where(x => x.State == EntityState.Modified))
        {
            if (TryGetPreviousVersion(entry, x => x.LocationsVersion, out var locationsVersion))
            {
                _pendingTags.Add(CacheTags.LocationsVersion(entry.Entity.Id, locationsVersion));
            }

            if (TryGetPreviousVersion(entry, x => x.PollingStationsVersion, out var pollingStationsVersion))
            {
                _pendingTags.Add(CacheTags.PollingStationsVersion(entry.Entity.Id, pollingStationsVersion));
            }
        }

        foreach (var entry in context.ChangeTracker.Entries<MonitoringNgo>().Where(x => x.State == EntityState.Modified))
        {
            if (TryGetPreviousVersion(entry, x => x.FormsVersion, out var formsVersion))
            {
                _pendingTags.Add(CacheTags.FormsVersion(entry.Entity.Id, formsVersion));
            }
        }
    }

    private static bool TryGetPreviousVersion<TEntity>(EntityEntry<TEntity> entry,
        System.Linq.Expressions.Expression<Func<TEntity, Guid>> property, out Guid previousVersion) where TEntity : class
    {
        var propertyEntry = entry.Property(property);
        previousVersion = propertyEntry.OriginalValue;

        return propertyEntry.IsModified && propertyEntry.OriginalValue != propertyEntry.CurrentValue;
    }

    private List<string> TakePendingTags()
    {
        var tags = _pendingTags.Distinct().ToList();
        _pendingTags.Clear();
        return tags;
    }
}
