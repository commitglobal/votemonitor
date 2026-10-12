using Ardalis.Specification;

namespace Feature.CitizenReports.Notes.Specifications;

public sealed class GetNoteByIdReadOnlySpecification : SingleResultSpecification<CitizenReportNoteAggregate>
{
    public GetNoteByIdReadOnlySpecification(Guid electionRoundId, Guid citizenReportId, Guid id)
    {
        Query.AsNoTracking();

        Query
            .Where(x => x.ElectionRoundId == electionRoundId && x.CitizenReportId == citizenReportId && x.Id == id);
    }
}