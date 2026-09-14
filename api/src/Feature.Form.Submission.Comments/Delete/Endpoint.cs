using Feature.Form.Submission.Comments.Specifications;

namespace Feature.Form.Submission.Comments.Delete;

public class Endpoint(
    IRepository<FormSubmissionCommentAggregate> repository)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}/comments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("form-submission-comments"));
        Summary(s => { s.Summary = "Deletes a form submission comment. Only the author can delete it."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var comment = await repository.FirstOrDefaultAsync(
            new GetCommentByIdSpecification(req.ElectionRoundId, req.SubmissionId, req.UserId, req.Id), ct);

        if (comment is null)
        {
            return TypedResults.NotFound();
        }

        await repository.DeleteAsync(comment, ct);

        return TypedResults.NoContent();
    }
}
