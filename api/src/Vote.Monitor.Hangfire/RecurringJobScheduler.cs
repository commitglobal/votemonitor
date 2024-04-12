using System.Threading;
using Hangfire;
using Vote.Monitor.Domain.Seeders;
using Vote.Monitor.Domain;
using Vote.Monitor.Hangfire.RecurringJobs;
using Job.Contracts.RecurringJobs;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Hangfire;

public static class RecurringJobScheduler
{
    public static void ScheduleRecurringJobs(this WebApplication application)
    {
        // Create a new scope to retrieve scoped services
        using var scope = application.Services.CreateScope();
        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

        recurringJobManager
            .AddOrUpdate<IAuditLogCleanerJob>(RecurringJobNames.AuditLogCleaner, x => x.Run(), Cron.Daily);
    }
}
