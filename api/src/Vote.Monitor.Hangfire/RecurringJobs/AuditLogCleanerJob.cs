using Job.Contracts.RecurringJobs;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Vote.Monitor.Hangfire.RecurringJobs;

public class AuditLogCleanerJob(VoteMonitorContext context): IAuditLogCleanerJob
{
    public async Task Run()
    {
       await context.AuditTrails
            .Where(x => x.Timestamp.AddDays(90) > DateTime.UtcNow)
            .ExecuteDeleteAsync();
    }
}
