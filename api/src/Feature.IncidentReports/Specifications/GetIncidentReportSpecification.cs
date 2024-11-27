namespace Feature.IncidentReports.Specifications;

public sealed class GetIncidentReportSpecification : SingleResultSpecification<IncidentReportAggregate>
{
    public GetIncidentReportSpecification(Guid electionRoundId, Guid observerId, Guid formId, Guid incidentReportId)
    {
        Query.Where(x =>
            x.ElectionRoundId == electionRoundId
            && x.Form.ElectionRoundId == electionRoundId
            && x.FormId == formId
            && x.MonitoringObserver.ObserverId == observerId
            && x.MonitoringObserver.ElectionRoundId == electionRoundId
            && x.Id == incidentReportId);
    }
}
