using Ardalis.Specification;

namespace Feature.Citizen.Guides.Specifications;

public sealed class GetCitizenGuideByIdSpecification : SingleResultSpecification<CitizenGuideAggregate>
{
    public GetCitizenGuideByIdSpecification(Guid electionRoundId, Guid id)
    {
        Query
            .Where(x =>
                x.ElectionRoundId == electionRoundId
                && x.Id == id
                && !x.IsDeleted);
    }
}