using Ardalis.Specification;

namespace Feature.IssueReports.Attachments.Specifications;

public sealed class ListIssueReportAttachmentsSpecification : Specification<IssueReportAttachmentAggregate>
{
    public ListIssueReportAttachmentsSpecification(Guid electionRoundId, Guid issueReportId, Guid formId)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId
                        && x.FormId == formId
                        && x.IssueReportId == issueReportId
                        && !x.IsDeleted
                        && x.IsCompleted);
    }
}