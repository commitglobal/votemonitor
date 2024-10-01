using Ardalis.Specification;

namespace Feature.IncidentsReports.Notes.Specifications;

public sealed class GetNoteByIdSpecification : SingleResultSpecification<IncidentReportNoteAggregate>
{
    public GetNoteByIdSpecification(Guid electionRoundId, Guid id)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId && x.Id == id);
    }
}
