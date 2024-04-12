namespace Vote.Monitor.Api.Feature.Emergencies.Delete;

public class Endpoint(IRepository<EmergencyAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/emergencies/{id}");
        DontAutoTag();
        Options(x => x.WithTags("emergencies"));
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
