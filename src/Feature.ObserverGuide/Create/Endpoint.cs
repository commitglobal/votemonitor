namespace Feature.ObserverGuide.Create;

public class Endpoint(IRepository<ObserverGuideAggregate> repository) :
        Endpoint<Request, Ok<ObserverGuideModel>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/observer-guide");
        DontAutoTag();
        Options(x => x.WithTags("observer-guide"));
    }

    public override async Task<Ok<ObserverGuideModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
