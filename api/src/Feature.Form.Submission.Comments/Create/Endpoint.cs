using Feature.Form.Submission.Comments.Specifications;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Feature.Form.Submission.Comments.Create;

public class Endpoint(
    IReadRepository<FormSubmission> formSubmissionRepository,
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
        var submissionExists = await formSubmissionRepository.AnyAsync(
            new GetSubmissionForNgoSpecification(req.ElectionRoundId, req.NgoId, req.SubmissionId), ct);

        if (!submissionExists)
        {
            return TypedResults.NotFound();
        }

        var comment = FormSubmissionCommentAggregate.Create(
            req.ElectionRoundId,
            req.SubmissionId,
            req.QuestionId,
            req.Text);

        await repository.AddAsync(comment, ct);

        return TypedResults.Ok(FormSubmissionCommentModel.FromEntity(comment));
    }
}
