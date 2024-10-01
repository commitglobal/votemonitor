using Ardalis.Specification;

namespace Feature.IncidentsReports.Attachments.Specifications;

public sealed class GetAttachmentByIdSpecification : SingleResultSpecification<IncidentReportAttachmentAggregate>
{
    public GetAttachmentByIdSpecification(Guid electionRoundId, Guid incidentReportId, Guid attachmentId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId
                         && x.IncidentReportId == incidentReportId
                         && x.Id == attachmentId);
    }
}