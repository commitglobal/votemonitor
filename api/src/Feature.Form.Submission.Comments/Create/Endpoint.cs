using Vote.Monitor.Domain.Entities.CoalitionAggregate;

namespace Feature.Form.Submission.Comments.Create;

public class Endpoint(
    IRepository<FormSubmissionCommentAggregate> repository)
    : Endpoint<Request, Results<Ok<FormSubmissionCommentModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}/comments");
        DontAutoTag();
        Options(x => x.WithTags("form-submission-comments"));
        Summary(s =>
        {
            s.Summary = "Creates a comment for a form submission. Omit questionId for a submission-level comment.";
        });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<FormSubmissionCommentModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        // todo: check if submission is from an observer from same ngo 
        var comment = FormSubmissionCommentAggregate.Create(
            req.ElectionRoundId,
            req.SubmissionId,
            req.QuestionId,
            req.Text);

        await repository.AddAsync(comment, ct);

        return TypedResults.Ok(FormSubmissionCommentModel.FromEntity(comment));
    }
}
