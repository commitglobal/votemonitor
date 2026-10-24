using Authorization.Policies;
using Dapper;
using Feature.Notifications.Specifications;
using Job.Contracts;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Notifications.Send;

public class Endpoint(
    IRepository<NotificationAggregate> repository,
    IReadRepository<MonitoringObserver> monitoringObserverRepository,
    INpgsqlConnectionFactory dbConnectionFactory,
    IJobService jobService,
    IHtmlStringSanitizer htmlStringSanitizer) :
    Endpoint<Request, Results<Ok<Response>, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/notifications:send");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, ProblemHttpResult>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql =
            """
            SELECT *
            FROM "GetNotificationRecipientTokens"(@electionRoundId, @ngoId, @searchText, @formType, @level1, @level2,
                                                  @level3, @level4, @level5, @pollingStationNumber,
                                                  @hasFlaggedAnswers, @submissionsFollowUpStatus, @tagsFilter,
                                                  @monitoringObserverStatus, @formId, @hasNotes, @hasAttachments,
                                                  @questionsAnswered, @fromDate, @toDate, @hasQuickReports,
                                                  @quickReportFollowUpStatus, @quickReportIncidentCategory)
            WHERE CARDINALITY(@monitoringObserverIds) = 0 OR "MonitoringObserverId" = ANY(@monitoringObserverIds)
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            monitoringObserverIds = req.MonitoringObserverIds ?? [],
            formType = req.FormTypeFilter?.ToString(),
            level1 = req.Level1Filter,
            level2 = req.Level2Filter,
            level3 = req.Level3Filter,
            level4 = req.Level4Filter,
            level5 = req.Level5Filter,
            pollingStationNumber = req.PollingStationNumberFilter,
            hasFlaggedAnswers = req.HasFlaggedAnswers,
            submissionsFollowUpStatus = req.SubmissionsFollowUpStatus?.ToString(),
            tagsFilter = req.TagsFilter ?? [],
            monitoringObserverStatus = req.MonitoringObserverStatus?.ToString(),
            formId = req.FormId,
            hasNotes = req.HasNotes,
            hasAttachments = req.HasAttachments,
            questionsAnswered = req.QuestionsAnswered?.ToString(),
            fromDate = req.FromDateFilter?.ToString("O"),
            toDate = req.ToDateFilter?.ToString("O"),

            hasQuickReports = req.HasQuickReports,
            quickReportFollowUpStatus = req.QuickReportFollowUpStatus?.ToString(),
            quickReportIncidentCategory = req.QuickReportIncidentCategory?.ToString()
        };

        IEnumerable<NotificationRecipient> result = [];
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            result = await dbConnection.QueryAsync<NotificationRecipient>(sql, queryArgs);
        }

        var recipients = result.ToList();

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
