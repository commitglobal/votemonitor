using Ardalis.Specification;

namespace Feature.ObserverGuide.Specifications;

public sealed class GetObserverGuidesForNgoAdminSpecification : Specification<ObserverGuideAggregate>
{
    public GetObserverGuidesForNgoAdminSpecification(Guid? ngoId)
    {
        Query.Where(x => x.MonitoringNgo.NgoId == ngoId && !x.IsDeleted);
    }
}
