﻿using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.MonitoringObservers.Get;

public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, Results<Ok<MonitoringObserverModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/monitoring-observers/{id}");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("monitoring-observers"));
        Summary(s =>
        {
            s.Summary = "Gets monitoring observer details";
        });
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<MonitoringObserverModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = @"
            WITH
                MONITORINGOBSERVER AS (
                    SELECT
                        MO.""Id"",
                        U.""FirstName"",
                        U.""LastName"",
                        U.""PhoneNumber"",
                        U.""Email"",
                        MO.""Tags"",
                        MO.""Status""
                    FROM
                        ""MonitoringObservers"" MO
                        INNER JOIN ""MonitoringNgos"" MN ON MN.""Id"" = MO.""MonitoringNgoId""
                        INNER JOIN ""Observers"" O ON O.""Id"" = MO.""ObserverId""
                        INNER JOIN ""AspNetUsers"" U ON U.""Id"" = O.""ApplicationUserId""
                    WHERE
                        MO.""Id"" = @id
                        AND MO.""ElectionRoundId"" = @electionRoundId
                ),
                LATESTTIMESTAMPS AS (
                    SELECT
                        MAX(COALESCE(PSI.""LastModifiedOn"", PSI.""CreatedOn"")) AS ""LatestActivityAt""
                    FROM
                        ""PollingStationInformation"" PSI
                    WHERE
                        PSI.""ElectionRoundId"" = @electionRoundId
                        AND PSI.""MonitoringObserverId"" = (
                            SELECT
                                ""Id""
                            FROM
                                MONITORINGOBSERVER
                        )
                    UNION ALL
                    SELECT
                        MAX(COALESCE(N.""LastModifiedOn"", N.""CreatedOn"")) AS ""LatestActivityAt""
                    FROM
                        ""Notes"" N
                    WHERE
                        N.""ElectionRoundId"" = @electionRoundId
                        AND N.""MonitoringObserverId"" = (
                            SELECT
                                ""Id""
                            FROM
                                MONITORINGOBSERVER
                        )
                    UNION ALL
                    SELECT
                        MAX(COALESCE(A.""LastModifiedOn"", A.""CreatedOn"")) AS ""LatestActivityAt""
                    FROM
                        ""Attachments"" A
                    WHERE
                        A.""ElectionRoundId"" = @electionRoundId
                        AND A.""MonitoringObserverId"" = (
                            SELECT
                                ""Id""
                            FROM
                                MONITORINGOBSERVER
                        )
                    UNION ALL
                    SELECT
                        MAX(COALESCE(QR.""LastModifiedOn"", QR.""CreatedOn"")) AS ""LatestActivityAt""
                    FROM
                        ""QuickReports"" QR
                    WHERE
                        QR.""ElectionRoundId"" = @electionRoundId
                        AND QR.""MonitoringObserverId"" = (
                            SELECT
                                ""Id""
                            FROM
                                MONITORINGOBSERVER
                        )
                )
            SELECT
                MO.""Id"",
                MO.""FirstName"",
                MO.""LastName"",
                MO.""PhoneNumber"",
                MO.""Email"",
                MO.""Tags"",
                MO.""Status"",
                MAX(LT.""LatestActivityAt"") AS ""LatestActivityAt""
            FROM
                MONITORINGOBSERVER MO,
                LATESTTIMESTAMPS LT
            GROUP BY
                MO.""Id"",
                MO.""FirstName"",
                MO.""LastName"",
                MO.""PhoneNumber"",
                MO.""Email"",
                MO.""Tags"",
                MO.""Status"";";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            id = req.Id
        };

        MonitoringObserverModel monitoringObserver = null;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            monitoringObserver = await dbConnection.QuerySingleOrDefaultAsync<MonitoringObserverModel>(sql, queryArgs);
        }

        if (monitoringObserver is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(monitoringObserver);
    }
}
