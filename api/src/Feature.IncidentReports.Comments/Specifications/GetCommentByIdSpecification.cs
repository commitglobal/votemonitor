using Ardalis.Specification;

namespace Feature.IncidentReports.Comments.Specifications;

public sealed class GetCommentByIdSpecification : SingleResultSpecification<IncidentReportCommentAggregate>
{
    public GetCommentByIdSpecification(Guid electionRoundId, Guid incidentReportId, Guid authorId, Guid id)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Where(x => x.IncidentReportId == incidentReportId)
            .Where(x => x.CreatedBy == authorId)
            .Where(x => x.Id == id);
    }
}
