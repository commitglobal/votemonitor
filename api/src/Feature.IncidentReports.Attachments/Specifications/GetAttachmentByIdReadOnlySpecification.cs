using Ardalis.Specification;

namespace Feature.IncidentReports.Attachments.Specifications;

public sealed class GetAttachmentByIdReadOnlySpecification : SingleResultSpecification<IncidentReportAttachmentAggregate>
{
    public GetAttachmentByIdReadOnlySpecification(Guid electionRoundId, Guid incidentReportId, Guid attachmentId)
    {
        Query.AsNoTracking();

        Query.Where(x => x.ElectionRoundId == electionRoundId
                         && x.IncidentReportId == incidentReportId
                         && x.Id == attachmentId);
    }
}