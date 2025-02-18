namespace Vote.Monitor.Api.Feature.ElectionRound.Specifications;

public sealed class GetActiveElectionRoundSpecification : Specification<ElectionRoundAggregate>
{
    /// <summary>
    /// Checks if there are other election rounds with same title 
    /// </summary>
    /// <remarks>Use this before creating a new election round</remarks>
    /// <param name="countryId"></param>
    /// <param name="title"></param>
    public GetActiveElectionRoundSpecification(Guid countryId, string title)
    {
        Query
            .Where(x => x.Title == title)
            .Where(x => x.CountryId == countryId)
            .Where(x => x.Status != ElectionRoundStatus.Archived);
    }

    /// <summary>
    /// Checks if there are other election rounds with same title excluding current one
    /// </summary>
    /// <remarks>Use this before updating an existing election round</remarks>
    /// <param name="electionRoundId"></param>
    /// <param name="countryId"></param>
    /// <param name="title"></param>
    public GetActiveElectionRoundSpecification(Guid electionRoundId, Guid countryId, string title)
    {
        Query
            .Where(x => x.Id != electionRoundId)
            .Where(x => x.Title == title)
            .Where(x => x.CountryId == countryId)
            .Where(x => x.Status != ElectionRoundStatus.Archived);
    }
}
