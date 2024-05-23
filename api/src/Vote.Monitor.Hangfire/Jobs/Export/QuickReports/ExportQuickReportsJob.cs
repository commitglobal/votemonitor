using Dapper;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.FileGenerators;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Hangfire;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions;
using Vote.Monitor.Hangfire.Jobs.Export.QuickReports.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.QuickReports;

public class ExportQuickReportsJob(VoteMonitorContext context,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFileStorageService fileStorageService,
    ILogger<ExportFormSubmissionsJob> logger,
    ITimeProvider timeProvider) : IExportQuickReportsJob
{
    public async Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct)
    {
        var exportedData = await context
            .ExportedData
            .Where(x => x.ElectionRoundId == electionRoundId && x.NgoId == ngoId && x.Id == exportedDataId)
            .FirstOrDefaultAsync(ct);

        if (exportedData == null)
        {
            logger.LogWarning("ExportData was not found for {exportDataType} {electionRoundId} {ngoId} {exportedDataId}",
                ExportedDataType.QuickReports, electionRoundId, ngoId, exportedDataId);
            throw new ExportedDataWasNotFoundException(ExportedDataType.QuickReports, electionRoundId, ngoId, exportedDataId);
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

            var quickReports = await GetQuickReports(electionRoundId, ngoId, ct);

            foreach (var submission in quickReports)
            {
                foreach (var attachment in submission.Attachments)
                {
                    var result = await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName, ct);

                    if (result is GetPresignedUrlResult.Ok okResult)
                    {
                        attachment.PresignedUrl = okResult.Url;
                    }
                }
            }

            var excelFileGenerator = ExcelFileGenerator.New();

            var sheetData = QuickReportsDataTable
                .New()
                .WithData()
                .ForQuickReports(quickReports)
                .Please();

            excelFileGenerator.WithSheet("quick-reports", sheetData.header, sheetData.dataTable);

            var base64EncodedData = excelFileGenerator.Please();
            var fileName = $"quick-reports-{utcNow:yyyyMMdd_HHmmss}.xlsx";
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

    private async Task<List<QuickReportModel>> GetQuickReports(Guid electionRoundId, Guid ngoId, CancellationToken ct)
    {
        var sql = @"
        SELECT
            QR.""Id"",
            QR.""QuickReportLocationType"",
            QR.""MonitoringObserverId"",
            COALESCE(QR.""LastModifiedOn"", QR.""CreatedOn"") AS ""Timestamp"",
            QR.""Title"",
            QR.""Description"",
            QR.""FollowUpStatus"",
            COALESCE((select jsonb_agg(jsonb_build_object('QuickReportId', ""QuickReportId"", 'FileName', ""FileName"", 'MimeType', ""MimeType"", 'FilePath', ""FilePath"", 'UploadedFileName', ""UploadedFileName"", 'TimeSubmitted', COALESCE(""LastModifiedOn"", ""CreatedOn"")))
            FROM ""QuickReportAttachments"" qra
            WHERE qra.""ElectionRoundId"" = @electionRoundId AND qra.""QuickReportId"" = qr.""Id""),'[]'::JSONB) AS ""Attachments"",
            O.""FirstName"",
            O.""LastName"",
            O.""Email"",
            O.""PhoneNumber"",
            QR.""PollingStationDetails"",
            PS.""Id"" AS ""PollingStationId"",
            PS.""Level1"",
            PS.""Level2"",
            PS.""Level3"",
            PS.""Level4"",
            PS.""Level5"",
            PS.""Number"",
            PS.""Address""
        FROM
            ""QuickReports"" QR
            INNER JOIN ""MonitoringObservers"" MO ON MO.""Id"" = QR.""MonitoringObserverId""
            INNER JOIN ""MonitoringNgos"" MN ON MN.""Id"" = MO.""MonitoringNgoId""
            INNER JOIN ""AspNetUsers"" O ON MO.""ObserverId"" = O.""Id""
            LEFT JOIN ""PollingStations"" PS ON PS.""Id"" = QR.""PollingStationId""
        WHERE
            QR.""ElectionRoundId"" = @electionRoundId
            AND MN.""NgoId"" = @ngoId
        ORDER BY
            COALESCE(QR.""LastModifiedOn"", QR.""CreatedOn"") DESC";

        var queryArgs = new
        {
            electionRoundId,
            ngoId,
        };

        IEnumerable<QuickReportModel> quickReports = [];
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            quickReports = await dbConnection.QueryAsync<QuickReportModel>(sql, queryArgs);
        }
        var quickReportsData = quickReports.ToList();
        return quickReportsData;
    }
}
