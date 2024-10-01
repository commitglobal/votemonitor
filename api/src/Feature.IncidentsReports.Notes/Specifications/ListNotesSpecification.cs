using Ardalis.Specification;

namespace Feature.IncidentsReports.Notes.Specifications;

public sealed class ListNotesSpecification : Specification<IncidentReportNoteAggregate, IncidentReportNoteModel>
{
    public ListNotesSpecification(Guid electionRoundId, Guid incidentReportId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId && x.FormId == incidentReportId);
        
        Query.Select(note => IncidentReportNoteModel.FromEntity(note));
    }
}