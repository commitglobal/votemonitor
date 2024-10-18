using Hangfire;
using Job.Contracts;
using Job.Contracts.Jobs;

namespace Vote.Monitor.Api.Hangfire;

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

    public void EnqueueExportQuickReports(Guid electionRoundId, Guid ngoId, Guid exportedDataId)
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

    public void EnqueueExportIncidentReports(Guid electionRoundId, Guid ngoId, Guid exportedDataId)
    {
        backgroundJobClient.Enqueue<IExportIncidentReportsJob>(job =>
            job.Run(electionRoundId, ngoId, exportedDataId, CancellationToken.None));
    }

    public void EnqueueSendNotifications(List<string> userIdentifiers, string title, string body)
    {
        backgroundJobClient.Enqueue<ISendNotificationJob>(job =>
            job.SendAsync(userIdentifiers, title, body, CancellationToken.None));
    }
}