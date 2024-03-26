namespace Feature.ObserverGuide.Get;

public class Endpoint(IReadRepository<ObserverGuideAggregate> repository) 
    : Endpoint<Request, Results<Ok<ObserverGuideModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/observer-guide/{id}");
        DontAutoTag();
        Options(x => x.WithTags("observer-guide"));
    }

    public override async Task<Results<Ok<ObserverGuideModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
