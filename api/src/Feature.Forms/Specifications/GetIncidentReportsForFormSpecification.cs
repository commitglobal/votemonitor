using Vote.Monitor.Domain.Entities.IncidentReportAggregate;

namespace Feature.Forms.Specifications;

public sealed class GetIncidentReportsForFormSpecification : Specification<IncidentReport>
{
    public GetIncidentReportsForFormSpecification(Guid electionRoundId, Guid ngoId, Guid formId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId
                         && x.MonitoringObserver.MonitoringNgo.NgoId == ngoId
                         && x.MonitoringObserver.MonitoringNgo.ElectionRoundId == electionRoundId
                         && x.Form.ElectionRoundId == electionRoundId
                         && x.FormId == formId);
    }
}
