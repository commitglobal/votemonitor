using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Vote.Monitor.Domain;

public class AuditTrailInterceptor : ISaveChangesInterceptor
{
    private readonly ISerializerService _serializerService;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ITimeProvider _timeProvider;

    public AuditTrailInterceptor(ISerializerService serializerService,
        ICurrentUserProvider currentUserProvider,
        ITimeProvider timeProvider)
    {
        _serializerService = serializerService;
        _currentUserProvider = currentUserProvider;
        _timeProvider = timeProvider;
    }

    public ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AddAuditTrailsOnSaving(eventData);

        return ValueTask.FromResult(result);
    }

    public InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        AddAuditTrailsOnSaving(eventData);

        return result;
    }

    private void AddAuditTrailsOnSaving(DbContextEventData eventData)
    {
        var userId = _currentUserProvider.GetUserId();

        var trailEntries = new List<AuditTrail>();

        var auditableEntries = eventData.Context?.ChangeTracker.Entries<AuditableBaseEntity>()
                                   .Where(e => e.State is
                                       EntityState.Added
                                       or EntityState.Deleted
                                       or EntityState.Modified)
                                   .ToList()
                               ?? Enumerable.Empty<EntityEntry<AuditableBaseEntity>>();

        foreach (var entry in auditableEntries)
        {
            var trailEntry = new AuditTrail(entry, _serializerService, _timeProvider)
            {
                TableName = entry.Entity.GetType().Name,
                UserId = userId
            };
            trailEntries.Add(trailEntry);
            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    trailEntry.TemporaryProperties.Add(property);
                    continue;
                }

                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    trailEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        trailEntry.TrailType = TrailType.Create;
                        trailEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        trailEntry.TrailType = TrailType.Delete;
                        trailEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified && property.OriginalValue?.Equals(property.CurrentValue) == false)
                        {
                            trailEntry.ChangedColumns.Add(propertyName);
                            trailEntry.TrailType = TrailType.Update;
                            trailEntry.OldValues[propertyName] = property.OriginalValue;
                            trailEntry.NewValues[propertyName] = property.CurrentValue;
                        }

                        break;
                }
            }
        }

        var voteMonitorContext = eventData.Context as VoteMonitorContext;

        foreach (var auditEntry in trailEntries.Where(e => !e.HasTemporaryProperties))
        {
            voteMonitorContext!.AuditTrails.Add(auditEntry.ToAuditTrail());
        }
    }
}
