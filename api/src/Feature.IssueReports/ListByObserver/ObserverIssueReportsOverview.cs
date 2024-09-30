using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.IssueReportAggregate;

namespace Feature.IssueReports.ListByObserver;

public record ObserverIssueReportsOverview
{
    public Guid MonitoringObserverId { get; init; }
    public string ObserverName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string PhoneNumber { get; init; } = default!;
    public string[] Tags { get; init; } = [];
    public int NumberOfFlaggedAnswers { get; init; }
    public int NumberOfIssuesSubmitted { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<IssueReportFollowUpStatus, string>))]
    public IssueReportFollowUpStatus? FollowUpStatus { get; init; }
}
