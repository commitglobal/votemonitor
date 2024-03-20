using Ardalis.Specification;

namespace Feature.PollingStation.Information.Form.Specifications;

public sealed class PollingStationInformationModelSpecification : SingleResultSpecification<PollingStationInfoFormAggregate, PollingStationInformationFormModel>
{
    public PollingStationInformationModelSpecification(Guid electionRoundId)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId);

        Query.Select(x => PollingStationInformationFormModel.FromEntity(x));
    }
}
