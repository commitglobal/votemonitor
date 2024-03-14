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
            Country = electionRound.Country.FullName,
            Title = electionRound.Title,
            EnglishTitle = electionRound.EnglishTitle,
            Status = electionRound.Status,
            StartDate = electionRound.StartDate,
            LastModifiedOn = electionRound.LastModifiedOn,
            CreatedOn = electionRound.CreatedOn,

            MonitoringNgos = electionRound.MonitoringNgos.Select(ngo => new MonitoringNgoModel
            {
                NgoId = ngo.NgoId,
                Name = ngo.Ngo.Name,
                Status = ngo.Ngo.Status,
                MonitoringObservers = ngo.MonitoringObservers.Select(observer => new MonitoringObserverModel
                {
                    ObserverId = observer.ObserverId,
                    Name = observer.Observer.Name,
                    Status = observer.Observer.Status
                }).ToList(),
            }).ToList()
        });
    }
}
