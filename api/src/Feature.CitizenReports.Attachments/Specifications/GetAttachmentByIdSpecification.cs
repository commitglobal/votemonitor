using Ardalis.Specification;

namespace Feature.CitizenReports.Attachments.Specifications;

public sealed class GetAttachmentByIdSpecification : SingleResultSpecification<CitizenReportAttachmentAggregate>
{
    public GetAttachmentByIdSpecification(Guid electionRoundId, Guid citizenReportId, Guid attachmentId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId
                         && x.CitizenReportId == citizenReportId
                         && x.Id == attachmentId);
    }
}