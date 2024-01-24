namespace Vote.Monitor.Api.Feature.ElectionRound.Specifications;

public class GetActiveElectionRoundSpecification : Specification<ElectionRoundAggregate>
{
    public GetActiveElectionRoundSpecification(Guid countryId, string title)
    {
        Query
            .Where(x => x.Title == title)
            .Where(x => x.CountryId == countryId)
            .Where(x=>x.Status != ElectionRoundStatus.Archived);
    }
}
