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
                  -- =====================================================================================
                  WITH "MonitoringNgoDetails" AS (
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
                          -- Get coalition id
                          (
                              SELECT
                                  C."Id"
                              FROM
                                  "CoalitionMemberships" CM
                                      JOIN "Coalitions" C ON CM."CoalitionId" = C."Id"
                              WHERE
                                  CM."MonitoringNgoId" = MN."Id"
                                AND CM."ElectionRoundId" = MN."ElectionRoundId"
                              LIMIT
                                  1
                          ) AS "CoalitionId"
                      FROM
                          "MonitoringNgos" MN
                      WHERE
                          MN."ElectionRoundId" = @electionRoundId
                        AND MN."NgoId" = @ngoId
                      LIMIT
                          1
                  ), -- if ngo is coalition leader they need to see all the responses
                       "AvailableMonitoringObservers" AS (
                           SELECT
                               MO."Id",
                               CASE WHEN (
                                   (
                                       SELECT
                                           "IsCoalitionLeader"
                                       FROM
                                           "MonitoringNgoDetails"
                                   )
                                       AND MN."NgoId" <> @ngoId
                                   ) THEN MO."Id" :: TEXT ELSE U."DisplayName" END AS "DisplayName",
                               CASE WHEN (
                                   (
                                       SELECT
                                           "IsCoalitionLeader"
                                       FROM
                                           "MonitoringNgoDetails"
                                   )
                                       AND MN."NgoId" <> @ngoId
                                   ) THEN MO."Id" :: TEXT ELSE U."Email" END AS "Email",
                               CASE WHEN (
                                   (
                                       SELECT
                                           "IsCoalitionLeader"
                                       FROM
                                           "MonitoringNgoDetails"
                                   )
                                       AND MN."NgoId" <> @ngoId
                                   ) THEN MO."Id" :: TEXT ELSE U."PhoneNumber" END AS "PhoneNumber",
                               CASE WHEN (
                                   (
                                       SELECT
                                           "IsCoalitionLeader"
                                       FROM
                                           "MonitoringNgoDetails"
                                   )
                                       AND MN."NgoId" <> @ngoId
                                   ) THEN '{}' :: TEXT[] ELSE MO."Tags" END AS "Tags",
                               MO."Status" AS "Status"
                           FROM
                               "Coalitions" C
                                   INNER JOIN "MonitoringNgoDetails" MND ON MND."CoalitionId" = C."Id"
                                   INNER JOIN "CoalitionMemberships" CM ON C."Id" = CM."CoalitionId"
                                   INNER JOIN "MonitoringObservers" MO ON MO."MonitoringNgoId" = CM."MonitoringNgoId"
                                   INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                                   INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                           WHERE
                               MND."CoalitionId" IS NOT NULL -- Case 1: `@dataSource` is "Ngo" or "Coalition" and they are not in a coalition
                             AND (
                               (
                                   (
                                       @dataSource = 'Ngo'
                                           OR @dataSource = 'Coalition'
                                       )
                                       AND NOT EXISTS (
                                       SELECT
                                           1
                                       FROM
                                           "MonitoringNgoDetails"
                                   )
                                       AND MN."NgoId" = @ngoId
                                   ) -- Case 2: `@dataSource` is "Coalition" and they are a coalition leader
                                   OR (
                                   @dataSource = 'Coalition'
                                       AND EXISTS (
                                       SELECT
                                           1
                                       FROM
                                           "MonitoringNgoDetails"
                                       WHERE
                                           "IsCoalitionLeader"
                                   )
                                   ) -- Case 3: `@dataSource` is "Ngo" and they are a coalition leader, apply `MN."NgoId" = @ngoId`
                                   OR (
                                   @dataSource = 'Ngo'
                                       AND EXISTS (
                                       SELECT
                                           1
                                       FROM
                                           "MonitoringNgoDetails"
                                       WHERE
                                           "IsCoalitionLeader"
                                   )
                                       AND MN."NgoId" = @ngoId
                                   ) -- Case 4: For all other cases, apply `MN."NgoId" = @ngoId`
                                   OR MN."NgoId" = @ngoId
                               )
                           UNION
                           SELECT
                               MO."Id",
                               U."DisplayName" AS "DisplayName",
                               U."Email" AS "Email",
                               U."PhoneNumber" AS "PhoneNumber",
                               MO."Tags" AS "Tags",
                               MO."Status" AS "Status"
                           FROM
                               "MonitoringObservers" MO
                                   INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                                   INNER JOIN "MonitoringNgoDetails" MND ON MND."MonitoringNgoId" = MO."MonitoringNgoId"
                                   INNER JOIN "MonitoringNgos" MN on mn."Id" = mo."MonitoringNgoId"
                           WHERE
                               MND."CoalitionId" IS NULL
                             and mn."NgoId" = @ngoId
                       )
                  SELECT
                      COUNT(*) count
                  FROM
                      "AvailableMonitoringObservers" MO
                  WHERE
                      (
                          @searchText IS NULL
                              OR @searchText = ''
                              OR mo."Id" :: TEXT ILIKE @searchText
                              OR mo."DisplayName" ILIKE @searchText
                              OR mo."Email" ILIKE @searchText
                              OR mo."PhoneNumber" ILIKE @searchText
                          )
                    AND (
                      @tagsFilter IS NULL
                          OR cardinality(@tagsFilter) = 0
                          OR mo."Tags" && @tagsFilter
                      );
                  WITH "MonitoringNgoDetails" AS (
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
                          -- Get coalition id
                          (
                              SELECT
                                  C."Id"
                              FROM
                                  "CoalitionMemberships" CM
                                      JOIN "Coalitions" C ON CM."CoalitionId" = C."Id"
                              WHERE
                                  CM."MonitoringNgoId" = MN."Id"
                                AND CM."ElectionRoundId" = MN."ElectionRoundId"
                              LIMIT
                                  1
                          ) AS "CoalitionId"
                      FROM
                          "MonitoringNgos" MN
                      WHERE
                          MN."ElectionRoundId" = @electionRoundId
                        AND MN."NgoId" = @ngoId
                      LIMIT
                          1
                  ), -- if ngo is coalition leader they need to see all the responses
                       "AvailableMonitoringObservers" AS (
                           SELECT
                               MO."Id",
                               CASE WHEN (
                                   (
                                       SELECT
                                           "IsCoalitionLeader"
                                       FROM
                                           "MonitoringNgoDetails"
                                   )
                                       AND MN."NgoId" <> @ngoId
                                   ) THEN MO."Id" :: TEXT ELSE U."DisplayName" END AS "DisplayName",
                               CASE WHEN (
                                   (
                                       SELECT
                                           "IsCoalitionLeader"
                                       FROM
                                           "MonitoringNgoDetails"
                                   )
                                       AND MN."NgoId" <> @ngoId
                                   ) THEN MO."Id" :: TEXT ELSE U."Email" END AS "Email",
                               CASE WHEN (
                                   (
                                       SELECT
                                           "IsCoalitionLeader"
                                       FROM
                                           "MonitoringNgoDetails"
                                   )
                                       AND MN."NgoId" <> @ngoId
                                   ) THEN MO."Id" :: TEXT ELSE U."PhoneNumber" END AS "PhoneNumber",
                               CASE WHEN (
                                   (
                                       SELECT
                                           "IsCoalitionLeader"
                                       FROM
                                           "MonitoringNgoDetails"
                                   )
                                       AND MN."NgoId" <> @ngoId
                                   ) THEN '{}' :: TEXT[] ELSE MO."Tags" END AS "Tags",
                               MO."Status" AS "Status"
                           FROM
                               "Coalitions" C
                                   INNER JOIN "MonitoringNgoDetails" MND ON MND."CoalitionId" = C."Id"
                                   INNER JOIN "CoalitionMemberships" CM ON C."Id" = CM."CoalitionId"
                                   INNER JOIN "MonitoringObservers" MO ON MO."MonitoringNgoId" = CM."MonitoringNgoId"
                                   INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                                   INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                           WHERE
                               MND."CoalitionId" IS NOT NULL -- Case 1: `@dataSource` is "Ngo" or "Coalition" and they are not in a coalition
                             AND (
                               (
                                   (
                                       @dataSource = 'Ngo'
                                           OR @dataSource = 'Coalition'
                                       )
                                       AND NOT EXISTS (
                                       SELECT
                                           1
                                       FROM
                                           "MonitoringNgoDetails"
                                   )
                                       AND MN."NgoId" = @ngoId
                                   ) -- Case 2: `@dataSource` is "Coalition" and they are a coalition leader
                                   OR (
                                   @dataSource = 'Coalition'
                                       AND EXISTS (
                                       SELECT
                                           1
                                       FROM
                                           "MonitoringNgoDetails"
                                       WHERE
                                           "IsCoalitionLeader"
                                   )
                                   ) -- Case 3: `@dataSource` is "Ngo" and they are a coalition leader, apply `MN."NgoId" = @ngoId`
                                   OR (
                                   @dataSource = 'Ngo'
                                       AND EXISTS (
                                       SELECT
                                           1
                                       FROM
                                           "MonitoringNgoDetails"
                                       WHERE
                                           "IsCoalitionLeader"
                                   )
                                       AND MN."NgoId" = @ngoId
                                   ) -- Case 4: For all other cases, apply `MN."NgoId" = @ngoId`
                                   OR MN."NgoId" = @ngoId
                               )
                           UNION
                           SELECT
                               MO."Id",
                               U."DisplayName" AS "DisplayName",
                               U."Email" AS "Email",
                               U."PhoneNumber" AS "PhoneNumber",
                               MO."Tags" AS "Tags",
                               MO."Status" AS "Status"
                           FROM
                               "MonitoringObservers" MO
                                   INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                                   INNER JOIN "MonitoringNgoDetails" MND ON MND."MonitoringNgoId" = MO."MonitoringNgoId"
                                   INNER JOIN "MonitoringNgos" MN on mn."Id" = mo."MonitoringNgoId"
                           WHERE
                               MND."CoalitionId" IS NULL
                             and mn."NgoId" = @ngoId
                       ),
                       "AvailableForms" AS (
                           SELECT
                               F."Id"
                           FROM
                               "CoalitionFormAccess" CFA
                                   INNER JOIN "Coalitions" C ON CFA."CoalitionId" = C."Id"
                                   inner join "MonitoringNgoDetails" mnd on mnd."CoalitionId" = c."Id"
                                   INNER JOIN "Forms" F ON CFA."FormId" = F."Id"
                                   inner join "MonitoringNgos" mn on mn."Id" = mnd."MonitoringNgoId"
                           WHERE
                               MND."CoalitionId" IS NOT NULL -- Case 1: `@dataSource` is "Ngo" or "Coalition" and they are not in a coalition
                             AND (
                               (
                                   (
                                       @dataSource = 'Ngo'
                                           OR @dataSource = 'Coalition'
                                       )
                                       AND NOT EXISTS (
                                       SELECT
                                           1
                                       FROM
                                           "MonitoringNgoDetails"
                                   )
                                       AND MN."NgoId" = @ngoId
                                   ) -- Case 2: `@dataSource` is "Coalition" and they are a coalition leader
                                   OR (
                                   @dataSource = 'Coalition'
                                       AND EXISTS (
                                       SELECT
                                           1
                                       FROM
                                           "MonitoringNgoDetails"
                                       WHERE
                                           "IsCoalitionLeader"
                                   )
                                   ) -- Case 3: `@dataSource` is "Ngo" and they are a coalition leader, apply `MN."NgoId" = @ngoId`
                                   OR (
                                   @dataSource = 'Ngo'
                                       AND EXISTS (
                                       SELECT
                                           1
                                       FROM
                                           "MonitoringNgoDetails"
                                       WHERE
                                           "IsCoalitionLeader"
                                   )
                                       AND MN."NgoId" = @ngoId
                                   ) -- Case 4: For all other cases, apply `MN."NgoId" = @ngoId`
                                   OR MN."NgoId" = @ngoId
                               )
                           UNION
                           SELECT
                               F."Id"
                           FROM
                               "Forms" F
                                   INNER JOIN "MonitoringNgos" MN ON MN."Id" = F."MonitoringNgoId"
                           WHERE
                               f."ElectionRoundId" = @electionRoundId
                             AND MN."NgoId" = @ngoId
                       )
                  SELECT
                      "MonitoringObserverId",
                      "ObserverName",
                      "PhoneNumber",
                      "Email",
                      "Tags",
                      "NumberOfFlaggedAnswers",
                      "NumberOfLocations",
                      "NumberOfFormsSubmitted",
                      "FollowUpStatus"
                  FROM
                      (
                          SELECT
                              MO."Id" "MonitoringObserverId",
                              Mo."DisplayName" "ObserverName",
                              Mo."PhoneNumber",
                              Mo."Email",
                              MO."Tags",
                              COALESCE(
                                      (
                                          SELECT
                                              SUM("NumberOfFlaggedAnswers")
                                          FROM
                                              "FormSubmissions" FS
                                                  inner join "AvailableForms" f on f."Id" = fs."FormId"
                                          WHERE
                                              FS."MonitoringObserverId" = MO."Id"
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
                                              PSI."MonitoringObserverId" = MO."Id"
                                            AND PSI."ElectionRoundId" = @electionRoundId
                                          UNION
                                          SELECT
                                              FS."PollingStationId"
                                          FROM
                                              "FormSubmissions" FS
                                                  inner join "AvailableForms" f on f."Id" = fs."FormId"
                                          WHERE
                                              FS."MonitoringObserverId" = MO."Id"
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
                                              PSI."MonitoringObserverId" = MO."Id"
                                            AND PSI."ElectionRoundId" = @electionRoundId
                                          UNION
                                          SELECT
                                              FS."Id"
                                          FROM
                                              "FormSubmissions" FS
                                                  inner join "AvailableForms" f on f."Id" = fs."FormId"
                                          WHERE
                                              FS."MonitoringObserverId" = MO."Id"
                                            AND FS."ElectionRoundId" = @electionRoundId
                                      ) TMP
                              ) AS "NumberOfFormsSubmitted",
                              (
                                  CASE WHEN EXISTS (
                                      SELECT
                                          1
                                      FROM
                                          "FormSubmissions" FS
                                              inner join "AvailableForms" f on f."Id" = fs."FormId"
                                      WHERE
                                          FS."FollowUpStatus" = 'NeedsFollowUp'
                                        AND FS."MonitoringObserverId" = MO."Id"
                                        AND FS."ElectionRoundId" = @electionRoundId
                                  ) THEN 'NeedsFollowUp' ELSE NULL END
                                  ) AS "FollowUpStatus"
                          FROM
                              "AvailableMonitoringObservers" MO
                          WHERE
                              (
                                  @searchText IS NULL
                                      OR @searchText = ''
                                      OR mo."Id" :: TEXT ILIKE @searchText
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
