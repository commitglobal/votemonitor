using Ardalis.Specification;

namespace Feature.QuickReports.Comments.Specifications;

public sealed class GetCommentByIdSpecification : SingleResultSpecification<QuickReportCommentAggregate>
{
    public GetCommentByIdSpecification(Guid electionRoundId, Guid quickReportId, Guid authorId, Guid id)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Where(x => x.QuickReportId == quickReportId)
            .Where(x => x.CreatedBy == authorId)
            .Where(x => x.Id == id);
    }
}
