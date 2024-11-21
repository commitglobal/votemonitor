using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Form.Submissions.ListByObserver;

public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<ObserverSubmissionOverview>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:byObserver");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminsOnly);

        Summary(x => { x.Summary = "Form submissions aggregated by observer"; });
    }

    public override async Task<Results<Ok<PagedResponse<ObserverSubmissionOverview>>, NotFound>> ExecuteAsync(
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
                  SELECT
                      COUNT(*) count
                  FROM
                      "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) MO
                  WHERE
                    (@COALITIONMEMBERID IS NULL OR mo."NgoId" = @COALITIONMEMBERID)
                   AND   (
                          @searchText IS NULL
                              OR @searchText = ''
                              OR mo."MonitoringObserverId"::TEXT ILIKE @searchText
                              OR mo."DisplayName" ILIKE @searchText
                              OR mo."Email" ILIKE @searchText
                              OR mo."PhoneNumber" ILIKE @searchText
                          )
                    AND (
                      @tagsFilter IS NULL
                          OR cardinality(@tagsFilter) = 0
                          OR mo."Tags" && @tagsFilter
                      );
               
                  SELECT
                      "MonitoringObserverId",
                      "ObserverName",
                      "PhoneNumber",
                      "Email",
                      "Tags",
                      "NgoName",
                      "NumberOfFlaggedAnswers",
                      "NumberOfLocations",
                      "NumberOfFormsSubmitted",
                      "FollowUpStatus"
                  FROM
                      (
                          SELECT
                              MO."MonitoringObserverId" "MonitoringObserverId",
                              Mo."DisplayName" "ObserverName",
                              Mo."PhoneNumber",
                              Mo."Email",
                              MO."Tags",
                              MO."NgoName",
                              COALESCE(
                                      (
                                          SELECT
                                              SUM("NumberOfFlaggedAnswers")
                                          FROM
                                              "FormSubmissions" FS
                                                  inner join "GetAvailableForms"(@electionRoundId, @ngoId, @dataSource) f on f."FormId" = fs."FormId"
                                          WHERE
                                              FS."MonitoringObserverId" = MO."MonitoringObserverId"
                                      ),
                                      0
                              ) AS "NumberOfFlaggedAnswers",
                              (
                                  SELECT
                                      COUNT(*)
                                  FROM
                                      (
                                          SELECT
                                              PSI."PollingStationId"
                                          FROM
                                              "PollingStationInformation" PSI
                                          WHERE
                                              PSI."MonitoringObserverId" = MO."MonitoringObserverId"
                                            AND PSI."ElectionRoundId" = @electionRoundId
                                          UNION
                                          SELECT
                                              FS."PollingStationId"
                                          FROM
                                              "FormSubmissions" FS
                                                  inner join "GetAvailableForms"(@electionRoundId, @ngoId, @dataSource) f on f."FormId" = fs."FormId"
                                          WHERE
                                              FS."MonitoringObserverId" = MO."MonitoringObserverId"
                                            AND FS."ElectionRoundId" = @electionRoundId
                                      ) TMP
                              ) AS "NumberOfLocations",
                              (
                                  SELECT
                                      COUNT(*)
                                  FROM
                                      (
                                          SELECT
                                              PSI."Id"
                                          FROM
                                              "PollingStationInformation" PSI
                                          WHERE
                                              PSI."MonitoringObserverId" = MO."MonitoringObserverId"
                                            AND PSI."ElectionRoundId" = @electionRoundId
                                          UNION
                                          SELECT
                                              FS."Id"
                                          FROM
                                              "FormSubmissions" FS
                                                  inner join "GetAvailableForms"(@electionRoundId, @ngoId, @dataSource) f on f."FormId" = fs."FormId"
                                          WHERE
                                              FS."MonitoringObserverId" = MO."MonitoringObserverId"
                                            AND FS."ElectionRoundId" = @electionRoundId
                                      ) TMP
                              ) AS "NumberOfFormsSubmitted",
                              (
                                  CASE WHEN EXISTS (
                                      SELECT
                                          1
                                      FROM
                                          "FormSubmissions" FS
                                              inner join "GetAvailableForms"(@electionRoundId, @ngoId, @dataSource) f on f."FormId" = fs."FormId"
                                      WHERE
                                          FS."FollowUpStatus" = 'NeedsFollowUp'
                                        AND FS."MonitoringObserverId" = MO."MonitoringObserverId"
                                        AND FS."ElectionRoundId" = @electionRoundId
                                  ) THEN 'NeedsFollowUp' ELSE NULL END
                                  ) AS "FollowUpStatus"
                          FROM
                              "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) MO
                          WHERE
                            (@COALITIONMEMBERID IS NULL OR mo."NgoId" = @COALITIONMEMBERID)
                            AND (
                                  @searchText IS NULL
                                      OR @searchText = ''
                                      OR mo."MonitoringObserverId"::TEXT ILIKE @searchText
                                      OR mo."DisplayName" ILIKE @searchText
                                      OR mo."Email" ILIKE @searchText
                                      OR mo."PhoneNumber" ILIKE @searchText
                                  )
                            AND (
                              @tagsFilter IS NULL
                                  OR cardinality(@tagsFilter) = 0
                                  OR mo."Tags" && @tagsFilter
                              )
                      ) T
                  WHERE
                      (
                          @needsFollowUp IS NULL
                              OR T."FollowUpStatus" = 'NeedsFollowUp'
                          )
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
                      CASE WHEN @sortExpression = 'NumberOfLocations ASC' THEN "NumberOfLocations" END ASC,
                      CASE WHEN @sortExpression = 'NumberOfLocations DESC' THEN "NumberOfLocations" END DESC,
                      CASE WHEN @sortExpression = 'NumberOfFormsSubmitted ASC' THEN "NumberOfFormsSubmitted" END ASC,
                      CASE WHEN @sortExpression = 'NumberOfFormsSubmitted DESC' THEN "NumberOfFormsSubmitted" END DESC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            coalitionMemberId = req.CoalitionMemberId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            tagsFilter = req.TagsFilter ?? [],
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            datasource = req.DataSource?.ToString(),
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting),
            needsFollowUp = req.FollowUpStatus?.ToString()
        };

        int totalRowCount = 0;
        List<ObserverSubmissionOverview> entries = [];

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<ObserverSubmissionOverview>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<ObserverSubmissionOverview>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(ObserverSubmissionOverview.ObserverName)} ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.ObserverName),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverSubmissionOverview.ObserverName)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.Email),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverSubmissionOverview.Email)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.PhoneNumber),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverSubmissionOverview.PhoneNumber)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.Tags),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverSubmissionOverview.Tags)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverSubmissionOverview.NumberOfFlaggedAnswers)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.NumberOfLocations),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverSubmissionOverview.NumberOfLocations)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(ObserverSubmissionOverview.NumberOfFormsSubmitted),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(ObserverSubmissionOverview.NumberOfFormsSubmitted)} {sortOrder}";
        }

        return $"{nameof(ObserverSubmissionOverview.ObserverName)} ASC";
    }
}
