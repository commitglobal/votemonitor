using Feature.Form.Submissions.Requests;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Feature.Form.Submissions.GetAggregated;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    INpgsqlConnectionFactory connectionFactory,
    IFileStorageService fileStorageService) : Endpoint<FormSubmissionsAggregateFilter, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/{formId}:aggregated");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Summary(s => { s.Summary = "Gets aggregated form with all the notes and attachments"; });
        Policies(PolicyNames.NgoAdminsOnly);
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

        var form = await context
            .Forms
            .FromSqlInterpolated($"""
                                  select f.* from "GetAvailableForms"({req.ElectionRoundId}, {req.NgoId}, {req.DataSource.ToString()}) af
                                  inner join "Forms" f on f."Id" = af."FormId"
                                  """)
            .Where(x => x.Id == req.FormId)
            .Where(x => x.Status == FormStatus.Published)
            .Where(x => x.FormType != FormType.CitizenReporting && x.FormType != FormType.IncidentReporting)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (form is not null)
        {
            return await AggregateNgoFormSubmissionsAsync(form, req, ct);
        }

        var psiForm = await context
            .PollingStationInformationForms
            .Where(x => x.ElectionRoundId == req.ElectionRoundId)
            .FirstOrDefaultAsync(ct);

        if (psiForm is not null)
        {
            return await AggregatePSIFormSubmissionsAsync(psiForm, req, ct);
        }

        return TypedResults.NotFound();
    }

    private async Task<Results<Ok<Response>, NotFound>> AggregateNgoFormSubmissionsAsync(FormAggregate form,
        FormSubmissionsAggregateFilter req,
        CancellationToken ct)
    {
        var submissionsSql = """
                             WITH
                             	POLLING_STATION_SUBMISSIONS AS (
                             		SELECT
                             			PSI."Id" AS "SubmissionId",
                             			'PSI' AS "FormType",
                             			'PSI' AS "FormCode",
                             			PSI."PollingStationId",
                             			PSI."MonitoringObserverId",
                             			PSI."NumberOfQuestionsAnswered",
                             			PSI."NumberOfFlaggedAnswers",
                             			0 AS "MediaFilesCount",
                             			0 AS "NotesCount",
                             			'[]'::JSONB AS "Attachments",
                             			'[]'::JSONB AS "Notes",
                             			PSI."LastUpdatedAt" AS "TimeSubmitted",
                             			PSI."FollowUpStatus",
                             			PSIF."DefaultLanguage",
                             			PSIF."Name",
                             			PSI."IsCompleted",
                             			PSI."Answers"
                             		FROM
                             			"PollingStationInformation" PSI
                             			INNER JOIN "PollingStationInformationForms" PSIF ON PSIF."Id" = PSI."PollingStationInformationFormId"
                             			INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId"
                             		WHERE
                             			PSI."ElectionRoundId" = @ELECTIONROUNDID
                             			AND (@COALITIONMEMBERID IS NULL OR mo."NgoId" = @COALITIONMEMBERID)
                             			AND (
                             				@MONITORINGOBSERVERSTATUS IS NULL
                             				OR MO."Status" = @MONITORINGOBSERVERSTATUS
                             			)
                             			AND (
                             				@FORMID IS NULL
                             				OR PSI."PollingStationInformationFormId" = @FORMID
                             			)
                             			AND (
                             				@FROMDATE IS NULL
                             				OR PSI."LastUpdatedAt" >= @FROMDATE::TIMESTAMP
                             			)
                             			AND (
                             				@TODATE IS NULL
                             				OR PSI."LastUpdatedAt" <= @TODATE::TIMESTAMP
                             			)
                             			AND (
                             				@QUESTIONSANSWERED IS NULL
                             				OR (
                             					@QUESTIONSANSWERED = 'All'
                             					AND PSIF."NumberOfQuestions" = PSI."NumberOfQuestionsAnswered"
                             				)
                             				OR (
                             					@QUESTIONSANSWERED = 'Some'
                             					AND PSIF."NumberOfQuestions" <> PSI."NumberOfQuestionsAnswered"
                             				)
                             				OR (
                             					@QUESTIONSANSWERED = 'None'
                             					AND PSI."NumberOfQuestionsAnswered" = 0
                             				)
                             			)
                             	),
                             	FORM_SUBMISSIONS AS (
                             		SELECT
                             			FS."Id" AS "SubmissionId",
                             			F."FormType",
                             			F."Code" AS "FormCode",
                             			FS."PollingStationId",
                             			FS."MonitoringObserverId",
                             			FS."NumberOfQuestionsAnswered",
                             			FS."NumberOfFlaggedAnswers",
                             			(
                             				SELECT
                             					COUNT(1)
                             				FROM
                             					"Attachments" A
                             				WHERE
                             					A."FormId" = FS."FormId"
                             					AND A."MonitoringObserverId" = FS."MonitoringObserverId"
                             					AND FS."PollingStationId" = A."PollingStationId"
                             					AND A."IsDeleted" = FALSE
                             					AND A."IsCompleted" = TRUE
                             			) AS "MediaFilesCount",
                             			(
                             				SELECT
                             					COUNT(1)
                             				FROM
                             					"Notes" N
                             				WHERE
                             					N."FormId" = FS."FormId"
                             					AND N."MonitoringObserverId" = FS."MonitoringObserverId"
                             					AND FS."PollingStationId" = N."PollingStationId"
                             			) AS "NotesCount",
                             			COALESCE(
                             				(
                             					SELECT
                             						JSONB_AGG(
                             							JSONB_BUILD_OBJECT(
                             							    'SubmissionId',
                             							    FS."Id",
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
                             						A."ElectionRoundId" = @ELECTIONROUNDID
                             						AND A."FormId" = FS."FormId"
                             						AND A."MonitoringObserverId" = FS."MonitoringObserverId"
                             						AND A."IsDeleted" = FALSE
                             						AND A."IsCompleted" = TRUE
                             						AND FS."PollingStationId" = A."PollingStationId"
                             				),
                             				'[]'::JSONB
                             			) AS "Attachments",
                             			COALESCE(
                             				(
                             					SELECT
                             						JSONB_AGG(
                             							JSONB_BUILD_OBJECT(
                             							    'SubmissionId',
                             							    FS."Id",
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
                             						N."ElectionRoundId" = @ELECTIONROUNDID
                             						AND N."FormId" = FS."FormId"
                             						AND N."MonitoringObserverId" = FS."MonitoringObserverId"
                             						AND FS."PollingStationId" = N."PollingStationId"
                             				),
                             				'[]'::JSONB
                             			) AS "Notes",
                             			"LastUpdatedAt" AS "TimeSubmitted",
                             			FS."FollowUpStatus",
                             			F."DefaultLanguage",
                             			F."Name",
                             			FS."IsCompleted",
                             			FS."Answers"
                             		FROM
                             			"FormSubmissions" FS
                             			INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                             			INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON FS."MonitoringObserverId" = MO."MonitoringObserverId"
                             			INNER JOIN "GetAvailableForms" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) AF ON AF."FormId" = FS."FormId"
                             		WHERE
                             			FS."ElectionRoundId" = @ELECTIONROUNDID
                                        AND (@COALITIONMEMBERID IS NULL OR mo."NgoId" = @COALITIONMEMBERID)
                             			AND (
                             				@MONITORINGOBSERVERSTATUS IS NULL
                             				OR MO."Status" = @MONITORINGOBSERVERSTATUS
                             			)
                             			AND (
                             				@FORMID IS NULL
                             				OR FS."FormId" = @FORMID
                             			)
                             			AND (
                             				@FROMDATE IS NULL
                             				OR "LastUpdatedAt" >= @FROMDATE::TIMESTAMP
                             			)
                             			AND (
                             				@TODATE IS NULL
                             				OR "LastUpdatedAt" <= @TODATE::TIMESTAMP
                             			)
                             			AND (
                             				@QUESTIONSANSWERED IS NULL
                             				OR (
                             					@QUESTIONSANSWERED = 'All'
                             					AND F."NumberOfQuestions" = FS."NumberOfQuestionsAnswered"
                             				)
                             				OR (
                             					@QUESTIONSANSWERED = 'Some'
                             					AND F."NumberOfQuestions" <> FS."NumberOfQuestionsAnswered"
                             				)
                             				OR (
                             					@QUESTIONSANSWERED = 'None'
                             					AND FS."NumberOfQuestionsAnswered" = 0
                             				)
                             			)
                             	)
                             SELECT
                             	S."SubmissionId",
                             	S."TimeSubmitted",
                             	S."FormCode",
                             	S."FormType",
                             	S."DefaultLanguage",
                             	S."Name" AS "FormName",
                             	PS."Id" AS "PollingStationId",
                             	PS."Level1",
                             	PS."Level2",
                             	PS."Level3",
                             	PS."Level4",
                             	PS."Level5",
                             	PS."Number",
                             	S."MonitoringObserverId",
                             	MO."DisplayName" AS "ObserverName",
                             	MO."Email",
                             	MO."PhoneNumber",
                             	MO."Status",
                             	MO."Tags",
                             	MO."NgoName",
                             	S."NumberOfQuestionsAnswered",
                             	S."NumberOfFlaggedAnswers",
                             	S."MediaFilesCount",
                             	S."NotesCount",
                             	S."FollowUpStatus",
                             	S."IsCompleted",
                             	MO."Status" "MonitoringObserverStatus",
                             	S."Notes",
                             	S."Attachments",
                             	S."Answers"
                             FROM
                             	(
                             		SELECT
                             			*
                             		FROM
                             			POLLING_STATION_SUBMISSIONS
                             		UNION ALL
                             		SELECT
                             			*
                             		FROM
                             			FORM_SUBMISSIONS
                             	) S
                             	INNER JOIN "PollingStations" PS ON PS."Id" = S."PollingStationId"
                             	INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON MO."MonitoringObserverId" = S."MonitoringObserverId"
                             WHERE
                                (@COALITIONMEMBERID IS NULL OR mo."NgoId" = @COALITIONMEMBERID)
                             	AND (
                             		@LEVEL1 IS NULL
                             		OR PS."Level1" = @LEVEL1
                             	)
                             	AND (
                             		@LEVEL2 IS NULL
                             		OR PS."Level2" = @LEVEL2
                             	)
                             	AND (
                             		@LEVEL3 IS NULL
                             		OR PS."Level3" = @LEVEL3
                             	)
                             	AND (
                             		@LEVEL4 IS NULL
                             		OR PS."Level4" = @LEVEL4
                             	)
                             	AND (
                             		@LEVEL5 IS NULL
                             		OR PS."Level5" = @LEVEL5
                             	)
                             	AND (
                             		@POLLINGSTATIONNUMBER IS NULL
                             		OR PS."Number" = @POLLINGSTATIONNUMBER
                             	)
                             	AND (
                             		@HASFLAGGEDANSWERS IS NULL
                             		OR (
                             			S."NumberOfFlaggedAnswers" = 0
                             			AND @HASFLAGGEDANSWERS = FALSE
                             		)
                             		OR (
                             			S."NumberOfFlaggedAnswers" > 0
                             			AND @HASFLAGGEDANSWERS = TRUE
                             		)
                             	)
                             	AND (
                             		@FOLLOWUPSTATUS IS NULL
                             		OR S."FollowUpStatus" = @FOLLOWUPSTATUS
                             	)
                             	AND (
                             		@TAGSFILTER IS NULL
                             		OR CARDINALITY(@TAGSFILTER) = 0
                             		OR MO."Tags" && @TAGSFILTER
                             	)
                             	AND (
                             		@HASNOTES IS NULL
                             		OR (
                             			S."NotesCount" = 0
                             			AND @HASNOTES = FALSE
                             		)
                             		OR (
                             			S."NotesCount" > 0
                             			AND @HASNOTES = TRUE
                             		)
                             	)
                             	AND (
                             		@HASATTACHMENTS IS NULL
                             		OR (
                             			S."MediaFilesCount" = 0
                             			AND @HASATTACHMENTS = FALSE
                             		)
                             		OR (
                             			S."MediaFilesCount" > 0
                             			AND @HASATTACHMENTS = TRUE
                             		)
                             	)
                             """;

        var submissionsQueryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            coalitionMemberId = req.CoalitionMemberId,
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
            fromDate = req.FromDateFilter?.ToString("O"),
            toDate = req.ToDateFilter?.ToString("O"),
            dataSource = req.DataSource?.ToString(),
        };

        List<FormSubmissionView> submissions;

        using (var dbConnection = await connectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(submissionsSql, submissionsQueryArgs);
            submissions = multi.Read<FormSubmissionView>().ToList();
        }

        var formSubmissionsAggregate = new FormSubmissionsAggregate(form);
        foreach (var formSubmission in submissions)
        {
            formSubmissionsAggregate.AggregateAnswers(formSubmission);
        }
        
        var notes = submissions
            .SelectMany(x => x.Notes.Select(note => note with { SubmissionId = x.SubmissionId }))
            .ToList();

        var attachments = submissions
            .SelectMany(x => x.Attachments.Select(attachment => attachment with { SubmissionId = x.SubmissionId }));

        attachments = await Task.WhenAll(
            attachments.Select(async attachment =>
            {
                var result = await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);
                return result is GetPresignedUrlResult.Ok(var url, _, var urlValidityInSeconds)
                    ? attachment with { PresignedUrl = url, UrlValidityInSeconds = urlValidityInSeconds }
                    : attachment;
            })
        );

        return TypedResults.Ok(new Response
        {
            SubmissionsAggregate = formSubmissionsAggregate,
            Notes = notes,
            Attachments = attachments.ToList(),
            SubmissionsFilter = new SubmissionsFilterModel
            {
                HasAttachments = req.HasAttachments,
                HasNotes = req.HasNotes,
                Level1Filter = req.Level1Filter,
                Level2Filter = req.Level2Filter,
                Level3Filter = req.Level3Filter,
                Level4Filter = req.Level4Filter,
                Level5Filter = req.Level5Filter,
                QuestionsAnswered = req.QuestionsAnswered,
                TagsFilter = req.TagsFilter,
                FollowUpStatus = req.FollowUpStatus,
                HasFlaggedAnswers = req.HasFlaggedAnswers,
                MonitoringObserverStatus = req.MonitoringObserverStatus,
                PollingStationNumberFilter = req.PollingStationNumberFilter,
                DataSource = req.DataSource!,
                CoalitionMemberId = req.CoalitionMemberId,
            }
        });
    }

    private async Task<Results<Ok<Response>, NotFound>> AggregatePSIFormSubmissionsAsync(
        PollingStationInformationForm form,
        FormSubmissionsAggregateFilter req,
        CancellationToken ct)
    {
        var tags = req.TagsFilter ?? [];

        var submissions = await context.PollingStationInformation
            .Include(x => x.MonitoringObserver)
            .ThenInclude(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.MonitoringNgo.NgoId == req.NgoId)
            .Where(x => string.IsNullOrWhiteSpace(req.Level1Filter) ||
                        EF.Functions.ILike(x.PollingStation.Level1, req.Level1Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.Level2Filter) ||
                        EF.Functions.ILike(x.PollingStation.Level2, req.Level2Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.Level3Filter) ||
                        EF.Functions.ILike(x.PollingStation.Level3, req.Level3Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.Level4Filter) ||
                        EF.Functions.ILike(x.PollingStation.Level4, req.Level4Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.Level5Filter) ||
                        EF.Functions.ILike(x.PollingStation.Level5, req.Level5Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.PollingStationNumberFilter) ||
                        EF.Functions.ILike(x.PollingStation.Number, req.PollingStationNumberFilter))
            .Where(x => req.HasFlaggedAnswers == null || (req.HasFlaggedAnswers.Value
                ? x.NumberOfFlaggedAnswers > 0
                : x.NumberOfFlaggedAnswers == 0))
            .Where(x => req.FollowUpStatus == null || x.FollowUpStatus == req.FollowUpStatus)
            .Where(x => tags.Length == 0 || x.MonitoringObserver.Tags.Any(tag => tags.Contains(tag)))
            .Where(x => req.MonitoringObserverStatus == null ||
                        x.MonitoringObserver.Status == req.MonitoringObserverStatus)
            .Where(x => req.QuestionsAnswered == null
                        || (req.QuestionsAnswered == QuestionsAnsweredFilter.All &&
                            x.NumberOfQuestionsAnswered == x.PollingStationInformationForm.NumberOfQuestions)
                        || (req.QuestionsAnswered == QuestionsAnsweredFilter.Some &&
                            x.NumberOfQuestionsAnswered < x.PollingStationInformationForm.NumberOfQuestions)
                        || (req.QuestionsAnswered == QuestionsAnsweredFilter.None && x.NumberOfQuestionsAnswered == 0))
            .Where(x => req.HasNotes == null || !req.HasNotes.Value)
            .Where(x => req.HasAttachments == null || !req.HasAttachments.Value)
            .AsNoTracking()
            .AsSplitQuery()
            .ToListAsync(ct);

        var formSubmissionsAggregate = new FormSubmissionsAggregate(form);
        foreach (var formSubmission in submissions)
        {
            formSubmissionsAggregate.AggregateAnswers(formSubmission);
        }

        return TypedResults.Ok(new Response
        {
            SubmissionsAggregate = formSubmissionsAggregate, Notes = [], Attachments = []
        });
    }
}
