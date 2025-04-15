using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Feature.PollingStation.Information.Forms.Specifications;

public sealed class GetPollingStationInformationFormSpecification : SingleResultSpecification<PollingStationInformationForm>
{
    public GetPollingStationInformationFormSpecification(Guid electionRoundId)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId);
    }
}
