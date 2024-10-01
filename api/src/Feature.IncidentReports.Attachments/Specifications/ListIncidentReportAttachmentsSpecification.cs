using Ardalis.Specification;

namespace Feature.IncidentReports.Attachments.Specifications;

public sealed class ListIncidentReportAttachmentsSpecification : Specification<IncidentReportAttachmentAggregate>
{
    public ListIncidentReportAttachmentsSpecification(Guid electionRoundId, Guid incidentReportId, Guid formId)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId
                        && x.FormId == formId
                        && x.IncidentReportId == incidentReportId
                        && !x.IsDeleted
                        && x.IsCompleted);
    }
}