using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Authorization.Policies.Specifications;

internal sealed class GetCitizenReportingMonitoringNgoSpecification : SingleResultSpecification<ElectionRound, MonitoringNgoView>
{
    public GetCitizenReportingMonitoringNgoSpecification(Guid electionRoundId, Guid ngoId)
    {
        Query
            .Include(x => x.MonitoringNgoForCitizenReporting)
            .ThenInclude(x => x.Ngo)
            .Where(x => x.CitizenReportingEnabled && x.Id == electionRoundId &&
                        x.MonitoringNgoForCitizenReporting.NgoId == ngoId)
            .AsNoTracking();

        Query.Select(x => new MonitoringNgoView
        {
            ElectionRoundId = x.Id,
            ElectionRoundStatus = x.Status,
            NgoId = x.MonitoringNgoForCitizenReporting.NgoId,
            NgoStatus = x.MonitoringNgoForCitizenReporting.Ngo.Status,
            MonitoringNgoId = x.MonitoringNgoForCitizenReporting.Id,
            MonitoringNgoStatus = x.MonitoringNgoForCitizenReporting.Status
        });
    }
}