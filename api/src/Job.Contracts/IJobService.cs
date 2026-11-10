namespace Job.Contracts;

public interface IJobService
{
    void EnqueueSendEmail(string to, string subject, string body, string textBody);
    void EnqueueExportFormSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
    void EnqueueExportQuickReports(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
    void EnqueueExportPollingStations(Guid electionRoundId, Guid exportedDataId);
    void EnqueueExportCitizenReports(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
    void EnqueueExportLocations(Guid electionRoundId, Guid exportedDataId);
    void EnqueueExportIncidentReports(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
    void EnqueueSendNotifications(List<string> userIdentifiers, string title, string body);
    void EnqueueExportFormSubmissionsSimplified(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
    void EnqueueExportIncidentReportsSimplified(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
    void EnqueueExportPollingStationInformation(Guid electionRoundId, Guid exportedDataId);
}
