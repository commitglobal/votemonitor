
namespace Vote.Monitor.Api.Feature.Notifications.ListSent;

public class Endpoint(IRepository<NotificationAggregate> repository, ITimeProvider timeProvider) :
        Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/notifications:listSent");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
        AllowFileUploads();
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
