using System.Data;
using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Notifications.ListSent;

public class Endpoint(IDbConnection dbConnection) : Endpoint<Request, PagedResponse<NotificationModel>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/notifications:listSent");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<PagedResponse<NotificationModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = @"
        SELECT count(*) as count
        FROM
            ""Notifications"" N
            INNER JOIN ""NgoAdmins"" NA ON N.""SenderId"" = NA.""Id""
            INNER JOIN ""MonitoringNgos"" MN ON NA.""NgoId"" = MN.""NgoId""
        WHERE
            MN.""NgoId"" = @ngoId
            AND N.""ElectionRoundId"" = @electionRoundId;

        SELECT
            N.""Id"",
            N.""Title"",
            N.""Body"",
            N.""CreatedOn"" ""SentAt"",
            U.""FirstName"" || ' ' || U.""LastName"" ""Sender"",
            (SELECT COUNT(*)
                FROM
                    ""MonitoringObserverNotification"" MON
                WHERE
                    MON.""NotificationId"" = N.""Id""
            ) ""NumberOfTargetedObservers""
        FROM
            ""Notifications"" N
            INNER JOIN ""NgoAdmins"" NA ON N.""SenderId"" = NA.""Id""
            INNER JOIN ""MonitoringNgos"" MN ON NA.""NgoId"" = MN.""NgoId""
            INNER JOIN ""AspNetUsers"" U ON U.""Id"" = NA.""Id""
        WHERE
            MN.""NgoId"" = @ngoId
            AND N.""ElectionRoundId"" = @electionRoundId
        ORDER BY N.""CreatedOn"" DESC
        FETCH NEXT
            @pageSize ROWS ONLY;";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
        };

        var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);

        var totalRowCount = multi.Read<int>().Single();
        var entries = multi.Read<NotificationModel>().ToList();

        return new PagedResponse<NotificationModel>(entries, totalRowCount, req.PageNumber, req.PageSize);
    }
}
