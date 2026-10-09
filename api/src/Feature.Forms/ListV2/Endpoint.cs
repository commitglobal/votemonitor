using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Feature.Forms.Models;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Forms.ListV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<List<FormSlimModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/forms:listAll");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Lists all forms for an election round";
            s.Description = "Gets all forms without pagination";
        });
    }

    public override async Task<Results<Ok<List<FormSlimModel>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
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
                              -- Check if MonitoringNgo is a coalition leader
                              EXISTS (
                                  SELECT
                                      1
                                  FROM
                                      "CoalitionMemberships" CM
                                          JOIN "Coalitions" C ON CM."CoalitionId" = C."Id"
                                  WHERE
                                      CM."MonitoringNgoId" = MN."Id"
                                    AND CM."ElectionRoundId" = MN."ElectionRoundId"
                                    AND C."LeaderId" = MN."Id"
                              ) AS "IsCoalitionLeader",
                              -- Check if MonitoringNgo is in a coalition
                              (
                                  SELECT
                                      COUNT(1)
                                  FROM
                                      "CoalitionMemberships" CM
                                  WHERE
                                      CM."MonitoringNgoId" = MN."Id"
                                    AND CM."ElectionRoundId" = MN."ElectionRoundId"
                              ) > 0 AS "IsInACoalition"
                          FROM
                              "MonitoringNgos" MN
                          WHERE
                              MN."ElectionRoundId" = @electionRoundId
                            AND MN."NgoId" = @ngoId
                          LIMIT
                              1
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
                      F."LastModifiedOn",
                      F."LastModifiedBy",
                      F."IsFormOwner",
                      CASE WHEN f."IsFormOwner" THEN COALESCE(
                              (SELECT JSONB_AGG(
                                              JSONB_BUILD_OBJECT(
                                                      'NgoId',
                                                      N."Id",
                                                      'Name',
                                                      N."Name"
                                              )
                                      )
                               FROM "CoalitionFormAccess" cfa
                                        inner join "Coalitions" c on c."Id" = cfa."CoalitionId"
                                        inner join "MonitoringNgos" mn on cfa."MonitoringNgoId" = mn."Id"
                                        inner join "Ngos" n on mn."NgoId" = n."Id"
                               WHERE c."ElectionRoundId" = @electionRoundId
                                 AND cfa."FormId" = F."Id"),
                              '[]'::JSONB) ELSE '[]'::jsonb END AS "FormAccess"
                  FROM
                      (
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
                              COALESCE(F."LastModifiedOn", F."CreatedOn") AS "LastModifiedOn",
                              COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "LastModifiedBy",
                              EXISTS (
                                  SELECT 1
                                  FROM "MonitoringNgoData"
                                  WHERE "MonitoringNgoId" = f."MonitoringNgoId"
                              ) AS "IsFormOwner"
                          FROM
                              "CoalitionFormAccess" CFA
                                  INNER JOIN "Coalitions" C ON CFA."CoalitionId" = C."Id"
                                  INNER JOIN "Forms" F ON CFA."FormId" = F."Id"
                                  LEFT JOIN "AspNetUsers" CREATOR ON F."CreatedBy" = CREATOR."Id"
                                  LEFT JOIN "AspNetUsers" UPDATER ON F."LastModifiedBy" = UPDATER."Id"
                          WHERE
                              CFA."MonitoringNgoId" = (
                                  SELECT
                                      "MonitoringNgoId"
                                  FROM
                                      "MonitoringNgoData"
                              )
                            AND C."ElectionRoundId" = @electionRoundId
                            AND (
                              (SELECT "IsInACoalition" FROM "MonitoringNgoData") 
                              OR (SELECT "IsCoalitionLeader" FROM "MonitoringNgoData")
                              )
                            AND (
                              @searchText IS NULL
                                  OR @searchText = ''
                                  OR F."Code" ILIKE @searchText
                                  OR F."Name" ->> F."DefaultLanguage" ILIKE @searchText
                                  OR F."Description" ->> F."DefaultLanguage" ILIKE @searchText
                                  OR F."Id"::TEXT ILIKE @searchText
                              )
                            AND (
                              @type IS NULL
                                  OR F."FormType" = @type
                              )
                            AND (
                              @status IS NULL
                                  OR F."Status" = @status
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
                              COALESCE(F."LastModifiedOn", F."CreatedOn") AS "LastModifiedOn",
                              COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "LastModifiedBy",
                              true as "IsFormOwner"
                          FROM
                              "Forms" F
                                  LEFT JOIN "AspNetUsers" CREATOR ON F."CreatedBy" = CREATOR."Id"
                                  LEFT JOIN "AspNetUsers" UPDATER ON F."LastModifiedBy" = UPDATER."Id"
                          WHERE
                              F."ElectionRoundId" = @electionRoundId
                            AND F."MonitoringNgoId" = (SELECT "MonitoringNgoId" FROM "MonitoringNgoData")
                            AND (
                              @searchText IS NULL
                                  OR @searchText = ''
                                  OR F."Code" ILIKE @searchText
                                  OR F."Name" ->> F."DefaultLanguage" ILIKE @searchText
                                  OR F."Description" ->> F."DefaultLanguage" ILIKE @searchText
                                  OR F."Id"::TEXT ILIKE @searchText
                              )
                            AND (@type IS NULL OR F."FormType" = @type)
                            AND (@status IS NULL OR F."Status" = @status)
                      ) F
                  WHERE
                      (
                          @searchText IS NULL
                              OR @searchText = ''
                              OR F."Code" ILIKE @searchText
                              OR F."Name" ->> F."DefaultLanguage" ILIKE @searchText
                              OR F."Description" ->> F."DefaultLanguage" ILIKE @searchText
                              OR F."Id"::TEXT ILIKE @searchText
                          )
                    AND (
                      @type IS NULL
                          OR F."FormType" = @type
                      )
                    AND (
                      @status IS NULL
                          OR F."Status" = @status
                      );
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            status = req.FormStatusFilter?.ToString(),
            type = req.TypeFilter?.ToString()
        };

        List<FormSlimModel> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            entries = (await dbConnection.QueryAsync<FormSlimModel>(sql, queryArgs)).ToList();
        }

        return TypedResults.Ok(entries);
    }
}
