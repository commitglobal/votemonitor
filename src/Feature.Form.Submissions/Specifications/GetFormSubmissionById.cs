using Ardalis.Specification;

namespace Feature.Form.Submissions.Specifications;

public sealed class GetFormSubmissionById : SingleResultSpecification<FormSubmission>
{
    public GetFormSubmissionById(Guid electionRoundId, Guid observerId, Guid id)
    {
        Query.Where(x =>
            x.ElectionRoundId == electionRoundId
            && x.MonitoringObserver.ObserverId == observerId
            && x.Id == id);
    }
}
