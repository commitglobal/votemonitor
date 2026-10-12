using Ardalis.Specification;

namespace Feature.CitizenReports.Attachments.Specifications;

public sealed class GetAttachmentByIdReadOnlySpecification : SingleResultSpecification<CitizenReportAttachmentAggregate>
{
    public GetAttachmentByIdReadOnlySpecification(Guid electionRoundId, Guid citizenReportId, Guid attachmentId)
    {
        Query.AsNoTracking();

        Query.Where(x => x.ElectionRoundId == electionRoundId
                         && x.CitizenReportId == citizenReportId
                         && x.Id == attachmentId);
    }
}