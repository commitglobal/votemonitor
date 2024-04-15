using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;
using Vote.Monitor.Domain.Specifications;

namespace Feature.PollingStation.Information.Specifications;

public sealed class GetPollingStationInformationForNgoSpecification : Specification<PollingStationInformation, PollingStationInformationModel>
{
    public GetPollingStationInformationForNgoSpecification(List.Request request)
    {
        Query.Where(x => x.ElectionRoundId == request.ElectionRoundId 
                         && x.MonitoringObserver.MonitoringNgoId == request.NgoId)
            .ApplyOrdering(request)
            .Paginate(request);

         Query.Select(x => PollingStationInformationModel.FromEntity(x));
    }
}
