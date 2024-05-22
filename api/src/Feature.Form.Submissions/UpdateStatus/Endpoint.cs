using Authorization.Policies;

namespace Feature.Form.Submissions.UpdateStatus;

public class Endpoint(IRepository<FormSubmission> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/form-submissions/{id}:status");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Summary(s =>
        {
            s.Summary = "Updates follow up status for a submission";
        });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetFormSubmissionSpecification(req.ElectionRoundId, req.NgoId, req.Id);
        var formSubmission = await repository.FirstOrDefaultAsync(specification, ct);

        if (formSubmission is null)
        {
            return TypedResults.NotFound();
        }

        formSubmission.UpdateFollowUpStatus(req.FollowUpStatus);

        await repository.UpdateAsync(formSubmission, ct);
        return TypedResults.NoContent();
    }
}
