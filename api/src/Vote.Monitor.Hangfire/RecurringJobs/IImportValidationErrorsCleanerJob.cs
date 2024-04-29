namespace Vote.Monitor.Hangfire.RecurringJobs;

public interface IImportValidationErrorsCleanerJob
{
    Task Run();
}
