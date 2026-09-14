using Feature.Form.Submission.Comments.Specifications;

namespace Feature.Form.Submission.Comments.Update;

public class Endpoint(
    IRepository<FormSubmissionCommentAggregate> repository)
    : Endpoint<Request, Results<Ok<FormSubmissionCommentModel>, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}/comments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("form-submission-comments"));
        Summary(s => { s.Summary = "Updates a form submission comment. Only the author can update it."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<FormSubmissionCommentModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var comment = await repository.FirstOrDefaultAsync(
            new GetCommentByIdSpecification(req.ElectionRoundId, req.SubmissionId,req.UserId, req.Id), ct);

        if (comment is null)
        {
            return TypedResults.NotFound();
        }

        comment.UpdateText(req.Text);
        await repository.UpdateAsync(comment, ct);

        return TypedResults.Ok(FormSubmissionCommentModel.FromEntity(comment));
    }
}
