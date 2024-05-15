using Feature.Notifications.Specifications;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.NotificationTokenAggregate;
using Vote.Monitor.Module.Notifications.Contracts;

namespace Feature.Notifications.Send;

public class Endpoint(IRepository<NotificationAggregate> repository,
    IRepository<NotificationToken> tokenRepository,
    IReadRepository<MonitoringObserver> monitoringObserverRepository,
    IPushNotificationService notificationService) :
        Endpoint<Request, Results<Ok<Response>, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/notifications:send");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
    }

    public override async Task<Results<Ok<Response>, ProblemHttpResult>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var monitoringObserverIds = req.MonitoringObserverIds.Distinct().ToList();
        var monitoringObservers = await monitoringObserverRepository.ListAsync(new GetMonitoringObserverSpecification(req.ElectionRoundId, req.NgoId, monitoringObserverIds), ct);

        var observerIds = monitoringObservers.Select(x => x.ObserverId).ToList();

        var tokens = await tokenRepository.ListAsync(new GetNotificationTokenForObserversSpecification(observerIds), ct);

        var result = await notificationService.SendNotificationAsync(tokens.Select(x => x.Token).ToList(), req.Title, req.Body, ct);

        var notification = NotificationAggregate.Create(req.ElectionRoundId,
            req.UserId,
            monitoringObservers,
            req.Title,
            req.Body);

        await repository.AddAsync(notification, ct);

        if (result is SendNotificationResult.Ok success)
        {
            return TypedResults.Ok(new Response
            {
                Status = "Success",
                FailedCount = success.FailedCount,
                SuccessCount = success.SuccessCount
            });
        }

        return TypedResults.Problem("Error when sending notifications contact PlatformAdmin!");
    }
}
