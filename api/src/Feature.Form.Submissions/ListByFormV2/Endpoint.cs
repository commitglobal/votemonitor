using Feature.Form.Submissions.ListByForm;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Form.Submissions.ListByFormV2;

public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<AggregatedFormOverview>>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/form-submissions:byFormV2");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminsOnly);

        Summary(x => { x.Summary = "Form submissions aggregated by form (v2)"; });
    }

    public override async Task<Results<Ok<PagedResponse<AggregatedFormOverview>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql =
            """
            WITH
            	FORM_SUBMISSIONS AS (
            		SELECT
            			FS."Id" AS "SubmissionId",
            			FS."NumberOfFlaggedAnswers",
            			COALESCE(
            				(
            					SELECT
            						JSONB_AGG(
            							JSONB_BUILD_OBJECT('QuestionId', "QuestionId")
            						)
            					FROM
            						"Attachments" A
            					WHERE
            						(
                                        (A."FormId" = FS."FormId" AND FS."PollingStationId" = A."PollingStationId")
                                        OR A."SubmissionId" = FS."Id"
                                    )
            						AND A."MonitoringObserverId" = FS."MonitoringObserverId"
            						AND A."IsDeleted" = FALSE
            						AND A."IsCompleted" = TRUE
            				),
            				'[]'::JSONB
            			) AS "Attachments",
            			COALESCE(
            				(
            					SELECT
            						JSONB_AGG(
            							JSONB_BUILD_OBJECT('QuestionId', "QuestionId")
            						)
            					FROM
            						"Notes" N
            					WHERE
            						(
                                        (N."FormId" = FS."FormId" AND FS."PollingStationId" = N."PollingStationId")
                                        OR N."SubmissionId" = FS."Id"
                                    )
            						AND N."MonitoringObserverId" = FS."MonitoringObserverId"
            				),
            				'[]'::JSONB
            			) AS "Notes",
            			F."Id" AS "FormId"
            		FROM
            			"FormSubmissions" FS
            			INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON FS."MonitoringObserverId" = MO."MonitoringObserverId"
            			INNER JOIN "Forms" F ON F."Id" = FS."FormId"
            		WHERE
            			FS."ElectionRoundId" = @ELECTIONROUNDID
            	),
            	PSI_SUBMISSIONS AS (
            		SELECT
            			PSI."Id" AS "SubmissionId",
            			PSI."NumberOfFlaggedAnswers",
            			'[]'::JSONB AS "Attachments",
            			'[]'::JSONB AS "Notes",
            			PSIF."Id" AS "FormId"
            		FROM
            			"PollingStationInformation" PSI
            			INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON PSI."MonitoringObserverId" = MO."MonitoringObserverId"
            			INNER JOIN "PollingStationInformationForms" PSIF ON PSIF."Id" = PSI."PollingStationInformationFormId"
            		WHERE
            			PSI."ElectionRoundId" = @ELECTIONROUNDID
            	),
            	ALL_SUBMISSIONS AS (
            		SELECT * FROM PSI_SUBMISSIONS
            		UNION ALL
            		SELECT * FROM FORM_SUBMISSIONS
            	),
            	AGGREGATED AS (
            		SELECT
            			AF."FormId",
            			AF."FormCode",
            			AF."FormType",
            			AF."FormName",
            			AF."FormDefaultLanguage" AS "DefaultLanguage",
            			COUNT(FS."SubmissionId") AS "NumberOfSubmissions",
            			COALESCE(SUM(FS."NumberOfFlaggedAnswers"), 0) AS "NumberOfFlaggedAnswers",
            			COALESCE(SUM(JSONB_ARRAY_LENGTH(FS."Attachments")), 0) AS "NumberOfMediaFiles",
            			COALESCE(SUM(JSONB_ARRAY_LENGTH(FS."Notes")), 0) AS "NumberOfNotes"
            		FROM
            			"GetAvailableForms" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) AF
            			LEFT JOIN ALL_SUBMISSIONS FS ON FS."FormId" = AF."FormId"
            		WHERE
            			AF."FormStatus" <> 'Drafted'
            			AND AF."FormType" NOT IN ('CitizenReporting')
            		GROUP BY
            			AF."FormId",
            			AF."FormCode",
            			AF."FormType",
            			AF."FormName",
            			AF."FormDefaultLanguage"
            	)
            SELECT COUNT(1) FROM AGGREGATED;

            SELECT *
            FROM AGGREGATED
            ORDER BY
                CASE WHEN @sortExpression = 'FormCode ASC' THEN "FormCode" END ASC,
                CASE WHEN @sortExpression = 'FormCode DESC' THEN "FormCode" END DESC,
                CASE WHEN @sortExpression = 'FormType ASC' THEN "FormType" END ASC,
                CASE WHEN @sortExpression = 'FormType DESC' THEN "FormType" END DESC,
                CASE WHEN @sortExpression = 'NumberOfSubmissions ASC' THEN "NumberOfSubmissions" END ASC,
                CASE WHEN @sortExpression = 'NumberOfSubmissions DESC' THEN "NumberOfSubmissions" END DESC,
                CASE WHEN @sortExpression = 'NumberOfFlaggedAnswers ASC' THEN "NumberOfFlaggedAnswers" END ASC,
                CASE WHEN @sortExpression = 'NumberOfFlaggedAnswers DESC' THEN "NumberOfFlaggedAnswers" END DESC,
                CASE WHEN @sortExpression = 'NumberOfNotes ASC' THEN "NumberOfNotes" END ASC,
                CASE WHEN @sortExpression = 'NumberOfNotes DESC' THEN "NumberOfNotes" END DESC,
                CASE WHEN @sortExpression = 'NumberOfMediaFiles ASC' THEN "NumberOfMediaFiles" END ASC,
                CASE WHEN @sortExpression = 'NumberOfMediaFiles DESC' THEN "NumberOfMediaFiles" END DESC,
                "FormCode" ASC
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            dataSource = req.DataSource.ToString(),
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        int totalRowCount;
        List<AggregatedFormOverview> aggregatedFormOverviews;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            aggregatedFormOverviews = multi.Read<AggregatedFormOverview>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<AggregatedFormOverview>(aggregatedFormOverviews, totalRowCount, req.PageNumber,
                req.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return "FormCode ASC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.FormCode),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"FormCode {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.FormType),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"FormType {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.NumberOfSubmissions),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"NumberOfSubmissions {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"NumberOfFlaggedAnswers {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.NumberOfNotes),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"NumberOfNotes {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.NumberOfMediaFiles),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"NumberOfMediaFiles {sortOrder}";
        }

        return "FormCode ASC";
    }
}
