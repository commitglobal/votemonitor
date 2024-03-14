
using Vote.Monitor.Core.Services.PushNotification.Contracts;

namespace Vote.Monitor.Api.Feature.Notifications.Send;

public class Endpoint(IRepository<NotificationAggregate> repository, IPushNotificationService notificationService, ITimeProvider timeProvider) :
        Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/notifications:send");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
        AllowFileUploads();
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
