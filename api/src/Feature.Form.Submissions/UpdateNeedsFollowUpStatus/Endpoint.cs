using Authorization.Policies;

namespace Feature.Form.Submissions.UpdateNeedsFollowUpStatus;

public class Endpoint(IRepository<FormSubmission> repository) : Endpoint<Request, Results<Ok<FormSubmissionModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/form-submissions/{id}:followUpStatus");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Summary(s =>
        {
            s.Summary = "Updates follow up status for a submission";
        });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<FormSubmissionModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetFormSubmissionSpecification(req.ElectionRoundId, req.NgoId, req.Id);
        var formSubmission = await repository.FirstOrDefaultAsync(specification, ct);

        if (formSubmission is null)
        {
            return TypedResults.NotFound();
        }

        formSubmission.UpdateNeedsFollowUpStatus(req.NeedsFollowUp);

        await repository.UpdateAsync(formSubmission, ct);
        return TypedResults.Ok(FormSubmissionModel.FromEntity(formSubmission));
    }
}
