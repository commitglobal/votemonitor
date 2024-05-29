namespace Feature.Feedback.List;

public class Endpoint(IReadRepository<FeedbackAggregate> repository) : Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/feedback/");
        DontAutoTag();
        Options(x => x.WithTags("feedback"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {

        var specification = new GetSubmittedFeedbackSpecification(req.ElectionRoundId);
        var feedback = await repository.ListAsync(specification, ct);

        return TypedResults.Ok(new Response
        {
            Feedback = feedback
        });
    }
}
