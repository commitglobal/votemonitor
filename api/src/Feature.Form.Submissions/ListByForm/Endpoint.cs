using Feature.Form.Submissions.Requests;

namespace Feature.Form.Submissions.ListByForm;

public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<FormSubmissionsAggregateFilter, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:byForm");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminsOnly);

        Summary(x => { x.Summary = "Form submissions aggregated by form"; });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(FormSubmissionsAggregateFilter req,
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
            			FS."NumberOfQuestionsAnswered",
            			FS."Answers",
            			FS."FollowUpStatus",
            			FS."IsCompleted",
            			"LastUpdatedAt" AS "TimeSubmitted",
            			COALESCE(
            				(
            					SELECT
            						JSONB_AGG(
            							JSONB_BUILD_OBJECT(
            								'QuestionId',
            								"QuestionId",
            								'FileName',
            								"FileName",
            								'MimeType',
            								"MimeType",
            								'FilePath',
            								"FilePath",
            								'UploadedFileName',
            								"UploadedFileName",
            								'TimeSubmitted',
            								"LastUpdatedAt"
            							)
            						)
            					FROM
            						"Attachments" A
            					WHERE
            						(
                                        (A."FormId" = FS."FormId" AND FS."PollingStationId" = A."PollingStationId") -- backwards compatibility
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
            							JSONB_BUILD_OBJECT(
            								'QuestionId',
            								"QuestionId",
            								'Text',
            								"Text",
            								'TimeSubmitted',
            								"LastUpdatedAt"
            							)
            						)
            					FROM
            						"Notes" N
            					WHERE
            						(
                                        (N."FormId" = FS."FormId" AND FS."PollingStationId" = N."PollingStationId") -- backwards compatibility
                                        OR N."SubmissionId" = FS."Id"
                                    )
            						AND N."MonitoringObserverId" = FS."MonitoringObserverId"
            				),
            				'[]'::JSONB
            			) AS "Notes",
            			PS."Id" AS "PollingStationId",
            			PS."Level1" AS "PollingStationLevel1",
            			PS."Level2" AS "PollingStationLevel2",
            			PS."Level3" AS "PollingStationLevel3",
            			PS."Level4" AS "PollingStationLevel4",
            			PS."Level5" AS "PollingStationLevel5",
            			PS."Number" AS "PollingStationNumber",
            			F."Id" AS "FormId",
            			F."Code" AS "FormCode",
            			F."FormType" AS "FormType",
            			F."Name" AS "FormName",
            			F."DefaultLanguage" AS "FormDefaultLanguage",
            			F."Questions" AS "FormQuestions",
            			MO."MonitoringObserverId",
            			MO."DisplayName",
            			MO."Tags",
            			MO."Status",
            			MO."Email",
            			MO."PhoneNumber",
            			MO."NgoId"
            		FROM
            			"FormSubmissions" FS
            			INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON FS."MonitoringObserverId" = MO."MonitoringObserverId"
            			INNER JOIN "PollingStations" PS ON PS."Id" = FS."PollingStationId"
            			INNER JOIN "Forms" F ON F."Id" = FS."FormId"
            		WHERE
            			FS."ElectionRoundId" = @ELECTIONROUNDID
            	),
            	PSI_SUBMISSIONS AS (
            		SELECT
            			PSI."Id" AS "SubmissionId",
            			PSI."NumberOfFlaggedAnswers",
            			PSI."NumberOfQuestionsAnswered",
            			PSI."Answers",
            			PSI."FollowUpStatus",
            			PSI."IsCompleted",
            			"LastUpdatedAt" AS "TimeSubmitted",
            			'[]'::JSONB AS "Attachments",
            			'[]'::JSONB AS "Notes",
            			PS."Id" AS "PollingStationId",
            			PS."Level1" AS "PollingStationLevel1",
            			PS."Level2" AS "PollingStationLevel2",
            			PS."Level3" AS "PollingStationLevel3",
            			PS."Level4" AS "PollingStationLevel4",
            			PS."Level5" AS "PollingStationLevel5",
            			PS."Number" AS "PollingStationNumber",
            			PSIF."Id" AS "FormId",
            			PSIF."Code" AS "FormCode",
            			PSIF."FormType" AS "FormType",
            			PSIF."Name" AS "FormName",
            			PSIF."DefaultLanguage" AS "FormDefaultLanguage",
            			PSIF."Questions" AS "FormQuestions",
            			MO."MonitoringObserverId",
            			MO."DisplayName",
            			MO."Tags",
            			MO."Status",
            			MO."Email",
            			MO."PhoneNumber",
            			MO."NgoId"
            		FROM
            			"PollingStationInformation" PSI
            			INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON PSI."MonitoringObserverId" = MO."MonitoringObserverId"
            			INNER JOIN "PollingStations" PS ON PSI."PollingStationId" = PS."Id"
            			INNER JOIN "PollingStationInformationForms" PSIF ON PSIF."Id" = PSI."PollingStationInformationFormId"
            		WHERE
            			PSI."ElectionRoundId" = @ELECTIONROUNDID
            	),
            	ALL_SUBMISSIONS AS (
            		SELECT
            			*
            		FROM
            			PSI_SUBMISSIONS
            		UNION
            		SELECT
            			*
            		FROM
            			FORM_SUBMISSIONS
            	),
            	FILTERED_SUBMISSIONS AS (
            		SELECT
            			*
            		FROM
            			ALL_SUBMISSIONS FS
            		WHERE
            			(
            				@COALITIONMEMBERID IS NULL
            				OR FS."NgoId" = @COALITIONMEMBERID
            			)
            			AND (
            				@LEVEL1 IS NULL
            				OR FS."PollingStationLevel1" = @LEVEL1
            			)
            			AND (
            				@LEVEL2 IS NULL
            				OR FS."PollingStationLevel2" = @LEVEL2
            			)
            			AND (
            				@LEVEL3 IS NULL
            				OR FS."PollingStationLevel3" = @LEVEL3
            			)
            			AND (
            				@LEVEL4 IS NULL
            				OR FS."PollingStationLevel4" = @LEVEL4
            			)
            			AND (
            				@LEVEL5 IS NULL
            				OR FS."PollingStationLevel5" = @LEVEL5
            			)
            			AND (
            				@POLLINGSTATIONNUMBER IS NULL
            				OR FS."PollingStationNumber" = @POLLINGSTATIONNUMBER
            			)
            			AND (
            				@HASFLAGGEDANSWERS IS NULL
            				OR (
            					"NumberOfFlaggedAnswers" = 0
            					AND @HASFLAGGEDANSWERS = FALSE
            				)
            				OR (
            					"NumberOfFlaggedAnswers" > 0
            					AND @HASFLAGGEDANSWERS = TRUE
            				)
            			)
            			AND (
            				@FOLLOWUPSTATUS IS NULL
            				OR "FollowUpStatus" = @FOLLOWUPSTATUS
            			)
            			AND (
            				@TAGSFILTER IS NULL
            				OR CARDINALITY(@TAGSFILTER) = 0
            				OR FS."Tags" && @TAGSFILTER
            			)
            			AND (
            				@MONITORINGOBSERVERSTATUS IS NULL
            				OR FS."Status" = @MONITORINGOBSERVERSTATUS
            			)
            			AND (
            				@FORMID IS NULL
            				OR FS."FormId" = @FORMID
            			)
            			AND (
            				@QUESTIONSANSWERED IS NULL
            				OR (
            					@QUESTIONSANSWERED = 'All'
            					AND JSONB_ARRAY_LENGTH(FS."FormQuestions") = FS."NumberOfQuestionsAnswered"
            				)
            				OR (
            					@QUESTIONSANSWERED = 'Some'
            					AND JSONB_ARRAY_LENGTH(FS."FormQuestions") <> FS."NumberOfQuestionsAnswered"
            				)
            				OR (
            					@QUESTIONSANSWERED = 'None'
            					AND FS."NumberOfQuestionsAnswered" = 0
            				)
            			)
            			AND (
            				@HASATTACHMENTS IS NULL
            				OR (
            					JSONB_ARRAY_LENGTH(FS."Attachments") = 0
            					AND @HASATTACHMENTS = FALSE
            				)
            				OR (
            					JSONB_ARRAY_LENGTH(FS."Attachments") > 0
            					AND @HASATTACHMENTS = TRUE
            				)
            			)
            			AND (
            				@HASNOTES IS NULL
            				OR (
            					JSONB_ARRAY_LENGTH(FS."Notes") = 0
            					AND @HASNOTES = FALSE
            				)
            				OR (
            					JSONB_ARRAY_LENGTH(FS."Notes") > 0
            					AND @HASNOTES = TRUE
            				)
            			)
            	)
            SELECT
            	AF."FormId",
            	AF."FormCode",
            	AF."FormType",
            	AF."FormName",
            	AF."FormDefaultLanguage",
            	COUNT(FS."SubmissionId") AS "NumberOfSubmissions",
            	COALESCE(SUM(FS."NumberOfFlaggedAnswers"), 0) AS "NumberOfFlaggedAnswers",
            	COALESCE(SUM(JSONB_ARRAY_LENGTH(FS."Attachments")), 0) AS "NumberOfMediaFiles",
            	COALESCE(SUM(JSONB_ARRAY_LENGTH(FS."Notes")), 0) AS "NumberOfNotes"
            FROM
            	"GetAvailableForms" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) AF
            	LEFT JOIN FILTERED_SUBMISSIONS FS ON FS."FormId" = AF."FormId"
            WHERE
            	AF."FormStatus" <> 'Drafted'
            	AND AF."FormType" NOT IN ('CitizenReporting')
            GROUP BY
            	AF."FormId",
            	AF."FormCode",
            	AF."FormType",
            	AF."FormName",
            	AF."FormDefaultLanguage";
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            dataSource = req.DataSource.ToString(),
            coalitionMemberId= req.CoalitionMemberId,
            level1 = req.Level1Filter,
            level2 = req.Level2Filter,
            level3 = req.Level3Filter,
            level4 = req.Level4Filter,
            level5 = req.Level5Filter,
            pollingStationNumber = req.PollingStationNumberFilter,
            hasFlaggedAnswers = req.HasFlaggedAnswers,
            followUpStatus = req.FollowUpStatus?.ToString(),
            tagsFilter = req.TagsFilter ?? [],
            monitoringObserverStatus = req.MonitoringObserverStatus?.ToString(),
            formId = req.FormId,
            hasNotes = req.HasNotes,
            hasAttachments = req.HasAttachments,
            questionsAnswered = req.QuestionsAnswered?.ToString(),
        };

        IEnumerable<AggregatedFormOverview> aggregatedFormOverviews;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            aggregatedFormOverviews = await dbConnection.QueryAsync<AggregatedFormOverview>(sql, queryArgs);
        }

        return TypedResults.Ok(new Response { AggregatedForms = aggregatedFormOverviews.ToList() });
    }
}
