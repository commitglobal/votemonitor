using Ardalis.Specification;

namespace Feature.CitizenReports.Notes.Specifications;

public sealed class GetNoteByIdSpecification : SingleResultSpecification<CitizenReportNoteAggregate>
{
    public GetNoteByIdSpecification(Guid electionRoundId, Guid citizenReportId, Guid id)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId && x.CitizenReportId == citizenReportId && x.Id == id);
    }
}