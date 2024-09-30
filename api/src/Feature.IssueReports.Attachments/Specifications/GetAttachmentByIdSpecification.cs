using Ardalis.Specification;

namespace Feature.IssueReports.Attachments.Specifications;

public sealed class GetAttachmentByIdSpecification : SingleResultSpecification<IssueReportAttachmentAggregate>
{
    public GetAttachmentByIdSpecification(Guid electionRoundId, Guid issueReportId, Guid attachmentId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId
                         && x.IssueReportId == issueReportId
                         && x.Id == attachmentId);
    }
}