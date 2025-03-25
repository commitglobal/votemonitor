using Dapper;
using Job.Contracts.Jobs;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.FileGenerators;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;
using Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions;
using Vote.Monitor.Hangfire.Jobs.Export.QuickReports.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.QuickReports;

public class ExportQuickReportsJob(
    VoteMonitorContext context,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFileStorageService fileStorageService,
    ILogger<ExportFormSubmissionsJob> logger,
    ITimeProvider timeProvider) : IExportQuickReportsJob
{
    public async Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct)
    {
        var exportedData = await context
            .ExportedData
            .Where(x => x.Id == exportedDataId)
            .FirstOrDefaultAsync(ct);

        if (exportedData == null)
        {
            logger.LogWarning("ExportData was not found for {exportDataType} {electionRoundId} {exportedDataId}",
                ExportedDataType.QuickReports, electionRoundId, exportedDataId);
            throw new ExportedDataWasNotFoundException(ExportedDataType.QuickReports, electionRoundId, exportedDataId);
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

            var quickReports = await GetQuickReports(electionRoundId, ngoId, exportedData.QuickReportsFilters, ct);

            foreach (var submission in quickReports)
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

    private async Task<List<QuickReportModel>> GetQuickReports(Guid electionRoundId, Guid ngoId,
        ExportQuickReportsFilters filters, CancellationToken ct)
    {
        var sql =
            """
            SELECT
            	QR."Id",
            	QR."QuickReportLocationType",
            	QR."IncidentCategory",
            	QR."MonitoringObserverId",
            	COALESCE(QR."LastModifiedOn", QR."CreatedOn") AS "Timestamp",
            	QR."Title",
            	QR."Description",
            	QR."FollowUpStatus",
            	COALESCE(
            		(
            			SELECT
            				JSONB_AGG(
            					JSONB_BUILD_OBJECT(
            						'QuickReportId',
            						"QuickReportId",
            						'FileName',
            						"FileName",
            						'MimeType',
            						"MimeType",
            						'FilePath',
            						"FilePath",
            						'UploadedFileName',
            						"UploadedFileName",
            						'TimeSubmitted',
            						COALESCE("LastModifiedOn", "CreatedOn")
            					)
            				)
            			FROM
            				"QuickReportAttachments" QRA
            			WHERE
            				QRA."ElectionRoundId" = @ELECTIONROUNDID
            				AND QRA."QuickReportId" = QR."Id"
            		),
            		'[]'::JSONB
            	) AS "Attachments",
            	MO."NgoName",
            	MO."DisplayName",
            	MO."Email",
            	MO."PhoneNumber",
            	QR."PollingStationDetails",
            	PS."Id" AS "PollingStationId",
            	PS."Level1",
            	PS."Level2",
            	PS."Level3",
            	PS."Level4",
            	PS."Level5",
            	PS."Number",
            	PS."Address"
            FROM
            	"QuickReports" QR
            	INNER JOIN "GetAvailableMonitoringObservers" (@ELECTIONROUNDID, @NGOID, @DATASOURCE) MO ON MO."MonitoringObserverId" = QR."MonitoringObserverId"
            	LEFT JOIN "PollingStations" PS ON PS."Id" = QR."PollingStationId"
            WHERE
            	(
            		@COALITIONMEMBERID IS NULL
            		OR MO."NgoId" = @COALITIONMEMBERID
            	)
            	AND (
            		@FOLLOWUPSTATUS IS NULL
            		OR QR."FollowUpStatus" = @FOLLOWUPSTATUS
            	)
            	AND (
            		@QUICKREPORTLOCATIONTYPE IS NULL
            		OR QR."QuickReportLocationType" = @QUICKREPORTLOCATIONTYPE
            	)
            	AND (
            		@INCIDENTCATEGORY IS NULL
            		OR QR."IncidentCategory" = @INCIDENTCATEGORY
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
            		@FROMDATE IS NULL
            		OR COALESCE(QR."LastModifiedOn", QR."CreatedOn") >= @FROMDATE::TIMESTAMP
            	)
            	AND (
            		@TODATE IS NULL
            		OR COALESCE(QR."LastModifiedOn", QR."CreatedOn") <= @TODATE::TIMESTAMP
            	)
            ORDER BY
            	COALESCE(QR."LastModifiedOn", QR."CreatedOn") DESC
            """;

        var queryArgs = new
        {
            electionRoundId,
            ngoId,
            dataSource = filters.DataSource.ToString(),
            coalitionMemberId = filters.CoalitionMemberId,
            level1 = filters.Level1Filter,
            level2 = filters.Level2Filter,
            level3 = filters.Level3Filter,
            level4 = filters.Level4Filter,
            level5 = filters.Level5Filter,
            followUpStatus = filters.QuickReportFollowUpStatus?.ToString(),
            quickReportLocationType = filters.QuickReportLocationType?.ToString(),
            incidentCategory = filters.IncidentCategory?.ToString(),
            fromDate = filters.FromDateFilter?.ToString("O"),
            toDate = filters.ToDateFilter?.ToString("O"),
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
