using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;
using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Specifications;

public sealed class GetPollingStationInformationForNgoSpecification : Specification<PollingStationInformation, PollingStationInformationModel>
{
    public GetPollingStationInformationForNgoSpecification(List.Request request)
    {
        Query.Where(x => x.ElectionRoundId == request.ElectionRoundId 
                         && x.MonitoringObserver.InviterNgoId == request.NgoId)
            .ApplyOrdering(request)
            .Paginate(request);

         Query.Select(x => PollingStationInformationModel.FromEntity(x));
    }
}
