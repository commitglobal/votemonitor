using Ardalis.Specification;

namespace Feature.PollingStation.Information.Forms.Specifications;

public sealed class PollingStationInformationModelSpecification : SingleResultSpecification<PollingStationInfoFormAggregate, PollingStationInformationFormModel>
{
    public PollingStationInformationModelSpecification(Guid electionRoundId)
    {
        Query.AsNoTracking();

        Query
            .Where(x => x.ElectionRoundId == electionRoundId);

        Query.Select(x => PollingStationInformationFormModel.FromEntity(x));
    }
}
