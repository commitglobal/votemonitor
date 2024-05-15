namespace Job.Contracts;

public interface IJobService
{
    void SendEmail(string to, string subject, string body);
    void ExportFormSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
}
