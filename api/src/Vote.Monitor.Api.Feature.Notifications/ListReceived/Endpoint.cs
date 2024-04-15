namespace Vote.Monitor.Api.Feature.Notifications.ListReceived;

public class Endpoint(IRepository<NotificationAggregate> repository) :
        Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/notifications:listReceived");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
        AllowFileUploads();
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
