namespace Vote.Monitor.Api.Feature.Emergencies.Attachments.List;

public class Endpoint(IReadRepository<EmergencyAttachmentAggregate> repository) : Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/emergencies/{emergencyId}/attachments");
        DontAutoTag();
        Options(x => x.WithTags("emergencies-attachments"));
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
