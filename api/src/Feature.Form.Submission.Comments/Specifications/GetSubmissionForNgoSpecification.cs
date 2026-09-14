using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Feature.Form.Submission.Comments.Specifications;

public sealed class GetSubmissionForNgoSpecification : Specification<FormSubmission>
{
    public GetSubmissionForNgoSpecification(Guid electionRoundId, Guid ngoId, Guid submissionId)
    {
        Query.Where(x => x.Id == submissionId
                         && x.ElectionRoundId == electionRoundId
                         && x.MonitoringObserver.MonitoringNgo.NgoId == ngoId
                         && x.MonitoringObserver.MonitoringNgo.ElectionRoundId == electionRoundId);
    }
}
