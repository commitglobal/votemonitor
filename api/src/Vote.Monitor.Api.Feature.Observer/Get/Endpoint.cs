﻿using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Vote.Monitor.Api.Feature.Observer.Get;

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
                  select
                         u."Id",
                         u."Email",
                         u."FirstName",
                         u."LastName",
                         u."PhoneNumber",
                         u."Status",
                         u."EmailConfirmed"                                      as "IsAccountVerified",
                         COALESCE((select jsonb_agg(jsonb_build_object('ElectionTitle', er."Title",
                                                                       'ElectionEnglishTitle', er."EnglishTitle",
                                                                       'ElectionDate', er."StartDate",
                                                                       'NgoName', n."Name"
                                                    ))
                                   FROM "MonitoringObservers" mo
                                            left join "ElectionRounds" er on er."Id" = mo."ElectionRoundId"
                                            left join "MonitoringNgos" mn on mo."MonitoringNgoId" = mn."Id"
                                            left join "Ngos" n on n."Id" = mn."NgoId"
                                   where mo."ObserverId" = o."Id"), '[]'::JSONB) AS "MonitoredElections"
                  from "Observers" o
                           inner join "AspNetUsers" u on u."Id" = o."ApplicationUserId"

                  where o."Id" = @observerId
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
