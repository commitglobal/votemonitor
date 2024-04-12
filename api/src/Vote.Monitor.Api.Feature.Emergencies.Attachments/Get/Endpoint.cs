namespace Vote.Monitor.Api.Feature.Emergencies.Attachments.Get;

public class Endpoint(IReadRepository<EmergencyAttachmentAggregate> repository) : Endpoint<Request, Results<Ok<AttachmentModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/emergencies/{emergencyId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("emergencies-attachments"));
    }

    public override async Task<Results<Ok<AttachmentModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
