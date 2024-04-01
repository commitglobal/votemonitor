using Ardalis.Specification;

namespace Feature.ObserverGuide.Specifications;

public sealed class GetObserverGuidesSpecification : Specification<ObserverGuideAggregate>
{
    public GetObserverGuidesSpecification(Guid? observerId)
    {
        Query.Include(x=> x.MonitoringNgo)
            .ThenInclude(x=> x.MonitoringObservers)
            .Where(x => x.MonitoringNgo
                .MonitoringObservers
                .Any(y=> y.ObserverId == observerId) 
            && !x.IsDeleted);
    }
}
