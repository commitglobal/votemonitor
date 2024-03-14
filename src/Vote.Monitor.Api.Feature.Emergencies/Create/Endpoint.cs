namespace Vote.Monitor.Api.Feature.Emergencies.Create;

public class Endpoint(IRepository<EmergencyAggregate> repository, ITimeProvider timeProvider) :
        Endpoint<Request, Ok<EmergencyModel>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/emergencies");
        DontAutoTag();
        Options(x => x.WithTags("emergencies"));
        AllowFileUploads();
    }

    public override async Task<Ok<EmergencyModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
