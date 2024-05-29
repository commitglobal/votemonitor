using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Feature.QuickReports.Specifications;

public sealed class ListQuickReportAttachmentsSpecification : Specification<QuickReportAttachment>
{
    public ListQuickReportAttachmentsSpecification(Guid electionRoundId, params Guid[] quickReportIds)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId && quickReportIds.Contains(x.QuickReportId))
            .Where(x=>x.IsDeleted == false)
            .Where(x=>x.IsCompleted);
    }
}
