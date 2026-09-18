using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Hangfire.RecurringJobs;

public class PendingMonitoringObserverCleanerJob(VoteMonitorContext context)
{
    public async Task Run()
    {
        await context.MonitoringObservers
            .Where(x => x.Status == MonitoringObserverStatus.Pending
                        && x.CreatedOn < DateTime.UtcNow.AddDays(-30))
            .ExecuteDeleteAsync();
    }
}
