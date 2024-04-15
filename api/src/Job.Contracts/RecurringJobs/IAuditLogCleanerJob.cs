namespace Job.Contracts.RecurringJobs;

public interface IAuditLogCleanerJob
{
    Task Run();
}
