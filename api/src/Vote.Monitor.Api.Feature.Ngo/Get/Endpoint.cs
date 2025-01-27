﻿using Authorization.Policies;
using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Vote.Monitor.Api.Feature.Ngo.Get;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, Results<Ok<NgoModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/ngos/{id}");
        DontAutoTag();
        Options(x => x.WithTags("ngos"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<NgoModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var sql = """
            SELECT
              N."Id",
              N."Name",
              N."Status",
              (SELECT COUNT(1) 
               FROM "NgoAdmins" NA 
               WHERE NA."NgoId" = N."Id") AS "NumberOfNgoAdmins",
              (SELECT COUNT(1) 
               FROM "MonitoringNgos" MN 
               WHERE MN."NgoId" = N."Id") AS "NumberOfElectionsMonitoring",
              (
                SELECT MAX(ER."StartDate") 
                FROM "MonitoringNgos" MN
                INNER JOIN "ElectionRounds" ER ON ER."Id" = MN."ElectionRoundId"
                WHERE MN."NgoId" = N."Id"
              ) AS "DateOfLastElection"
            FROM
              "Ngos" N
            WHERE
              N."Id" = @ngoId
            """;

        var queryArgs = new { ngoId = req.Id };


        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            var ngoModel = await dbConnection.QuerySingleOrDefaultAsync<NgoModel>(sql, queryArgs);
            if (ngoModel is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(ngoModel);
        }
    }
}
