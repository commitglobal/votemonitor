using Vote.Monitor.Api.Feature.ElectionRound.Monitoring;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.Api.Feature.ElectionRound.Specifications;

public sealed class GetNgoElectionSpecification : Specification<MonitoringNgo, NgoElectionRoundView>
{
    public GetNgoElectionSpecification(Guid ngoId)
    {
        Query
            .Include(x => x.ElectionRound)
            .Where(x => x.NgoId == ngoId);

        Query.Select(x => new NgoElectionRoundView
        {
            MonitoringNgoId = x.Id,
            ElectionRoundId = x.ElectionRoundId,
            Title = x.ElectionRound.Title,
            EnglishTitle = x.ElectionRound.EnglishTitle,
            StartDate = x.ElectionRound.StartDate,
            Country = x.ElectionRound.Country.FullName,
            CountryId = x.ElectionRound.CountryId,
            IsMonitoringNgoForCitizenReporting = x.ElectionRound.CitizenReportingEnabled &&
                                                 x.ElectionRound.MonitoringNgoForCitizenReporting.NgoId == ngoId,
            Status = x.ElectionRound.Status,
        });
    }
}