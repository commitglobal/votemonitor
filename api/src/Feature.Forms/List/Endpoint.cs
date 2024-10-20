using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Forms.List;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<FormSlimModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/forms");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<PagedResponse<FormSlimModel>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }
        
        var sql = """
                  SELECT COUNT(*) COUNT
                  FROM "Forms" F
                           INNER JOIN "MonitoringNgos" MN ON MN."Id" = F."MonitoringNgoId"
                  WHERE (
                      @searchText IS NULL
                          OR @searchText = ''
                          OR F."Code" ILIKE @searchText
                          OR F."Name" ->> F."DefaultLanguage" ILIKE @searchText
                          OR F."Description" ->> F."DefaultLanguage" ILIKE @searchText
                      )
                    AND (@type IS NULL OR F."FormType" = @type)
                    AND (@status IS NULL OR F."Status" = @status)
                    AND F."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId;
                  
                  SELECT F."Id",
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
                         F."LastModifiedBy"
                  FROM (SELECT F."Id",
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
                               COALESCE(F."LastModifiedOn", F."CreatedOn")                as "LastModifiedOn",
                               COALESCE(UPDATER."FirstName" || ' ' || UPDATER."LastName",
                                        CREATOR."FirstName" || ' ' || CREATOR."LastName") AS "LastModifiedBy"
                        FROM "Forms" F
                                 INNER JOIN "MonitoringNgos" MN ON MN."Id" = F."MonitoringNgoId"
                                 INNER JOIN "AspNetUsers" CREATOR ON F."CreatedBy" = CREATOR."Id"
                                 LEFT JOIN "AspNetUsers" UPDATER ON F."LastModifiedBy" = UPDATER."Id"
                        Where F."ElectionRoundId" = @electionRoundId
                          AND MN."NgoId" = @ngoId) F
                  WHERE (
                      @searchText IS NULL
                          OR @searchText = ''
                          OR F."Code" ILIKE @searchText
                          OR F."Name" ->> F."DefaultLanguage" ILIKE @searchText
                          OR F."Description" ->> F."DefaultLanguage" ILIKE @searchText
                      )
                    AND (@type IS NULL OR F."FormType" = @type)
                    AND (@status IS NULL OR F."Status" = @status)
                  
                  ORDER BY CASE
                               WHEN @sortExpression = 'Code ASC' THEN "Code"
                               END ASC,
                           CASE
                               WHEN @sortExpression = 'Code DESC' THEN "Code"
                               END DESC,
                           CASE
                               WHEN @sortExpression = 'LastModifiedOn ASC' THEN "LastModifiedOn"
                               END ASC,
                           CASE
                               WHEN @sortExpression = 'LastModifiedOn DESC' THEN "LastModifiedOn"
                               END DESC,
                           CASE
                               WHEN @sortExpression = 'FormType ASC' THEN "FormType"
                               END ASC,
                           CASE
                               WHEN @sortExpression = 'FormType DESC' THEN "FormType"
                               END DESC,
                           CASE
                               WHEN @sortExpression = 'Status ASC' THEN "Status"
                               END ASC,
                           CASE
                               WHEN @sortExpression = 'Status DESC' THEN "Status"
                               END DESC
                  OFFSET @offset ROWS 
                  FETCH NEXT @pageSize ROWS ONLY;
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            status = req.FormStatusFilter?.ToString(),
            type = req.TypeFilter?.ToString(),
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        int totalRowCount = 0;
        List<FormSlimModel> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<FormSlimModel>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<FormSlimModel>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return "LastModifiedOn ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";
        
        if (string.Equals(sortColumnName, nameof(FormSlimModel.Code), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSlimModel.Code)} {sortOrder}";
        }
        
        if (string.Equals(sortColumnName, nameof(FormSlimModel.LastModifiedOn),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSlimModel.LastModifiedOn)} {sortOrder}";
        }
        
        if (string.Equals(sortColumnName, nameof(FormSlimModel.FormType), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSlimModel.FormType)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSlimModel.Status), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSlimModel.Status)} {sortOrder}";
        }

        return "LastModifiedOn ASC";
    }
}