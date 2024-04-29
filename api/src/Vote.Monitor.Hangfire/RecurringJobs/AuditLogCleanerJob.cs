using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Vote.Monitor.Hangfire.RecurringJobs;

public class AuditLogCleanerJob(VoteMonitorContext context) : IAuditLogCleanerJob
{
    public async Task Run()
    {
        await context.AuditTrails
             .Where(x => x.Timestamp < DateTime.UtcNow.AddDays(-30))
             .ExecuteDeleteAsync();
    }
}
