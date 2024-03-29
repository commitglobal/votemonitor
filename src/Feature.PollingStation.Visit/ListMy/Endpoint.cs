namespace Feature.PollingStation.Visit.ListMy;

public class Endpoint(IReadRepository<NgoAggregate> repository) : Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-station-visits:my");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-visit", "mobile"));
        Summary(s => {
            s.Summary = "Lists observer visited polling stations ";
            s.Description = "Polling station visits are based on polling station information / form answers";
        });
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
