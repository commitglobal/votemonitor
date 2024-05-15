namespace Feature.Forms.Specifications;

public sealed class GetFormByIdSpecification : SingleResultSpecification<FormAggregate>
{
    public GetFormByIdSpecification(Guid electionRoundId, Guid ngoId, Guid id)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId && x.MonitoringNgo.NgoId == ngoId && x.Id == id);
    }
}
