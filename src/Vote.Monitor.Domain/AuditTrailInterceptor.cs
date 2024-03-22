using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Vote.Monitor.Domain;

public class AuditTrailInterceptor : ISaveChangesInterceptor
{
    public ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {

        return ValueTask.FromResult(result);
    }

    public InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {

        return result;
    }
}
