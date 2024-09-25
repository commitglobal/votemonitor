﻿using Hangfire;
using Vote.Monitor.Hangfire.RecurringJobs;
using Job.Contracts.RecurringJobs;

namespace Vote.Monitor.Hangfire.Extensions;

public static class SchedulingExtensions
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
