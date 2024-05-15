namespace Vote.Monitor.Hangfire.RecurringJobs;

public interface IAttachmentsRemoverJob
{
    Task Run();
}
