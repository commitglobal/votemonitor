using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Specifications;

public class GetPollingStationInformationFormSpecification: SingleResultSpecification<PollingStationInformationForm>
{
    public GetPollingStationInformationFormSpecification(Guid electionRondId, Guid formId)
    {
        Query.Where(x => x.ElectionRoundId == electionRondId && x.Id == formId);
    }
}
