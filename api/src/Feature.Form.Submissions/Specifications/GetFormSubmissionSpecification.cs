using Ardalis.Specification;

namespace Feature.Form.Submissions.Specifications;

public sealed class GetFormSubmissionSpecification : SingleResultSpecification<FormSubmission>
{
    [Obsolete("Will be removed in future version")]
    public GetFormSubmissionSpecification(Guid electionRoundId, Guid pollingStationId, Guid formId, Guid observerId)
    {
        Query.Where(x =>
            x.ElectionRoundId == electionRoundId
            && x.MonitoringObserver.ObserverId == observerId
            && x.MonitoringObserver.ElectionRoundId == electionRoundId
            && x.PollingStationId == pollingStationId
            && x.Form.ElectionRoundId == electionRoundId
            && x.FormId == formId);
    }

    public GetFormSubmissionSpecification(Guid electionRoundId,Guid pollingStationId, Guid formId, Guid observerId, Guid submissionId)
    {
        Query.Where(x =>
            x.ElectionRoundId == electionRoundId
            && x.MonitoringObserver.ObserverId == observerId
            && x.MonitoringObserver.ElectionRoundId == electionRoundId
            && x.PollingStationId == pollingStationId
            && x.Form.ElectionRoundId == electionRoundId
            && x.FormId == formId
            && x.Id == submissionId);
    }
}
