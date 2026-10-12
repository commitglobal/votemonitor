using Ardalis.Specification;

namespace Feature.Form.Submissions.Specifications;

public sealed class GetFormSubmissionReadOnlySpecification : SingleResultSpecification<FormSubmission>
{
    [Obsolete("Will be removed in future version")]
    public GetFormSubmissionReadOnlySpecification(Guid electionRoundId, Guid pollingStationId, Guid formId, Guid observerId)
    {
        Query.AsNoTracking();

        Query.Where(x =>
            x.ElectionRoundId == electionRoundId
            && x.MonitoringObserver.ObserverId == observerId
            && x.MonitoringObserver.ElectionRoundId == electionRoundId
            && x.PollingStationId == pollingStationId
            && x.Form.ElectionRoundId == electionRoundId
            && x.FormId == formId);
    }

    public GetFormSubmissionReadOnlySpecification(Guid electionRoundId,Guid pollingStationId, Guid formId, Guid observerId, Guid submissionId)
    {
        Query.AsNoTracking();

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
