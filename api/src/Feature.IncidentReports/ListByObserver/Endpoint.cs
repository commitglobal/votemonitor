using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.IncidentReports.ListByObserver;

public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<ObserverIncidentReportsOverview>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/incident-reports:byObserver");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports"));
        Policies(PolicyNames.NgoAdminsOnly);

        Summary(x => { x.Summary = "Incident reports aggregated by observer"; });
    }

    public override async Task<Results<Ok<PagedResponse<ObserverIncidentReportsOverview>>, NotFound>> ExecuteAsync(
        Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT COUNT(*) count
                  FROM
                      "MonitoringObservers" MO
                      INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                      INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                      INNER JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                  WHERE
                      MN."ElectionRoundId" = @electionRoundId
                      AND MN."NgoId" = @ngoId
                      AND (@searchText IS NULL OR @searchText = '' OR u."FirstName" ILIKE @searchText OR u."LastName" ILIKE @searchText OR u."Email" ILIKE @searchText OR u."PhoneNumber" ILIKE @searchText)
                      AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR  mo."Tags" && @tagsFilter);

                  SELECT "MonitoringObserverId",
                         "ObserverName",
                         "PhoneNumber",
                         "Email",
                         "Tags",
                         "NumberOfFlaggedAnswers",
                         "NumberOfIncidentsSubmitted",
                         "FollowUpStatus"
                  FROM (SELECT MO."Id" AS "MonitoringObserverId",
                               U."FirstName" || ' ' || U."LastName" AS "ObserverName",
                               U."PhoneNumber",
                               U."Email",
                               MO."Tags",
                               COALESCE(
                                       (SELECT SUM("NumberOfFlaggedAnswers")
                                        FROM "IncidentReports" IR
                                        WHERE IR."MonitoringObserverId" = MO."Id"),
                                       0
                               ) AS "NumberOfFlaggedAnswers",
                               (SELECT COUNT(1)
                                FROM "IncidentReports" IR
                                WHERE IR."MonitoringObserverId" = MO."Id" AND IR."ElectionRoundId" = @electionRoundId) AS "NumberOfIncidentsSubmitted",
                               (
                                   CASE
                                       WHEN EXISTS (SELECT 1
                                                    FROM "IncidentReports" IR
                                                    WHERE IR."FollowUpStatus" = 'NeedsFollowUp'
                                                      AND IR."MonitoringObserverId" = MO."Id"
                                                      AND IR."ElectionRoundId" = @electionRoundId)
                                           THEN 'NeedsFollowUp'
                                       ELSE NULL
                                       END
                                   )                                           AS "FollowUpStatus"
                        FROM "MonitoringObservers" MO
                                 INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                                 INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                                 INNER JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                        WHERE MN."ElectionRoundId" = @electionRoundId
                          AND MN."NgoId" = @ngoId
                          AND (@searchText IS NULL OR @searchText = '' OR u."FirstName" ILIKE @searchText OR
                               u."LastName" ILIKE @searchText OR u."Email" ILIKE @searchText OR u."PhoneNumber" ILIKE @searchText)
                          AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR mo."Tags" && @tagsFilter)) T
                  WHERE (@needsFollowUp IS NULL OR T."FollowUpStatus" = 'NeedsFollowUp')
                  ORDER BY
                      CASE WHEN @sortExpression = 'ObserverName ASC' THEN "ObserverName" END ASC,
                      CASE WHEN @sortExpression = 'ObserverName DESC' THEN "ObserverName" END DESC,
                      CASE WHEN @sortExpression = 'PhoneNumber ASC' THEN "PhoneNumber" END ASC,
                      CASE WHEN @sortExpression = 'PhoneNumber DESC' THEN "PhoneNumber" END DESC,
                      CASE WHEN @sortExpression = 'Email ASC' THEN "Email" END ASC,
                      CASE WHEN @sortExpression = 'Email DESC' THEN "Email" END DESC,
                      CASE WHEN @sortExpression = 'Tags ASC' THEN "Tags" END ASC,
                      CASE WHEN @sortExpression = 'Tags DESC' THEN "Tags" END DESC,
                      CASE WHEN @sortExpression = 'NumberOfFlaggedAnswers ASC' THEN "NumberOfFlaggedAnswers" END ASC,
                      CASE WHEN @sortExpression = 'NumberOfFlaggedAnswers DESC' THEN "NumberOfFlaggedAnswers" END DESC,
                      
                      CASE WHEN @sortExpression = 'NumberOfIncidentsSubmitted ASC' THEN "NumberOfIncidentsSubmitted" END ASC,
                      CASE WHEN @sortExpression = 'NumberOfIncidentsSubmitted DESC' THEN "NumberOfIncidentsSubmitted" END DESC
                  
                  OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            tagsFilter = req.TagsFilter ?? [],
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting),
            needsFollowUp = req.FollowUpStatus?.ToString(),
        };

        int totalRowCount = 0;
        List<ObserverIncidentReportsOverview> entries = [];

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<ObserverIncidentReportsOverview>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<ObserverIncidentReportsOverview>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(ObserverIncidentReportsOverview.ObserverName)} ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(ObserverIncidentReportsOverview.ObserverName),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverIncidentReportsOverview.ObserverName)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverIncidentReportsOverview.Email),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverIncidentReportsOverview.Email)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverIncidentReportsOverview.PhoneNumber),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverIncidentReportsOverview.PhoneNumber)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverIncidentReportsOverview.Tags),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverIncidentReportsOverview.Tags)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverIncidentReportsOverview.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverIncidentReportsOverview.NumberOfFlaggedAnswers)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverIncidentReportsOverview.NumberOfIncidentsSubmitted),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverIncidentReportsOverview.NumberOfIncidentsSubmitted)} {sortOrder}";
        }
        
        return $"{nameof(ObserverIncidentReportsOverview.ObserverName)} ASC";
    }
}