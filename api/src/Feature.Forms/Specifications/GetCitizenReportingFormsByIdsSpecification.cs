using Vote.Monitor.Domain.Entities.FormBase;

namespace Feature.Forms.Specifications;

public sealed class GetCitizenReportingFormsByIdsSpecification : Specification<FormAggregate>
{
    public GetCitizenReportingFormsByIdsSpecification(Guid electionRoundId, Guid ngoId, IEnumerable<Guid> formIds)
    {
        Query.Where(x =>
            x.ElectionRoundId == electionRoundId
            && x.MonitoringNgo.NgoId == ngoId
            && x.FormType == FormType.CitizenReporting
            && formIds.Contains(x.Id));
    }
}
