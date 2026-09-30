using Authorization.Policies;
using Dapper;
using Feature.PollingStation.Information.PsiData;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.PollingStation.Information.ListAll;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, PagedResponse<PsiDataRowModel>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/information:all");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Lists all polling station information submitted in an election round (platform admins)";
        });
    }

    public override async Task<PagedResponse<PsiDataRowModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = $"""
                   SELECT COUNT(*)
                   {PsiDataQuery.FromWhere};

                   {PsiDataQuery.SelectColumns}
                   {PsiDataQuery.FromWhere}
                   {PsiDataQuery.OrderBy}
                   OFFSET @offset ROWS
                   FETCH NEXT @pageSize ROWS ONLY;
                   """;

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        using var multi = await dbConnection.QueryMultipleAsync(sql, PsiDataQuery.BuildArgs(req, paged: true));

        var totalCount = multi.Read<int>().Single();
        var items = multi.Read<PsiDataRowModel>().ToList();

        return new PagedResponse<PsiDataRowModel>(items, totalCount, req.PageNumber, req.PageSize);
    }
}
