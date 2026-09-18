using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Feature.Forms.Models;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Forms.Get;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, Results<Ok<FormFullModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/forms/{id}");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<FormFullModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  WITH
                      "MonitoringNgoData" AS (
                          SELECT
                              MN."ElectionRoundId",
                              MN."Id" AS "MonitoringNgoId",
                              EXISTS (
                                  SELECT 1
                                  FROM "CoalitionMemberships" CM
                                           JOIN "Coalitions" C ON CM."CoalitionId" = C."Id"
                                  WHERE CM."MonitoringNgoId" = MN."Id"
                                    AND CM."ElectionRoundId" = MN."ElectionRoundId"
                                    AND C."LeaderId" = MN."Id"
                              ) AS "IsCoalitionLeader",
                              (
                                  SELECT COUNT(1)
                                  FROM "CoalitionMemberships" CM
                                  WHERE CM."MonitoringNgoId" = MN."Id"
                                    AND CM."ElectionRoundId" = MN."ElectionRoundId"
                              ) > 0 AS "IsInACoalition"
                          FROM "MonitoringNgos" MN
                          WHERE MN."ElectionRoundId" = @electionRoundId
                            AND MN."NgoId" = @ngoId
                          LIMIT 1
                      )
                  SELECT
                      F."Id",
                      F."Code",
                      F."Name",
                      F."Description",
                      F."DefaultLanguage",
                      F."Languages",
                      F."Status",
                      F."FormType",
                      F."NumberOfQuestions",
                      F."LanguagesTranslationStatus",
                      F."Icon",
                      F."DisplayOrder",
                      F."Questions",
                      F."LastModifiedOn",
                      F."LastModifiedBy",
                      F."IsFormOwner"
                  FROM (
                           SELECT
                               F."Id",
                               F."Code",
                               F."Name",
                               F."Description",
                               F."DefaultLanguage",
                               F."Languages",
                               F."Status",
                               F."FormType",
                               F."NumberOfQuestions",
                               F."LanguagesTranslationStatus",
                               F."Icon",
                               F."DisplayOrder",
                               F."Questions",
                               COALESCE(F."LastModifiedOn", F."CreatedOn") AS "LastModifiedOn",
                               COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "LastModifiedBy",
                               EXISTS (
                                   SELECT 1
                                   FROM "MonitoringNgoData"
                                   WHERE "MonitoringNgoId" = F."MonitoringNgoId"
                               ) AS "IsFormOwner"
                           FROM "CoalitionFormAccess" CFA
                                    INNER JOIN "Coalitions" C ON CFA."CoalitionId" = C."Id"
                                    INNER JOIN "Forms" F ON CFA."FormId" = F."Id"
                                    INNER JOIN "AspNetUsers" CREATOR ON F."CreatedBy" = CREATOR."Id"
                                    LEFT JOIN "AspNetUsers" UPDATER ON F."LastModifiedBy" = UPDATER."Id"
                           WHERE CFA."MonitoringNgoId" = (SELECT "MonitoringNgoId" FROM "MonitoringNgoData")
                             AND C."ElectionRoundId" = @electionRoundId
                             AND F."Id" = @id
                             AND (
                               (SELECT "IsInACoalition" FROM "MonitoringNgoData")
                                   OR (SELECT "IsCoalitionLeader" FROM "MonitoringNgoData")
                               )

                           UNION

                           SELECT
                               F."Id",
                               F."Code",
                               F."Name",
                               F."Description",
                               F."DefaultLanguage",
                               F."Languages",
                               F."Status",
                               F."FormType",
                               F."NumberOfQuestions",
                               F."LanguagesTranslationStatus",
                               F."Icon",
                               F."DisplayOrder",
                               F."Questions",
                               COALESCE(F."LastModifiedOn", F."CreatedOn") AS "LastModifiedOn",
                               COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "LastModifiedBy",
                               TRUE AS "IsFormOwner"
                           FROM "Forms" F
                                    INNER JOIN "AspNetUsers" CREATOR ON F."CreatedBy" = CREATOR."Id"
                                    LEFT JOIN "AspNetUsers" UPDATER ON F."LastModifiedBy" = UPDATER."Id"
                           WHERE F."ElectionRoundId" = @electionRoundId
                             AND F."MonitoringNgoId" = (SELECT "MonitoringNgoId" FROM "MonitoringNgoData")
                             AND F."Id" = @id

                           UNION

                           SELECT
                               PSIF."Id",
                               PSIF."Code",
                               PSIF."Name",
                               PSIF."Description",
                               PSIF."DefaultLanguage",
                               PSIF."Languages",
                               PSIF."Status",
                               PSIF."FormType",
                               PSIF."NumberOfQuestions",
                               PSIF."LanguagesTranslationStatus",
                               NULL AS "Icon",
                               0 AS "DisplayOrder",
                               PSIF."Questions",
                               COALESCE(PSIF."LastModifiedOn", PSIF."CreatedOn") AS "LastModifiedOn",
                               COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "LastModifiedBy",
                               FALSE AS "IsFormOwner"
                           FROM "PollingStationInformationForms" PSIF
                                    INNER JOIN "AspNetUsers" CREATOR ON PSIF."CreatedBy" = CREATOR."Id"
                                    LEFT JOIN "AspNetUsers" UPDATER ON PSIF."LastModifiedBy" = UPDATER."Id"
                           WHERE PSIF."ElectionRoundId" = @electionRoundId
                             AND PSIF."Id" = @id
                       ) F
                  LIMIT 1;
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            id = req.Id
        };

        FormFullModel? form;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            form = await dbConnection.QuerySingleOrDefaultAsync<FormFullModel>(sql, queryArgs);
        }

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(form);
    }
}
