using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Feature.Notifications.RecipientsV2;
using Feature.Notifications.Send;
using Feature.Notifications.Specifications;
using Job.Contracts;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Notifications.SendV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<NotificationAggregate> repository,
    IReadRepository<MonitoringObserver> monitoringObserverRepository,
    INpgsqlConnectionFactory dbConnectionFactory,
    IJobService jobService,
    IHtmlStringSanitizer htmlStringSanitizer) :
    Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/notifications:sendV2");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(x =>
        {
            x.Summary = "Sends a notification to the observers matching v2 filters";
            x.Description = "Takes the same recipient filters as notifications:listRecipientsV2.";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var parameters = new DynamicParameters();
        var sql = $"""
                   {RecipientsV2Query.With(req, parameters)}
                   SELECT DISTINCT O."Id" AS "MonitoringObserverId", NT."Token"::TEXT AS "Token"
                   FROM "Recipients" R
                       INNER JOIN "Observers" O ON O."Id" = R."MonitoringObserverId"
                       LEFT JOIN "NotificationTokens" NT ON NT."ObserverId" = O."ObserverId"
                   """;

        List<NotificationRecipient> recipients;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            recipients = (await dbConnection.QueryAsync<NotificationRecipient>(sql, parameters)).ToList();
        }

        var monitoringObserverIds = recipients.Select(x => x.MonitoringObserverId).Distinct().ToList();
        var pushNotificationTokens = recipients
            .Select(x => x.Token)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!)
            .Distinct()
            .ToList();

        var monitoringObservers = await monitoringObserverRepository.ListAsync(
            new GetMonitoringObserverSpecification(req.ElectionRoundId, req.NgoId, monitoringObserverIds), ct);

        var sanitizedMessage = htmlStringSanitizer.Sanitize(req.Body);

        var notification = NotificationAggregate.Create(req.ElectionRoundId,
            req.UserId,
            monitoringObservers,
            req.Title,
            sanitizedMessage);

        await repository.AddAsync(notification, ct);

        jobService.EnqueueSendNotifications(pushNotificationTokens, req.Title, sanitizedMessage);

        return TypedResults.Ok(new Response
        {
            Status = "Success"
        });
    }
}
