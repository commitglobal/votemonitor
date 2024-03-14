namespace Vote.Monitor.Api.Feature.Emergencies.Attachments.Create;

public class Endpoint(IRepository<EmergencyAttachmentAggregate> repository, ITimeProvider timeProvider) :
        Endpoint<Request, Ok<AttachmentModel>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/emergencies/{emergencyId}/attachments");
        DontAutoTag();
        Options(x => x.WithTags("emergencies-attachments"));
        AllowFileUploads();
    }

    public override async Task<Ok<AttachmentModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
