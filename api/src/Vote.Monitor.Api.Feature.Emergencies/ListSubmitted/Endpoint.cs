namespace Vote.Monitor.Api.Feature.Emergencies.ListSubmitted;

public class Endpoint(IReadRepository<EmergencyAggregate> repository) : Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/emergencies:submitted");
        DontAutoTag();
        Options(x => x.WithTags("emergencies"));
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
