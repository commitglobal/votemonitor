namespace Feature.Form.Submissions.Get;

public class Endpoint(IAuthorizationService authorizationService, IReadRepository<FormSubmission> repository)
    : Endpoint<Request, Results<Ok<FormSubmissionModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Summary(s => { s.Summary = "Gets submission for a polling station"; });

        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<Results<Ok<FormSubmissionModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }
        
        var specification =
            new GetFormSubmissionSpecification(req.ElectionRoundId, req.PollingStationId, req.FormId, req.ObserverId);
        var formSubmission = await repository.FirstOrDefaultAsync(specification, ct);

        if (formSubmission is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(FormSubmissionModel.FromEntity(formSubmission));
    }
}