using FastEndpoints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.ImportPollingStations;
using Vote.Monitor.Feature.PollingStation.Repositories;
using Xunit;

namespace Vote.Monitor.Feature.PollingStation.UnitTests;
public class ImportPollingStationTests
{
    [Fact]
    public async Task HandleAsync_SuccessfulImport_ReturnsImportedCount()
    {
        // Arrange
        var repository = new Mock<IPollingStationRepository>();
        var logger = new Mock<ILogger<ImportPollingStationsEndpoint>>();
        var configuration = new Mock<IConfiguration>();
        var tempCsvPath = Path.Combine(Path.GetTempPath(), "test.csv");
        configuration.Setup(c => c.GetSection("CSVFileToImport")["path"])
                     .Returns(tempCsvPath);
        var csvData = "DisplayOrder,Address,Tag1,Tag2\n" +
                      "1,Address1,TagA,TagB\n" +
                      "2,Address2,TagC,TagD\n" +
                      "3,Address3,TagE,TagF";

        File.WriteAllText(tempCsvPath, csvData);

        configuration.Setup(c => c.GetSection("CSVFileToImport")["path"]).Returns(tempCsvPath);

        var endpoint = new ImportPollingStationsEndpoint(repository.Object, logger.Object, configuration.Object);
        var cancellationToken = new CancellationToken();

        // Act
        var importResult = await endpoint.HandleAsync(cancellationToken);

        // Assert
        repository.Verify(r => r.DeleteAllAsync(), Times.Once);
        Assert.Equal(3, importResult);
    }

    [Fact]
    public async Task HandleAsync_ImportFailure_ThrowError()
    {
        // Arrange
        var repository = new Mock<IPollingStationRepository>();
        var logger = new Mock<ILogger<ImportPollingStationsEndpoint>>();
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c.GetSection("CSVFileToImport")["path"])
                     .Returns("test.csv");

        var endpoint = new ImportPollingStationsEndpoint(repository.Object, logger.Object, configuration.Object);
        var cancellationToken = new CancellationToken();

        // Act
        await Assert.ThrowsAsync<ValidationFailureException>(() => endpoint.HandleAsync(cancellationToken));

        // Assert
        repository.Verify(r => r.DeleteAllAsync(), Times.Never);
        repository.Verify(r => r.AddAsync(It.IsAny<PollingStationModel>()), Times.Exactly(0));

        Assert.Contains("CSV file not found at path: test.csv", endpoint.ValidationFailures[0].ToString());
    }
}
