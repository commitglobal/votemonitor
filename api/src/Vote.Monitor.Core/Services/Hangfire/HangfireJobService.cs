using Hangfire;
using Job.Contracts;
using Job.Contracts.Jobs;

namespace Vote.Monitor.Core.Services.Hangfire;

public class HangfireJobService(IBackgroundJobClient backgroundJobClient) : IJobService
{
    public void SendEmail(string to, string subject, string body)
    {
        backgroundJobClient.Enqueue<ISendEmailJob>(job => job.SendAsync(to, subject, body));
    }

    public string ExportFormSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId)
    {
        return backgroundJobClient.Enqueue<IExportFormSubmissionsJob>(job => job.ExportFormSubmissions(electionRoundId, ngoId, exportedDataId));
    }
}
