using Ardalis.Specification;

namespace Feature.Form.Submission.SMS.Specifications;
public class GetFormSpecification : SingleResultSpecification<FormAggregate>
{
    public GetFormSpecification(string formCode, Guid electionRoundId, Guid monitoringNgoId)
    {
        Query.Where(f => f.Code == formCode && f.ElectionRoundId == electionRoundId && f.MonitoringNgoId == monitoringNgoId);
    }
}
