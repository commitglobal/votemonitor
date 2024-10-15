using Dapper;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.FileGenerators;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Hangfire;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;
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
            .Where(x => x.ElectionRoundId == electionRoundId && x.Id == exportedDataId)
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

            var publishedForms = await context
                .Forms
                .Where(x => x.ElectionRoundId == electionRoundId
                            && x.MonitoringNgo.NgoId == ngoId
                            && x.Status == FormStatus.Published)
                .OrderBy(x => x.CreatedOn)
                .AsNoTracking()
                .ToListAsync(ct);

            var submissions = await GetSubmissions(electionRoundId, ngoId, ct);

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

    private async Task<List<SubmissionModel>> GetSubmissions(Guid electionRoundId, Guid ngoId, CancellationToken ct)
    {
        var sql = """
                  WITH submissions AS
                           (SELECT psi."Id" AS "SubmissionId",
                                   (select "Id" from "PollingStationInformationForms" where "ElectionRoundId" = @electionRoundId) AS "FormId",
                                   psi."PollingStationId",
                                   psi."MonitoringObserverId",
                                   psi."Answers",
                                   psi."FollowUpStatus",
                                   (select "Questions"
                                    from "PollingStationInformationForms"
                                    where "ElectionRoundId" = @electionRoundId) AS "Questions",
                                   '[]'::jsonb AS "Attachments",
                                   '[]'::jsonb AS "Notes",
                                   COALESCE(psi."LastModifiedOn", psi."CreatedOn") "TimeSubmitted"
                            FROM "PollingStationInformation" psi
                                     INNER JOIN "MonitoringObservers" mo ON mo."Id" = psi."MonitoringObserverId"
                                     INNER JOIN "MonitoringNgos" mn ON mn."Id" = mo."MonitoringNgoId"
                            WHERE mn."ElectionRoundId" = @electionRoundId
                              AND mn."NgoId" = @ngoId
                            UNION ALL
                            SELECT fs."Id" AS "SubmissionId",
                                   f."Id" AS "FormId",
                                   fs."PollingStationId",
                                   fs."MonitoringObserverId",
                                   fs."Answers",
                                   fs."FollowUpStatus",
                                   f."Questions",
                  
                                   COALESCE((select jsonb_agg(jsonb_build_object('QuestionId', "QuestionId", 'FilePath', "FilePath", 'UploadedFileName', "UploadedFileName"))
                                             FROM "Attachments" a
                                             WHERE a."ElectionRoundId" = @electionRoundId
                                               AND a."FormId" = fs."FormId"
                                               AND a."MonitoringObserverId" = fs."MonitoringObserverId"
                                               AND fs."PollingStationId" = a."PollingStationId"), '[]'::JSONB) AS "Attachments",
                  
                                   COALESCE((select jsonb_agg(jsonb_build_object('QuestionId', "QuestionId", 'Text', "Text"))
                                             FROM "Notes" n
                                             WHERE n."ElectionRoundId" = @electionRoundId
                                               AND n."FormId" = fs."FormId"
                                               AND n."MonitoringObserverId" = fs."MonitoringObserverId"
                                               AND fs."PollingStationId" = n."PollingStationId"), '[]'::JSONB) AS "Notes",
                  
                                   COALESCE(fs."LastModifiedOn", fs."CreatedOn") "TimeSubmitted"
                  
                            FROM "FormSubmissions" fs
                                     INNER JOIN "MonitoringObservers" mo ON fs."MonitoringObserverId" = mo."Id"
                                     INNER JOIN "MonitoringNgos" mn ON mn."Id" = mo."MonitoringNgoId"
                                     INNER JOIN "Forms" f on f."Id" = fs."FormId"
                            WHERE mn."ElectionRoundId" = @electionRoundId
                              AND mn."NgoId" = @ngoId
                            order by "TimeSubmitted" desc)
                  SELECT s."SubmissionId",
                         s."FormId",
                         s."TimeSubmitted",
                         ps."Level1",
                         ps."Level2",
                         ps."Level3",
                         ps."Level4",
                         ps."Level5",
                         ps."Number",
                         s."MonitoringObserverId",
                         u."FirstName",
                         u."LastName",
                         u."Email",
                         u."PhoneNumber",
                         mo."Tags",
                         s."Attachments",
                         s."Notes",
                         s."Answers",
                         s."Questions",
                         s."FollowUpStatus"
                  FROM submissions s
                           INNER JOIN "PollingStations" ps on ps."Id" = s."PollingStationId"
                           INNER JOIN "MonitoringObservers" mo on mo."Id" = s."MonitoringObserverId"
                           INNER JOIN "MonitoringNgos" mn ON mn."Id" = mo."MonitoringNgoId"
                           INNER JOIN "Observers" o ON o."Id" = mo."ObserverId"
                           INNER JOIN "AspNetUsers" u ON u."Id" = o."ApplicationUserId"
                  WHERE mn."ElectionRoundId" = @electionRoundId
                    AND mn."NgoId" = @ngoId
                  """;

        var queryParams = new { electionRoundId, ngoId };

        IEnumerable<SubmissionModel> submissions = [];
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            submissions = await dbConnection.QueryAsync<SubmissionModel>(sql, queryParams);
        }

        var submissionsData = submissions.ToList();
        return submissionsData;
    }
}