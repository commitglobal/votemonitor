using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.Specifications;
public sealed class ListObserverQuickReportsSpecification : Specification<QuickReport>
{
    public ListObserverQuickReportsSpecification(Guid electionRoundId, Guid observerId)
    {
        Query.Where(qr => qr.ElectionRoundId == electionRoundId && qr.MonitoringObserver.ObserverId == observerId);
    }    
}
