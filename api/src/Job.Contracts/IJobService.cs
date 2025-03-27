namespace Job.Contracts;

public interface IJobService
{
    void EnqueueSendEmail(string to, string subject, string body);
    void EnqueueExportFormSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
    void EnqueueExportQuickReports(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
    void EnqueueExportPollingStations(Guid electionRoundId, Guid exportedDataId);
    void EnqueueExportCitizenReports(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
    void EnqueueExportLocations(Guid electionRoundId, Guid exportedDataId);
    void EnqueueExportIncidentReports(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
    void EnqueueSendNotifications(List<string> userIdentifiers, string title, string body);
}
