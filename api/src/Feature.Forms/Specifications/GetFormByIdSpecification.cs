namespace Feature.Forms.Specifications;

public sealed class GetFormByIdSpecification: SingleResultSpecification<FormAggregate>
{
    public GetFormByIdSpecification(Guid electionRoundId, Guid monitoringNgoId, Guid Id)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId && x.MonitoringNgoId == monitoringNgoId && x.Id == Id);
    }
}
