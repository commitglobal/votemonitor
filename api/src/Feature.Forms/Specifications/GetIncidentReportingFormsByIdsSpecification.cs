using Vote.Monitor.Domain.Entities.FormBase;

namespace Feature.Forms.Specifications;

public sealed class GetIncidentReportingFormsByIdsSpecification : Specification<FormAggregate>
{
    public GetIncidentReportingFormsByIdsSpecification(Guid electionRoundId, Guid ngoId, IEnumerable<Guid> formIds)
    {
        Query.Where(x =>
            x.ElectionRoundId == electionRoundId
            && x.MonitoringNgo.NgoId == ngoId
            && x.FormType == FormType.IncidentReporting
            && formIds.Contains(x.Id));
    }
}
