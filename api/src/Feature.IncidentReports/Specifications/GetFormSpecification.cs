namespace Feature.IncidentReports.Specifications;

public sealed class GetFormSpecification : SingleResultSpecification<FormAggregate>
{
    public GetFormSpecification(Guid electionRondId, Guid formId)
    {
        Query.Where(x => x.ElectionRoundId == electionRondId && x.Id == formId);
        Query.Include(x => x.ElectionRound);
    }
}