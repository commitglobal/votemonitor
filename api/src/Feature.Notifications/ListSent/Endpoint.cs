using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Notifications.ListSent;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, PagedResponse<NotificationModel>>
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
        var sql = """
                  SELECT
                      COUNT(DISTINCT N."Id") AS COUNT
                  FROM
                      "MonitoringNgos" MN
                      INNER JOIN "NgoAdmins" NA ON NA."NgoId" = MN."NgoId"
                      INNER JOIN "Notifications" N ON N."SenderId" = NA."Id"
                  WHERE
                      MN."NgoId" = @ngoId
                      AND N."ElectionRoundId" = @electionRoundId;
          
                  SELECT
                      N."Id",
                      N."Title",
                      N."Body",
                      N."CreatedOn" "SentAt",
                      U."FirstName" || ' ' || U."LastName" "Sender",
                      (SELECT COUNT(*)
                          FROM
                              "MonitoringObserverNotification" MON
                          WHERE
                              MON."NotificationId" = N."Id"
                      ) "NumberOfTargetedObservers"
                  FROM
                      "Notifications" N
                      INNER JOIN "NgoAdmins" NA ON N."SenderId" = NA."Id"
                      INNER JOIN "MonitoringNgos" MN ON NA."NgoId" = MN."NgoId"
                      INNER JOIN "AspNetUsers" U ON U."Id" = NA."Id"
                  WHERE
                      MN."NgoId" = @ngoId
                      AND N."ElectionRoundId" = @electionRoundId
                  GROUP BY
                      N."Id",
                      U."Id"
                  ORDER BY N."CreatedOn" DESC
                  FETCH NEXT
                      @pageSize ROWS ONLY;
                  """;
        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
        };

        int totalRowCount;
        List<NotificationModel> entries = [];
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);

            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<NotificationModel>().ToList();
        }
        return new PagedResponse<NotificationModel>(entries, totalRowCount, req.PageNumber, req.PageSize);
    }
}
