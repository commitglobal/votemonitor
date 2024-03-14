namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.Create;

public class Endpoint(IRepository<PollingStationAttachmentAggregate> repository, ITimeProvider timeProvider) :
        Endpoint<Request, Ok<AttachmentModel>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/attachments");
        DontAutoTag();
        Options(x => x.WithTags("attachments"));
        AllowFileUploads();
    }

    public override async Task<Ok<AttachmentModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
