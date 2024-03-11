namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.Delete;

public class Endpoint(IRepository<PollingStationAttachmentAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("attachments"));
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
