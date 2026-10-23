using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Observers.Get;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<ObserverModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/observers/{id}");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<ObserverModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = """
                  select *
                  from "GetPlatformObserverDetails"(@observerId)
                  """;

        var queryArgs = new { observerId = req.Id };


        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        var observer = await dbConnection.QueryFirstOrDefaultAsync<ObserverModel>(sql, queryArgs);
        if (observer is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(observer);
    }
}
