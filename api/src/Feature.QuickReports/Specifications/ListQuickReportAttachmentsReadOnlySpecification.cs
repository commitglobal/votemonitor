using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Feature.QuickReports.Specifications;

public sealed class ListQuickReportAttachmentsReadOnlySpecification : Specification<QuickReportAttachment>
{
    public ListQuickReportAttachmentsReadOnlySpecification(Guid electionRoundId, params Guid[] quickReportIds)
    {
        Query.AsNoTracking();

        Query
            .Where(x => x.ElectionRoundId == electionRoundId && quickReportIds.Contains(x.QuickReportId))
            .Where(x=>x.IsDeleted == false)
            .Where(x=>x.IsCompleted);
    }
}
