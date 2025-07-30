using Dapper;
using Job.Contracts.Jobs;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.FileGenerators;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions;

public class ExportFormSubmissionsJob(
    VoteMonitorContext context,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFileStorageService fileStorageService,
    ILogger<ExportFormSubmissionsJob> logger,
    ITimeProvider timeProvider) : IExportFormSubmissionsJob
{
    public async Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct)
    {
        var exportedData = await context
            .ExportedData
            .Where(x => x.Id == exportedDataId)
            .FirstOrDefaultAsync(ct);

        if (exportedData == null)
        {
            logger.LogWarning("ExportData was not found for {electionRoundId}  {exportedDataId}",
                electionRoundId, exportedDataId);
            throw new ExportedDataWasNotFoundException(ExportedDataType.FormSubmissions, electionRoundId,
                exportedDataId);
        }

        try
        {
            if (exportedData.ExportStatus == ExportedDataStatus.Completed)
            {
                logger.LogWarning("ExportData was completed for {electionRoundId} {ngoId} {exportedDataId}",
                    electionRoundId, ngoId, exportedDataId);
                return;
            }

            var utcNow = timeProvider.UtcNow;

            var psiForm = await context
                .PollingStationInformationForms
                .Where(x => x.ElectionRoundId == electionRoundId)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);
            var filters = exportedData.FormSubmissionsFilters ?? new ExportFormSubmissionsFilters();

            var publishedForms = await context
                .Forms
                .FromSqlInterpolated(
                    @$"SELECT f.* FROM ""Forms"" f 
                       INNER JOIN ""GetAvailableForms""({electionRoundId}, {ngoId}, {filters.DataSource.ToString()}) af on af.""FormId"" = f.""Id""")
                .Where(x => x.Status == FormStatus.Published)
                .Where(x => x.FormType != FormType.CitizenReporting && x.FormType != FormType.IncidentReporting)
                .OrderBy(x => x.DisplayOrder)
                .AsNoTracking()
                .ToListAsync(ct);

            var submissions = await GetSubmissions(electionRoundId, ngoId, filters, ct);

            foreach (var submission in submissions)
            {
                foreach (var attachment in submission.Attachments)
                {
                    var result =
                        await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);

                    if (result is GetPresignedUrlResult.Ok okResult)
                    {
                        attachment.PresignedUrl = okResult.Url;
                    }
                }
            }

            var excelFileGenerator = ExcelFileGenerator.New();
            if (psiForm != null)
            {
                var psiDataTable = FormSubmissionsDataTable
                    .FromForm(psiForm)
                    .WithData()
                    .ForSubmissions(submissions)
                    .Please();

                excelFileGenerator.WithSheet("PSI", psiDataTable.header, psiDataTable.dataTable);
            }

            for (var index = 0; index < publishedForms.Count; index++)
            {
                var form = publishedForms[index];
                var sheetData = FormSubmissionsDataTable
                    .FromForm(form)
                    .WithData()
                    .ForSubmissions(submissions)
                    .Please();

                excelFileGenerator.WithSheet((index + 1) + "-" + form.Code, sheetData.header, sheetData.dataTable);
            }

            var base64EncodedData = excelFileGenerator.Please();
            var fileName = $"form-submissions-{utcNow:yyyyMMdd_HHmmss}.xlsx";
            exportedData.Complete(fileName, base64EncodedData, utcNow);

            await context.SaveChangesAsync(ct);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured when exporting data");
            exportedData.Fail();
            await context.SaveChangesAsync(ct);

            throw;
        }
    }

    private async Task<List<SubmissionModel>> GetSubmissions(Guid electionRoundId, Guid ngoId,
        ExportFormSubmissionsFilters filters, CancellationToken ct)
    {
        var sql =
            """
            WITH
            	SUBMISSIONS AS (
            		SELECT
            			PSI."Id" AS "SubmissionId",
            			PSI."PollingStationInformationFormId" AS "FormId",
            			'PSI' AS "FormType",
            			PSI."PollingStationId",
            			PSI."MonitoringObserverId",
            			PSI."Answers",
            			PSI."FollowUpStatus",
            			PSIF."Questions",
            			PSI."NumberOfQuestionsAnswered",
            			PSI."NumberOfFlaggedAnswers",
            			'[]'::JSONB AS "Attachments",
            			'[]'::JSONB AS "Notes",
            			psi."LastUpdatedAt" "TimeSubmitted",
            			PSI."IsCompleted",
            			MO."NgoName",
            			MO."DisplayName",
            			MO."Email",
            			MO."PhoneNumber",
            			MO."Tags"
            		FROM
            			"PollingStationInformation" PSI
            			INNER JOIN "PollingStationInformationForms" PSIF ON PSIF."Id" = PSI."PollingStationInformationFormId"
            			INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId"
            		WHERE
            			(
            				@COALITIONMEMBERID IS NULL
            				OR MO."NgoId" = @COALITIONMEMBERID
            			)
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
            				OR psi."LastUpdatedAt" >= @FROMDATE::TIMESTAMP
            			)
            			AND (
            				@TODATE IS NULL
            				OR psi."LastUpdatedAt" <= @TODATE::TIMESTAMP
            			)
            			AND (
            				@ISCOMPLETED IS NULL
            				OR PSI."IsCompleted" = @ISCOMPLETED
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
            		UNION ALL
            		SELECT
            			FS."Id" AS "SubmissionId",
            			F."Id" AS "FormId",
            			F."FormType",
            			FS."PollingStationId",
            			FS."MonitoringObserverId",
            			FS."Answers",
            			FS."FollowUpStatus",
            			F."Questions",
            			FS."NumberOfQuestionsAnswered",
            			FS."NumberOfFlaggedAnswers",
            			COALESCE(
            				(
            					SELECT
            						JSONB_AGG(
            							JSONB_BUILD_OBJECT(
            								'QuestionId',
            								"QuestionId",
            								'FilePath',
            								"FilePath",
            								'UploadedFileName',
            								"UploadedFileName"
            							)
            						)
            					FROM
            						"Attachments" A
            					WHERE
            						A."ElectionRoundId" = @ELECTIONROUNDID
            						AND A."FormId" = FS."FormId"
            						AND A."MonitoringObserverId" = FS."MonitoringObserverId"
            						AND FS."PollingStationId" = A."PollingStationId"
            				),
            				'[]'::JSONB
            			) AS "Attachments",
            			COALESCE(
            				(
            					SELECT
            						JSONB_AGG(
            							JSONB_BUILD_OBJECT('QuestionId', "QuestionId", 'Text', "Text")
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
            			FS."LastUpdatedAt" "TimeSubmitted",
            			FS."IsCompleted",
            			MO."NgoName",
            			MO."DisplayName",
            			MO."Email",
            			MO."PhoneNumber",
            			MO."Tags"
            		FROM
            			"FormSubmissions" FS
            			INNER JOIN "Forms" F ON F."Id" = FS."FormId"
            			INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId"
            			INNER JOIN "GetAvailableForms" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) AF ON FS."FormId" = AF."FormId"
            		WHERE
            			(
            				@COALITIONMEMBERID IS NULL
            				OR MO."NgoId" = @COALITIONMEMBERID
            			)
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
            				OR FS."LastUpdatedAt" >= @FROMDATE::TIMESTAMP
            			)
            			AND (
            				@TODATE IS NULL
            				OR FS."LastUpdatedAt" <= @TODATE::TIMESTAMP
            			)
            			AND (
            				@ISCOMPLETED IS NULL
            				OR FS."IsCompleted" = @ISCOMPLETED
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
            		ORDER BY
            			"TimeSubmitted" DESC
            	)
            SELECT
            	S."SubmissionId",
            	S."FormId",
            	S."FormType",
            	S."TimeSubmitted",
            	PS."Level1",
            	PS."Level2",
            	PS."Level3",
            	PS."Level4",
            	PS."Level5",
            	PS."Number",
            	S."NgoName",
            	S."MonitoringObserverId",
            	S."DisplayName",
            	S."Email",
            	S."PhoneNumber",
            	S."Tags",
            	S."Attachments",
            	S."Notes",
            	S."Answers",
            	S."Questions",
            	S."FollowUpStatus",
            	S."IsCompleted"
            FROM
            	SUBMISSIONS S
            	INNER JOIN "PollingStations" PS ON PS."Id" = S."PollingStationId"
            WHERE
            	(
            		@SEARCHTEXT IS NULL
            		OR @SEARCHTEXT = ''
            		OR S."DisplayName" ILIKE @SEARCHTEXT
            		OR S."Email" ILIKE @SEARCHTEXT
            		OR S."PhoneNumber" ILIKE @SEARCHTEXT
            		OR S."MonitoringObserverId"::TEXT ILIKE @SEARCHTEXT
            	)
            	AND (
            		@FORMTYPE IS NULL
            		OR S."FormType" = @FORMTYPE
            	)
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
            		OR S."Tags" && @TAGSFILTER
            	)
            	AND (
            		@HASNOTES IS NULL
            		OR (
            			JSONB_ARRAY_LENGTH(S."Notes") = 0
            			AND @HASNOTES = FALSE
            		)
            		OR (
            			JSONB_ARRAY_LENGTH(S."Notes") > 0
            			AND @HASNOTES = TRUE
            		)
            	)
            	AND (
            		@HASATTACHMENTS IS NULL
            		OR (
            			JSONB_ARRAY_LENGTH(S."Attachments") = 0
            			AND @HASATTACHMENTS = FALSE
            		)
            		OR (
            			JSONB_ARRAY_LENGTH(S."Attachments") > 0
            			AND @HASATTACHMENTS = TRUE
            		)
            	)
            """;

        var queryParams = new
        {
            electionRoundId,
            ngoId,
            coalitionMemberId = filters.CoalitionMemberId,
            dataSource = filters.DataSource.ToString(),
            searchText = $"%{filters.SearchText?.Trim() ?? string.Empty}%",
            formType = filters.FormTypeFilter?.ToString(),
            level1 = filters.Level1Filter,
            level2 = filters.Level2Filter,
            level3 = filters.Level3Filter,
            level4 = filters.Level4Filter,
            level5 = filters.Level5Filter,
            pollingStationNumber = filters.PollingStationNumberFilter,
            hasFlaggedAnswers = filters.HasFlaggedAnswers,
            followUpStatus = filters.FollowUpStatus?.ToString(),
            tagsFilter = filters.TagsFilter ?? [],
            monitoringObserverStatus = filters.MonitoringObserverStatus?.ToString(),
            formId = filters.FormId,
            hasNotes = filters.HasNotes,
            hasAttachments = filters.HasAttachments,
            questionsAnswered = filters.QuestionsAnswered?.ToString(),
            fromDate = filters.FromDateFilter?.ToString("O"),
            toDate = filters.ToDateFilter?.ToString("O"),
            isCompleted = filters.IsCompletedFilter
        };

        IEnumerable<SubmissionModel> submissions = [];
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            submissions = await dbConnection.QueryAsync<SubmissionModel>(sql, queryParams);
        }

        var submissionsData = submissions.ToList();
        return submissionsData;
    }
}
