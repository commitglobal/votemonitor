using Authorization.Policies;
using Dapper;
using Feature.Notifications.Specifications;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Module.Notifications.Contracts;

namespace Feature.Notifications.Send;

public class Endpoint(IRepository<NotificationAggregate> repository,
    IReadRepository<MonitoringObserver> monitoringObserverRepository,
    INpgsqlConnectionFactory dbConnectionFactory,
    IPushNotificationService notificationService) :
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
        var sql = @"
            SELECT
                MO.""Id"",
                NT.""Token""
            FROM
                ""MonitoringObservers"" MO
                INNER JOIN ""MonitoringNgos"" MN ON MN.""Id"" = MO.""MonitoringNgoId""
                INNER JOIN ""Observers"" O ON O.""Id"" = MO.""ObserverId""
                INNER JOIN ""AspNetUsers"" U ON U.""Id"" = O.""ApplicationUserId""
                LEFT JOIN ""NotificationTokens"" NT ON NT.""ObserverId"" = MO.""ObserverId""
            WHERE
                MN.""ElectionRoundId"" = @electionRoundId
                AND MN.""NgoId"" = @ngoId
                AND (@searchText IS NULL OR @searchText = '' OR (U.""FirstName"" || ' ' || U.""LastName"") ILIKE @searchText OR u.""Email"" ILIKE @searchText OR u.""PhoneNumber"" ILIKE @searchText)
                AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR  mo.""Tags"" @> @tagsFilter)
                AND (@status IS NULL OR  mo.""Status"" = @status)
                AND (@level1 IS NULL OR EXISTS (
                    SELECT
                        1
                    FROM
                        (
                            SELECT
                                PSI.""PollingStationId"" ""PollingStationId""
                            FROM
                                ""PollingStationInformation"" PSI
                                INNER JOIN ""PollingStations"" PS ON PS.""Id"" = PSI.""PollingStationId""
                            WHERE
                                PSI.""MonitoringObserverId"" = MO.""Id""
                                AND PS.""ElectionRoundId"" = @electionRoundId
                                AND PSI.""ElectionRoundId"" = @electionRoundId
                            UNION
                            SELECT
                                N.""PollingStationId"" ""PollingStationId""
                            FROM
                                ""Notes"" N
                                INNER JOIN ""PollingStations"" PS ON PS.""Id"" = N.""PollingStationId""
                            WHERE
                                N.""MonitoringObserverId"" = MO.""Id""
                                AND PS.""ElectionRoundId"" = @electionRoundId
                                AND N.""ElectionRoundId"" = @electionRoundId
                            UNION
                            SELECT
                                A.""PollingStationId"" ""PollingStationId""
                            FROM
                                ""Attachments"" A
                                INNER JOIN ""PollingStations"" PS ON PS.""Id"" = A.""PollingStationId""
                            WHERE
                                A.""MonitoringObserverId"" = MO.""Id""
                                AND PS.""ElectionRoundId"" = @electionRoundId
                                AND A.""ElectionRoundId"" = @electionRoundId
                            UNION
                            SELECT
                                QR.""PollingStationId"" ""PollingStationId""
                            FROM
                                ""QuickReports"" QR
                                INNER JOIN ""PollingStations"" PS ON PS.""Id"" = QR.""PollingStationId""
                            WHERE
                                QR.""PollingStationId"" IS NOT NULL
                                AND QR.""MonitoringObserverId"" = MO.""Id""
                                AND PS.""ElectionRoundId"" = @electionRoundId
                                AND QR.""ElectionRoundId"" = @electionRoundId
                            UNION
                            SELECT
                                FS.""PollingStationId"" ""PollingStationId""
                            FROM
                                ""FormSubmissions"" FS
                                INNER JOIN ""PollingStations"" PS ON PS.""Id"" = FS.""PollingStationId""
                            WHERE
                                FS.""MonitoringObserverId"" = MO.""Id""
                                AND PS.""ElectionRoundId"" = @electionRoundId
                                AND FS.""ElectionRoundId"" = @electionRoundId
                        ) psVisits
                        INNER JOIN ""PollingStations"" PS ON psVisits.""PollingStationId"" = PS.""Id""
                    WHERE
                        ""ElectionRoundId"" = @electionRoundId
                        AND (
                            @level1 IS NULL
                            OR PS.""Level1"" = @level1
                        )
                        AND (
                            @level2 IS NULL
                            OR PS.""Level2"" = @level2
                        )
                        AND (
                            @level3 IS NULL
                            OR PS.""Level3"" = @level3
                        )
                        AND (
                            @level4 IS NULL
                            OR PS.""Level3"" = @level4
                        )
                        AND (
                            @level5 IS NULL
                            OR PS.""Level3"" = @level5
                        )))";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            tagsFilter = req.TagsFilter ?? [],
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            status = req.StatusFilter?.ToString(),
            level1 = req.Level1Filter,
            level2 = req.Level2Filter,
            level3 = req.Level3Filter,
            level4 = req.Level4Filter,
            level5 = req.Level5Filter
        };

        var result = await dbConnectionFactory.GetOpenConnection().QueryAsync<NotificationRecipient>(sql, queryArgs);
        var recipients = result.ToList();

        var monitoringObserverIds = recipients.Select(x => x.Id).ToList();
        var pushNotificationTokens = recipients
            .Select(x => x.Token)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        var monitoringObservers = await monitoringObserverRepository.ListAsync(new GetMonitoringObserverSpecification(req.ElectionRoundId, req.NgoId, monitoringObserverIds), ct);

        var sendResultNotification = await notificationService.SendNotificationAsync(pushNotificationTokens, req.Title, req.Body, ct);

        var notification = NotificationAggregate.Create(req.ElectionRoundId,
            req.UserId,
            monitoringObservers,
            req.Title,
            req.Body);

        await repository.AddAsync(notification, ct);

        if (sendResultNotification is SendNotificationResult.Ok success)
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
