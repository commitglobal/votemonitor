namespace Vote.Monitor.Api.Feature.ElectionRound.Specifications;

public sealed class GetNgoElectionSpecification : Specification<ElectionRoundAggregate, ElectionRoundModel>
{
    public GetNgoElectionSpecification(Guid ngoId)
    {
        Query
            .Include(x => x.MonitoringNgos)
            .ThenInclude(x => x.MonitoringObservers)
            .Where(x => x.Status != ElectionRoundStatus.Archived)
            .Where(x => x.MonitoringNgos.Any(ngo => ngo.Id == ngoId));

        Query.Select(x => new ElectionRoundModel
        {
            Id = x.Id,
            Title = x.Title,
            EnglishTitle = x.EnglishTitle,
            StartDate = x.StartDate,
            Status = x.Status,
            CreatedOn = x.CreatedOn,
            LastModifiedOn = x.LastModifiedOn,
            Country = x.Country.FullName,
            CountryId = x.CountryId
        });
    }
}
