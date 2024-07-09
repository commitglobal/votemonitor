namespace Feature.Forms.Specifications;

public sealed class GetFormByIdSpecification : SingleResultSpecification<FormAggregate>
{
    public GetFormByIdSpecification(Guid electionRoundId, Guid id)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId && x.Id == id);
    }
}
