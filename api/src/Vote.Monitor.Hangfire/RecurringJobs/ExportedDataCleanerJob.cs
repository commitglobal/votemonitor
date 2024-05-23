using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Vote.Monitor.Hangfire.RecurringJobs;

public class ExportedDataCleanerJob(VoteMonitorContext context)
{
    public async Task Run()
    {
        await context.ExportedData
            .Where(x => x.CreatedOn < DateTime.UtcNow.AddDays(-7))
            .ExecuteDeleteAsync();
    }
}
