namespace Vote.Monitor.Api.Feature.ElectionRound.Specifications;

public sealed class GetObserverUpcomingElectionSpecification : Specification<ElectionRoundAggregate, ElectionRoundModel>
{
    public GetObserverUpcomingElectionSpecification(Guid observerId, ITimeProvider timeProvider)
    {
        var today = timeProvider.UtcNowDate;

        Query
            .Include(x => x.MonitoringNgos)
            .ThenInclude(x => x.MonitoringObservers)
            .Where(x=>x.StartDate >= today)
            .Where(x => x.MonitoringNgos.Any(ngo => ngo.MonitoringObservers.Any(o => o.ObserverId == observerId)));

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
