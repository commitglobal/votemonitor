using Ardalis.Specification;

namespace Feature.Form.Submission.Comments.Specifications;

public sealed class GetCommentByIdSpecification : SingleResultSpecification<FormSubmissionCommentAggregate>
{
    public GetCommentByIdSpecification(Guid electionRoundId, Guid submissionId, Guid authorId, Guid id)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Where(x => x.SubmissionId == submissionId)
            .Where(x => x.CreatedBy == authorId)
            .Where(x => x.Id == id);
    }
}
