using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.Specifications;
public sealed class GetQuickReportByIdSpecification : SingleResultSpecification<QuickReport>
{
    public GetQuickReportByIdSpecification(Guid electionRoundId, Guid observerId, Guid quickReportId)
    {
        Query.Where(qr => qr.ElectionRoundId == electionRoundId && qr.MonitoringObserver.ObserverId== observerId && qr.Id == quickReportId);
    }
}
