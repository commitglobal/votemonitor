using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;

namespace Feature.IncidentReports.Attachments.Specifications;

/// <summary>
/// Matches the incident report only when it is completed: a completed (finalized) report is locked,
/// its notes and attachments can no longer be changed by the observer.
/// </summary>
public sealed class CompletedIncidentReportSpecification : Specification<IncidentReport>
{
    public CompletedIncidentReportSpecification(Guid electionRoundId, Guid incidentReportId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId && x.Id == incidentReportId && x.IsCompleted);
    }
}
