namespace Vote.Monitor.Api.Feature.ElectionRound.Specifications;

public sealed class GetObserverElectionSpecification : Specification<ElectionRoundAggregate, ElectionRoundModel>
{
    public GetObserverElectionSpecification(Guid observerId)
    {
        Query
            .Include(x => x.MonitoringNgos)
            .ThenInclude(x => x.MonitoringObservers)
            .Where(x => x.Status == ElectionRoundStatus.Started)
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
            CountryId = x.CountryId,
            CountryIso2 = x.Country.Iso2,
            CountryIso3 = x.Country.Iso3,
            CountryName = x.Country.Name,
            CountryFullName = x.Country.FullName,
            CountryNumericCode = x.Country.NumericCode,
            CoalitionId = null,
            CoalitionName = null,
            IsCoalitionLeader = false,
            IsMonitoringNgoForCitizenReporting = false
        });
    }
}
