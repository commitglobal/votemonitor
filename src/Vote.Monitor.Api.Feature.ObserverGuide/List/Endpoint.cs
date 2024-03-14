namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.List;

public class Endpoint(IReadRepository<PollingStationAttachmentAggregate> repository) : Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/attachments");
        DontAutoTag();
        Options(x => x.WithTags("attachments"));
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
