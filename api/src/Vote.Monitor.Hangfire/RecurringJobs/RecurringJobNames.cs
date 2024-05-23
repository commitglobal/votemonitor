namespace Vote.Monitor.Hangfire.RecurringJobs;

public class RecurringJobNames
{
    public const string AuditLogCleaner = "audit-log-cleaner";
    public const string ExportedDataCleaner = "exported-data-cleaner";
    public const string ExportedDataFailer = "exported-data-failer";
    public const string ImportValidationErrorsCleaner = "import-validation-errors-cleaner";
}
