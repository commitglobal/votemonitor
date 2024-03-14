namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.Get;

public class Endpoint(IReadRepository<PollingStationAttachmentAggregate> repository) : Endpoint<Request, Results<Ok<AttachmentModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("attachments"));
    }

    public override async Task<Results<Ok<AttachmentModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
