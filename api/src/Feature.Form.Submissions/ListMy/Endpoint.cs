namespace Feature.Form.Submissions.ListMy;

public class Endpoint(IAuthorizationService authorizationService, IReadRepository<FormSubmission> repository)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:my");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Summary(s =>
        {
            s.Summary = "Gets all form submissions by an observer";
            s.Description = "Allows filtering by polling station";
        });

        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification =
            new GetFormSubmissionForObserverSpecification(req.ElectionRoundId, req.ObserverId, req.PollingStationIds);
        var submissions = await repository.ListAsync(specification, ct);

        return TypedResults.Ok(new Response
        {
            Submissions = submissions
        });
    }
}