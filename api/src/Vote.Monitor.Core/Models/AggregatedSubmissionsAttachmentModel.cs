namespace Vote.Monitor.Core.Models;

public record AggregatedSubmissionsAttachmentModel : SubmissionAttachmentModel
{
    public Guid MonitoringObserverId { get; init; }
}
