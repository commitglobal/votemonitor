using Ardalis.Specification;

namespace Feature.CitizenReports.Guides.Specifications;

public sealed class GetObserverGuideByIdSpecification : SingleResultSpecification<CitizenReportGuideAggregate>
{
    public GetObserverGuideByIdSpecification(Guid electionRoundId, Guid id)
    {
        Query
            .Where(x =>
                x.ElectionRoundId == electionRoundId
                && x.Id == id
                && !x.IsDeleted);
    }
}