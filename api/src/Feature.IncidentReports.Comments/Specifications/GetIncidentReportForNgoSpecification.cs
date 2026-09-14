using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;

namespace Feature.IncidentReports.Comments.Specifications;

public sealed class GetIncidentReportForNgoSpecification : Specification<IncidentReport>
{
    public GetIncidentReportForNgoSpecification(Guid electionRoundId, Guid ngoId, Guid incidentReportId)
    {
        Query.Where(x => x.Id == incidentReportId
                         && x.ElectionRoundId == electionRoundId
                         && x.MonitoringObserver.MonitoringNgo.NgoId == ngoId
                         && x.MonitoringObserver.MonitoringNgo.ElectionRoundId == electionRoundId);
    }
}
