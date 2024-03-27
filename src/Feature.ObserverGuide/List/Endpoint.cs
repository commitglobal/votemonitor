namespace Feature.ObserverGuide.List;

public class Endpoint(IReadRepository<ObserverGuideAggregate> repository) : Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/observer-guide");
        DontAutoTag();
        Options(x => x.WithTags("observer-guide"));
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
