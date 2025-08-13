using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Feature.Forms.Specifications;

public sealed class GetPsiFormById : SingleResultSpecification<PollingStationInformationForm>
{
    public GetPsiFormById(Guid electionRoundId, Guid id)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId && x.Id == id);
    }
}
