using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;

namespace Feature.Form.Submission.SMS.Specifications;
public class GetPollingStation : SingleResultSpecification<PollingStation>
{
    public GetPollingStation(string pollingStationCode, Guid electionRoundId)
    {
        Query.Where(ps => ps.SecondaryId == pollingStationCode && ps.ElectionRoundId == electionRoundId);
    }
}
