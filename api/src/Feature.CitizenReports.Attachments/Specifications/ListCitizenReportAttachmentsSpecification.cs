using Ardalis.Specification;

namespace Feature.CitizenReports.Attachments.Specifications;

public sealed class ListCitizenReportAttachmentsSpecification : Specification<CitizenReportAttachmentAggregate>
{
    public ListCitizenReportAttachmentsSpecification(Guid electionRoundId, Guid citizenReportId, Guid formId)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId
                        && x.FormId == formId
                        && x.CitizenReportId == citizenReportId
                        && !x.IsDeleted
                        && x.IsCompleted);
    }
}