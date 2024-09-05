namespace Vote.Monitor.Api.Feature.ElectionRound.Specifications;

public sealed class GetElectionRoundByIdSpecification : SingleResultSpecification<ElectionRoundAggregate, ElectionRoundModel>
{
    public GetElectionRoundByIdSpecification(Guid id)
    {
        Query
            .Where(x => x.Id == id)
            .Include(x => x.MonitoringNgos)
            .ThenInclude(x => x.Ngo)
            .Include(x => x.MonitoringNgos)
            .ThenInclude(x => x.MonitoringObservers)
            .Include(x => x.Country)
            .AsSplitQuery();

        Query.Select(electionRound => new ElectionRoundModel
        {
            Id = electionRound.Id,
            CountryId = electionRound.CountryId,
            CountryIso2 = electionRound.Country.Iso2,
            CountryIso3 = electionRound.Country.Iso3,
            CountryName = electionRound.Country.Name,
            CountryFullName = electionRound.Country.FullName,
            CountryNumericCode = electionRound.Country.NumericCode,
            Title = electionRound.Title,
            EnglishTitle = electionRound.EnglishTitle,
            Status = electionRound.Status,
            StartDate = electionRound.StartDate,
            LastModifiedOn = electionRound.LastModifiedOn,
            CreatedOn = electionRound.CreatedOn
        });
    }
}
