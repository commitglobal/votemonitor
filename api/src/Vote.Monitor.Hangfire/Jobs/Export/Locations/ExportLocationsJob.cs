using Dapper;
using Job.Contracts.Jobs;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.FileGenerators;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Hangfire.Jobs.Export.Locations.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.Locations;

public class ExportLocationsJob(
    VoteMonitorContext context,
    INpgsqlConnectionFactory dbConnectionFactory,
    ILogger<ExportLocationsJob> logger,
    ITimeProvider timeProvider) : IExportLocationsJob
{
    public async Task Run(Guid electionRoundId, Guid exportedDataId, CancellationToken ct)
    {
        var exportedData = await context
            .ExportedData
            .Where(x => x.Id == exportedDataId)
            .FirstOrDefaultAsync(ct);

        if (exportedData == null)
        {
            logger.LogWarning("ExportData was not found for {exportDataType} {electionRoundId} {exportedDataId}",
                ExportedDataType.Locations, electionRoundId, exportedDataId);
            throw new ExportedDataWasNotFoundException(ExportedDataType.Locations, electionRoundId, exportedDataId);
        }

        try
        {
            if (exportedData.ExportStatus == ExportedDataStatus.Completed)
            {
                logger.LogWarning("ExportData was completed for {electionRoundId} {exportedDataId}",
                    electionRoundId, exportedDataId);
                return;
            }

            var utcNow = timeProvider.UtcNow;

            var locations = await GetLocationsAsync(electionRoundId, ct);

            var excelFileGenerator = ExcelFileGenerator.New();

            var sheetData = LocationsDataTable
                .New()
                .WithData()
                .For(locations)
                .Please();

            excelFileGenerator.WithSheet("locations", sheetData.header, sheetData.dataTable);

            var base64EncodedData = excelFileGenerator.Please();
            var fileName = $"locations-{utcNow:yyyyMMdd_HHmmss}.xlsx";
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

    private async Task<LocationModel[]> GetLocationsAsync(Guid electionRoundId, CancellationToken ct)
    {
        var sql = """
                  SELECT
                      "Id",
                      "Level1",
                      "Level2",
                      "Level3",
                      "Level4",
                      "Level5",
                      "DisplayOrder",
                      "Tags"
                  FROM
                      "Locations"
                  WHERE
                      "ElectionRoundId" = @electionRoundId
                  ORDER BY
                      "DisplayOrder" ASC;
                  """;

        var queryArgs = new { electionRoundId, };

        IEnumerable<LocationModel> locations;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            locations = await dbConnection.QueryAsync<LocationModel>(sql, queryArgs);
        }

        var locationsData = locations.ToArray();
        return locationsData;
    }
}
