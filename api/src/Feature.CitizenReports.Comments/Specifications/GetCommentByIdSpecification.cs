using Ardalis.Specification;

namespace Feature.CitizenReports.Comments.Specifications;

public sealed class GetCommentByIdSpecification : SingleResultSpecification<CitizenReportCommentAggregate>
{
    public GetCommentByIdSpecification(Guid electionRoundId, Guid citizenReportId, Guid authorId, Guid id)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Where(x => x.CitizenReportId == citizenReportId)
            .Where(x => x.CreatedBy == authorId)
            .Where(x => x.Id == id);
    }
}
