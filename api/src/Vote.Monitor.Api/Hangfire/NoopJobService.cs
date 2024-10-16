using Job.Contracts;

namespace Vote.Monitor.Api.Hangfire;

internal class NoopJobService : IJobService
{
    public void EnqueueSendEmail(string to, string subject, string body)
    {
    }

    public void EnqueueExportFormSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId)
    {
    }

    public void EnqueueExportQuickReports(Guid electionRoundId, Guid ngoId, Guid exportedDataId)
    {
    }

    public void EnqueueExportPollingStations(Guid electionRoundId, Guid exportedDataId)
    {
    }

    public void EnqueueExportCitizenReports(Guid electionRoundId, Guid ngoId, Guid exportedDataId)
    {
    }

    public void EnqueueExportLocations(Guid electionRoundId, Guid exportedDataId)
    {
    }

    public void EnqueueExportIncidentReports(Guid electionRoundId, Guid ngoId, Guid exportedDataId)
    {
    }
}