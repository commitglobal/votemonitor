namespace Job.Contracts;

public interface IJobService
{
    void SendEmail(string to, string subject, string body);
    void ExportFormSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
    void ExportQuickReportsSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
    void ExportPollingStations(Guid electionRoundId, Guid exportedDataId);
}
