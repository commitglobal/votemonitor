namespace Feature.Form.Submissions.ListMy;

public class Endpoint(IReadRepository<FormSubmission> repository) : Endpoint<Request, Ok<Response>>
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
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetFormSubmissionForObserverSpecification(req.ElectionRoundId, req.ObserverId, req.PollingStationIds);
        var submissions = await repository.ListAsync(specification, ct);

        return TypedResults.Ok(new Response
        {
            Submissions = submissions
        });
    }
}
