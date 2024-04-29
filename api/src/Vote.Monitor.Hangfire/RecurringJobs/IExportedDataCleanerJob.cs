namespace Vote.Monitor.Hangfire.RecurringJobs;

public interface IExportedDataCleanerJob
{
    Task Run();
}
