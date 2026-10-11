using Ardalis.Specification;

namespace Feature.IncidentReports.Notes.Specifications;

public sealed class GetNoteByIdReadOnlySpecification : SingleResultSpecification<IncidentReportNoteAggregate>
{
    public GetNoteByIdReadOnlySpecification(Guid electionRoundId, Guid id)
    {
        Query.AsNoTracking();

        Query
            .Where(x => x.ElectionRoundId == electionRoundId && x.Id == id);
    }
}
