using Hangfire;
using Job.Contracts;

namespace Vote.Monitor.Core.Services.Hangfire;

public class HangfireJobService(IBackgroundJobClient backgroundJobClient) : IJobService
{
    public void SendEmail(string to, string subject, string body)
    {
        backgroundJobClient.Enqueue<ISendEmailJob>(job => job.SendAsync(to, subject, body));
    }

    public void ExportFormSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId)
    {
        backgroundJobClient.Enqueue<IExportFormSubmissionsJob>(job => job.ExportFormSubmissions(electionRoundId, ngoId, exportedDataId, CancellationToken.None));
    }
}
