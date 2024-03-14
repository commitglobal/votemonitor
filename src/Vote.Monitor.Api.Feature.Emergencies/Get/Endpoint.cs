namespace Vote.Monitor.Api.Feature.Emergencies.Get;

public class Endpoint(IReadRepository<EmergencyAggregate> repository) : Endpoint<Request, Results<Ok<EmergencyModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/emergencies/{id}");
        DontAutoTag();
        Options(x => x.WithTags("emergencies"));
    }

    public override async Task<Results<Ok<EmergencyModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
