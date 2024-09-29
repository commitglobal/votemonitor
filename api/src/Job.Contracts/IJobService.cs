namespace Job.Contracts;

public interface IJobService
{
    void EnqueueSendEmail(string to, string subject, string body);
    void EnqueueExportFormSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
    void EnqueueExportQuickReportsSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
    void EnqueueExportPollingStations(Guid electionRoundId, Guid exportedDataId);
    void EnqueueExportCitizenReports(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
    void EnqueueExportLocations(Guid electionRoundId, Guid exportedDataId);
    void EnqueueExportIssueReports(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
}