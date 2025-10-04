using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Form.Submission.SMS.Specifications;
public class GetMonitoringObserver : SingleResultSpecification<MonitoringObserver>
{
    public GetMonitoringObserver(string phoneNumber, Guid monitoringNgoId, Guid electionRoundId)
    {
        Query.Where(mo => mo.PhoneNumber == phoneNumber && mo.MonitoringNgoId == monitoringNgoId && mo.ElectionRoundId == electionRoundId);
    }
}
