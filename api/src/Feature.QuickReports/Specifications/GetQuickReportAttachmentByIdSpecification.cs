using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Feature.QuickReports.Specifications;

public sealed class GetQuickReportAttachmentByIdSpecification : SingleResultSpecification<QuickReportAttachment>
{
    public GetQuickReportAttachmentByIdSpecification(Guid electionRoundId, Guid observerId, Guid quickReportId, Guid id)
    {
        Query.Where(qr => qr.Id == id
                          && qr.QuickReportId == quickReportId
                          && qr.ElectionRoundId == electionRoundId
                          && qr.MonitoringObserver.ObserverId == observerId);
    }
}
