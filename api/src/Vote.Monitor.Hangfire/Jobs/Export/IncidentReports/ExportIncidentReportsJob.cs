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
using Vote.Monitor.Hangfire.Jobs.Export.IncidentReports.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.IncidentReports;

public class ExportIncidentReportsJob(
    VoteMonitorContext context,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFileStorageService fileStorageService,
    ILogger<ExportIncidentReportsJob> logger,
    ITimeProvider timeProvider) : IExportIncidentReportsJob
{
    public async Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct)
    {
        var exportedData = await context
            .ExportedData
            .Where(x => x.ElectionRoundId == electionRoundId && x.Id == exportedDataId)
            .FirstOrDefaultAsync(ct);

        if (exportedData == null)
        {
            logger.LogWarning("ExportData was not found for {electionRoundId}  {exportedDataId}",
                electionRoundId, exportedDataId);
            throw new ExportedDataWasNotFoundException(ExportedDataType.IncidentReports, electionRoundId,
                exportedDataId);
        }

        try
        {
            if (exportedData.ExportStatus == ExportedDataStatus.Completed ||
                exportedData.ExportStatus == ExportedDataStatus.Failed)
            {
                logger.LogWarning("ExportData was completed or failed for {electionRoundId} {ngoId} {exportedDataId}",
                    electionRoundId, ngoId, exportedDataId);
                return;
            }

            var utcNow = timeProvider.UtcNow;

            var incidentReportingForm = await context
                .Forms
                .Where(x => x.ElectionRoundId == electionRoundId
                            && x.MonitoringNgo.NgoId == ngoId
                            && x.Status == FormStatus.Published
                            && x.FormType == FormType.IncidentReporting)
                .AsNoTracking()
                .ToListAsync(ct);

            var incidentReports =
                await GetIncidentReports(electionRoundId, ngoId, exportedData.IncidentReportsFilters, ct);

            foreach (var attachment in incidentReports.SelectMany(
                         incidentReportModel => incidentReportModel.Attachments))
            {
                var result =
                    await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);

                if (result is GetPresignedUrlResult.Ok okResult)
                {
                    attachment.PresignedUrl = okResult.Url;
                }
            }

            var excelFileGenerator = ExcelFileGenerator.New();

            for (var index = 0; index < incidentReportingForm.Count; index++)
            {
                var form = incidentReportingForm[index];
                var sheetData = IncidentReportsDataTable
                    .FromForm(form)
                    .WithData()
                    .For(incidentReports)
                    .Please();

                excelFileGenerator.WithSheet((index + 1) + "-" + form.Code, sheetData.header, sheetData.dataTable);
            }

            var base64EncodedData = excelFileGenerator.Please();
            var fileName = $"incident-reports-{utcNow:yyyyMMdd_HHmmss}.xlsx";
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

    private async Task<IncidentReportModel[]> GetIncidentReports(Guid electionRoundId, Guid ngoId,
        ExportIncidentReportsFilters? filters, CancellationToken ct)
    {
        var sql = """
                  WITH
                      INCIDENT_REPORTS AS (
                          SELECT
                              IR."Id" AS "IncidentReportId",
                              F."Id" as "FormId",
                              F."Code" AS "FormCode",
                              F."Name" AS "FormName",
                              F."DefaultLanguage" "FormDefaultLanguage",
                              IR."LocationType",
                              IR."LocationDescription",
                              IR."PollingStationId",
                              IR."MonitoringObserverId",
                              IR."NumberOfQuestionsAnswered",
                              IR."NumberOfFlaggedAnswers",
                              IR."Answers",
                              IR."IsCompleted",
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
                                       "IncidentReportAttachments" A
                                   WHERE
                                       A."ElectionRoundId" = @electionRoundId
                                     AND A."FormId" = IR."FormId"
                                     AND A."IncidentReportId" = IR."Id"
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
                                       "IncidentReportNotes" N
                                   WHERE
                                       N."ElectionRoundId" = @electionRoundId
                                     AND N."FormId" = IR."FormId"
                                     AND N."IncidentReportId" = IR."Id"
                               ),
                               '[]'::JSONB
                               ) AS "Notes",
                              COALESCE(IR."LastModifiedOn", IR."CreatedOn") AS "TimeSubmitted",
                              IR."FollowUpStatus"
                          FROM
                              "IncidentReports" IR
                                  INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                                  INNER JOIN "MonitoringObservers" MO ON IR."MonitoringObserverId" = MO."Id"
                                  INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                          WHERE
                            MN."ElectionRoundId" = @electionRoundId
                            AND MN."NgoId" = @ngoId
                            AND (@monitoringObserverStatus IS NULL OR MO."Status" = @monitoringObserverStatus)
                            AND (@formId IS NULL OR IR."FormId" = @formId)
                            AND (@locationType IS NULL OR IR."LocationType" = @locationType)
                            AND (@questionsAnswered IS NULL
                                OR (@questionsAnswered = 'All' AND F."NumberOfQuestions" = IR."NumberOfQuestionsAnswered")
                                OR (@questionsAnswered = 'Some' AND F."NumberOfQuestions" <> IR."NumberOfQuestionsAnswered")
                                OR (@questionsAnswered = 'None' AND IR."NumberOfQuestionsAnswered" = 0)
                            )
                            AND (@fromDate is NULL OR COALESCE(IR."LastModifiedOn", IR."CreatedOn") >= @fromDate::timestamp)
                            AND (@toDate is NULL OR COALESCE(IR."LastModifiedOn", IR."CreatedOn") <= @toDate::timestamp)
                            AND (@isCompleted is NULL OR IR."IsCompleted" = @isCompleted)
                      )
                  SELECT
                      IR."IncidentReportId",
                      IR."TimeSubmitted",
                      IR."FormId",
                      IR."FormCode",
                      IR."FormName",
                      IR."FormDefaultLanguage",
                      IR."LocationType",
                      IR."LocationDescription",
                      PS."Id" AS "PollingStationId",
                      PS."Level1",
                      PS."Level2",
                      PS."Level3",
                      PS."Level4",
                      PS."Level5",
                      PS."Number",
                      IR."MonitoringObserverId",
                      U."FirstName",
                      U."LastName",
                      U."Email",
                      U."PhoneNumber",
                      IR."Answers",
                      IR."Attachments",
                      IR."Notes",
                      IR."FollowUpStatus",
                      IR."NumberOfFlaggedAnswers",
                      IR."IsCompleted"
                  FROM
                      INCIDENT_REPORTS IR
                          INNER JOIN "MonitoringObservers" MO ON MO."Id" = IR."MonitoringObserverId"
                          INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                          INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                          INNER JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                          LEFT JOIN "PollingStations" PS ON PS."Id" = IR."PollingStationId"
                  WHERE
                      MN."ElectionRoundId" = @electionRoundId
                    AND MN."NgoId" = @ngoId
                    AND (
                      @searchText IS NULL
                          OR @searchText = ''
                          OR U."FirstName" ILIKE @searchText
                          OR U."LastName" ILIKE @searchText
                          OR U."Email" ILIKE @searchText
                          OR U."PhoneNumber" ILIKE @searchText
                      )
                    AND (@level1 IS NULL OR PS."Level1" = @level1)
                    AND (@level2 IS NULL OR PS."Level2" = @level2)
                    AND (@level3 IS NULL OR PS."Level3" = @level3)
                    AND (@level4 IS NULL OR PS."Level4" = @level4)
                    AND (@level5 IS NULL OR PS."Level5" = @level5)
                    AND (@pollingStationNumber IS NULL OR PS."Number" = @pollingStationNumber)
                    AND (@hasFlaggedAnswers IS NULL
                      OR (IR."NumberOfFlaggedAnswers" = 0 AND @hasFlaggedAnswers = FALSE)
                      OR (IR."NumberOfFlaggedAnswers" > 0 AND @hasFlaggedAnswers = TRUE)
                      )
                    AND (@followUpStatus IS NULL OR IR."FollowUpStatus" = @followUpStatus)
                    AND (@tagsFilter IS NULL OR CARDINALITY(@tagsFilter) = 0 OR MO."Tags" && @tagsFilter)
                    AND (@hasNotes IS NULL
                      OR (jsonb_array_length(IR."Notes") = 0 AND @hasNotes = FALSE)
                      OR (jsonb_array_length(IR."Notes") > 0 AND @hasNotes = TRUE)
                      )
                    AND (@hasAttachments IS NULL
                      OR (jsonb_array_length(IR."Attachments") = 0 AND @hasAttachments = FALSE)
                      OR (jsonb_array_length(IR."Attachments") > 0 AND @hasAttachments = TRUE)
                      )
                  ORDER BY IR."TimeSubmitted" DESC;
                  """;

        var queryParams = new
        {
            electionRoundId,
            ngoId,
            searchText = $"%{filters?.SearchText?.Trim() ?? string.Empty}%",
            LocationType = filters?.LocationType?.ToString(),
            level1 = filters?.Level1Filter,
            level2 = filters?.Level2Filter,
            level3 = filters?.Level3Filter,
            level4 = filters?.Level4Filter,
            level5 = filters?.Level5Filter,
            pollingStationNumber = filters?.PollingStationNumberFilter,
            hasFlaggedAnswers = filters?.HasFlaggedAnswers,
            followUpStatus = filters?.FollowUpStatus?.ToString(),
            tagsFilter = filters?.TagsFilter ?? [],
            monitoringObserverStatus = filters?.MonitoringObserverStatus?.ToString(),
            formId = filters?.FormId,
            hasNotes = filters?.HasNotes,
            hasAttachments = filters?.HasAttachments,
            questionsAnswered = filters?.QuestionsAnswered?.ToString(),
            fromDate = filters?.FromDateFilter?.ToString("O"),
            toDate = filters?.ToDateFilter?.ToString("O"),
            isCompleted = filters?.IsCompletedFilter
        };

        IEnumerable<IncidentReportModel> incidentReports;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            incidentReports = await dbConnection.QueryAsync<IncidentReportModel>(sql, queryParams);
        }

        var incidentReportsData = incidentReports.ToArray();
        return incidentReportsData;
    }
}