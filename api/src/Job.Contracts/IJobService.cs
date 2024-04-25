namespace Job.Contracts;

public interface IJobService
{
    void SendEmail(string to, string subject, string body);
    string ExportFormSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
}
