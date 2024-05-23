using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;

namespace Vote.Monitor.Hangfire.RecurringJobs;

/// <summary>
/// Fails any started data export that is older than 1 h
/// </summary>
/// <param name="context"></param>
public class ExportedDataFailerJob(VoteMonitorContext context)
{
    public async Task Run()
    {
        await context.ExportedData
            .Where(x => x.CreatedOn < DateTime.UtcNow.AddHours(-1) && x.ExportStatus == ExportedDataStatus.Started)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.ExportStatus, ExportedDataStatus.Failed));
    }
}
