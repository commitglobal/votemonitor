using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Feature.PollingStation.Information.Specifications;

public sealed class GetPollingStationInformationByIdSpecification : SingleResultSpecification<PollingStationInformation, PollingStationInformationModel>
{
    public GetPollingStationInformationByIdSpecification(Guid electionRoundId, Guid id, Guid? ngoId)
    {
        Query.AsNoTracking();

        Query.Where(x => x.ElectionRoundId == electionRoundId && x.Id == id);

        if (ngoId.HasValue)
        {
            Query.Where(x => x.MonitoringObserver.MonitoringNgo.NgoId == ngoId.Value);
        }

        Query.Select(x => PollingStationInformationModel.FromEntity(x));
    }
}
