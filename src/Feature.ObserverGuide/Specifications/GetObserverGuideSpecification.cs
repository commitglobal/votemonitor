using Ardalis.Specification;

namespace Feature.ObserverGuide.Specifications;

public sealed class GetObserverGuideSpecification : SingleResultSpecification<ObserverGuideAggregate>
{
    public GetObserverGuideSpecification(Guid? observerId, Guid id)
    {
        Query.Include(x => x.MonitoringNgo)
            .ThenInclude(x => x.MonitoringObservers)
            .Where(x => x.MonitoringNgo
                            .MonitoringObservers
                            .Any(y => y.ObserverId == observerId)
                        && x.Id == id
                        && !x.IsDeleted);
    }
}
