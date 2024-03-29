using Ardalis.Specification;

namespace Feature.ObserverGuide.Specifications;

public sealed class GetObserverGuideForNgoAdminSpecification : SingleResultSpecification<ObserverGuideAggregate>
{
    public GetObserverGuideForNgoAdminSpecification(Guid? ngoId, Guid id)
    {
        Query.Where(x => x.MonitoringNgo.NgoId == ngoId
                         && x.Id == id
                         && !x.IsDeleted);
    }
}
