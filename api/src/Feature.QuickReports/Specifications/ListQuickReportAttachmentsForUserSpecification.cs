using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Feature.QuickReports.Specifications;

public sealed class ListQuickReportAttachmentsForUserSpecification : Specification<QuickReportAttachment>
{
    public ListQuickReportAttachmentsForUserSpecification(Guid electionRoundId, Guid observerId, params Guid[] quickReportIds)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId && x.MonitoringObserver.ObserverId== observerId && quickReportIds.Contains(x.QuickReportId));
    }
}
