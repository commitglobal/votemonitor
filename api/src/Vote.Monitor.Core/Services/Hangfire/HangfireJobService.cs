using Hangfire;
using Job.Contracts;

namespace Vote.Monitor.Core.Services.Hangfire;

public class HangfireJobService(IBackgroundJobClient backgroundJobClient) : IJobService
{
    public void EnqueueSendEmail(string to, string subject, string body)
    {
        backgroundJobClient.Enqueue<ISendEmailJob>(job => job.SendAsync(to, subject, body));
    }

    public void EnqueueExportFormSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId)
    {
        backgroundJobClient.Enqueue<IExportFormSubmissionsJob>(job =>
            job.Run(electionRoundId, ngoId, exportedDataId, CancellationToken.None));
    }

    public void EnqueueExportQuickReportsSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId)
    {
        backgroundJobClient.Enqueue<IExportQuickReportsJob>(job =>
            job.Run(electionRoundId, ngoId, exportedDataId, CancellationToken.None));
    }

    public void EnqueueExportPollingStations(Guid electionRoundId, Guid exportedDataId)
    {
        backgroundJobClient.Enqueue<IExportPollingStationsJob>(job =>
            job.Run(electionRoundId, exportedDataId, CancellationToken.None));
    }

    public void EnqueueExportCitizenReports(Guid electionRoundId, Guid ngoId, Guid exportedDataId)
    {
        backgroundJobClient.Enqueue<IExportCitizenReportsJob>(job =>
            job.Run(electionRoundId, ngoId, exportedDataId, CancellationToken.None));
    }

    public void EnqueueExportLocations(Guid electionRoundId, Guid exportedDataId)
    {
        backgroundJobClient.Enqueue<IExportLocationsJob>(job =>
            job.Run(electionRoundId, exportedDataId, CancellationToken.None));
    }

    public void EnqueueExportIssueReports(Guid electionRoundId, Guid ngoId, Guid exportedDataId)
    {
        backgroundJobClient.Enqueue<IExportIssueReportsJob>(job =>
            job.Run(electionRoundId, ngoId, exportedDataId, CancellationToken.None));
    }
}