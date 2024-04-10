using Ardalis.Specification;

namespace Feature.Form.Submissions.Specifications;

public sealed class GetFormSubmissionForObserverSpecification : Specification<FormSubmission, FormSubmissionModel>
{
    public GetFormSubmissionForObserverSpecification(Guid electionRoundId, Guid observerId, List<Guid>? pollingStationIds)
    {
        Query.Where(x =>
            x.ElectionRoundId == electionRoundId &&
            x.MonitoringObserver.ObserverId == observerId)
            .Where(x => pollingStationIds.Contains(x.PollingStationId), pollingStationIds != null && pollingStationIds.Any());

        Query.Select(x => FormSubmissionModel.FromEntity(x));

    }
}
