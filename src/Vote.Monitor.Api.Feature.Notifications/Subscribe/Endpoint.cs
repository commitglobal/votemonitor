using Vote.Monitor.Api.Feature.Notifications.Specifications;
using Vote.Monitor.Domain.Entities.NotificationTokenAggregate;

namespace Vote.Monitor.Api.Feature.Notifications.Subscribe;

public class Endpoint(IRepository<NotificationToken> repository) : Endpoint<Request, NoContent>
{
    public override void Configure()
    {
        Post("/api/notifications:subscribe");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
    }

    public override async Task<NoContent> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetNotificationTokenForObserverSpecification(req.ObserverId);
        var token = await repository.FirstOrDefaultAsync(specification, ct);

        if (token == null)
        {
            var newToken = NotificationToken.Create(req.ObserverId, req.Token);
            await repository.AddAsync(newToken, ct);

            return TypedResults.NoContent();
        }

        token.Update(req.Token);
        await repository.UpdateAsync(token, ct);

        return TypedResults.NoContent();
    }
}
