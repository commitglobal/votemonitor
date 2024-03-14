namespace Vote.Monitor.Api.Feature.Emergencies.Attachments.Delete;

public class Endpoint(IRepository<EmergencyAttachmentAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/emergencies/{emergencyId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("attachments"));
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
