using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Vote.Monitor.Domain;

public class AuditingInterceptor : ISaveChangesInterceptor
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ITimeProvider _timeProvider;

    public AuditingInterceptor(ICurrentUserProvider currentUserProvider, 
        ITimeProvider timeProvider)
    {
        _currentUserProvider = currentUserProvider;
        _timeProvider = timeProvider;
    }

    public ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AddAuditTimeStamps(eventData);

        return ValueTask.FromResult(result);
    }

    public InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        AddAuditTimeStamps(eventData);

        return result;
    }

    private void AddAuditTimeStamps(DbContextEventData eventData)
    {
        var auditableEntries = eventData.Context?.ChangeTracker.Entries<AuditableBaseEntity>() 
                               ?? Enumerable.Empty<EntityEntry<AuditableBaseEntity>>();
        var userId = _currentUserProvider.GetUserId();

        foreach (var entry in auditableEntries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedOn = _timeProvider.UtcNow;
                    entry.Entity.LastModifiedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedOn = _timeProvider.UtcNow;
                    entry.Entity.LastModifiedBy = userId;
                    break;
            }
        }
    }
}
