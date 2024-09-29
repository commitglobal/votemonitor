using Ardalis.Specification;

namespace Feature.IssueReports.Notes.Specifications;

public sealed class GetNoteByIdSpecification : SingleResultSpecification<IssueReportNoteAggregate>
{
    public GetNoteByIdSpecification(Guid electionRoundId, Guid id)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId && x.Id == id);
    }
}
