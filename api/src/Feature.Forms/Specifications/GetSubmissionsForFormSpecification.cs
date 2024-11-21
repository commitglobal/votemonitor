using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Feature.Forms.Specifications;

public sealed class GetSubmissionsForFormSpecification : Specification<FormSubmission>
{
    public GetSubmissionsForFormSpecification(Guid electionRoundId, Guid ngoId, Guid formId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId
                         && x.MonitoringObserver.MonitoringNgo.NgoId == ngoId
                         && x.MonitoringObserver.MonitoringNgo.ElectionRoundId == electionRoundId
                         && x.Form.ElectionRoundId == electionRoundId
                         && x.FormId == formId);
    }
}
