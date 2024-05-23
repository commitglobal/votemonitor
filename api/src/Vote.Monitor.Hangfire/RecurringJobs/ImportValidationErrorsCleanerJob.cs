using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Vote.Monitor.Hangfire.RecurringJobs;

public class ImportValidationErrorsCleanerJob(VoteMonitorContext context)
{
    public async Task Run()
    {
        await context.ImportValidationErrors
            .Where(x => x.CreatedOn < DateTime.UtcNow.AddDays(-1))
            .ExecuteDeleteAsync();
    }
}
