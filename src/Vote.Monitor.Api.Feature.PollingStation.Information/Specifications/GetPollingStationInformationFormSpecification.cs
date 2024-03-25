using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Specifications;

public sealed class GetPollingStationInformationFormSpecification: SingleResultSpecification<PollingStationInformationForm>
{
    public GetPollingStationInformationFormSpecification(Guid electionRondId)
    {
        Query.Where(x => x.ElectionRoundId == electionRondId);
        Query.Include(x => x.ElectionRound);
    }
}
