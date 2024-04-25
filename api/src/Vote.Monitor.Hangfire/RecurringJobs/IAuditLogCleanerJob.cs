namespace Vote.Monitor.Hangfire.RecurringJobs;

public interface IAuditLogCleanerJob
{
    Task Run();
}
