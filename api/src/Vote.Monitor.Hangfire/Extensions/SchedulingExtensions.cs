using Hangfire;
using Vote.Monitor.Hangfire.RecurringJobs;

namespace Vote.Monitor.Hangfire.Extensions;

public static class SchedulingExtensions
{
    public static void ScheduleRecurringJobs(this WebApplication application)
    {
        // Create a new scope to retrieve scoped services
        using var scope = application.Services.CreateScope();
        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

        recurringJobManager
            .AddOrUpdate<AuditLogCleanerJob>(RecurringJobNames.AuditLogCleaner, x => x.Run(), Cron.Daily);

        recurringJobManager
            .AddOrUpdate<ExportedDataCleanerJob>(RecurringJobNames.ExportedDataCleaner, x => x.Run(), Cron.Daily);

        recurringJobManager
            .AddOrUpdate<ExportedDataFailerJob>(RecurringJobNames.ExportedDataFailer, x => x.Run(), "*/15 * * * *");

        recurringJobManager
            .AddOrUpdate<ImportValidationErrorsCleanerJob>(RecurringJobNames.ImportValidationErrorsCleaner, x => x.Run(), Cron.Daily);
    }
}
