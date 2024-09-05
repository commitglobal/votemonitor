using Ardalis.Specification;

namespace Feature.CitizenReports.Notes.Specifications;

public sealed class GetNoteByIdSpecification : SingleResultSpecification<CitizenReportNoteAggregate>
{
    public GetNoteByIdSpecification(Guid electionRoundId, Guid id)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId && x.Id == id);
    }
}
