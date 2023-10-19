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
    //[Fact]
    //public async Task HandleAsync_SuccessfulImport_ReturnsImportedCount()
    //{
    //    // Arrange
    //    var repository = new Mock<IPollingStationRepository>();
    //    var logger = new Mock<ILogger<ImportPollingStationsEndpoint>>();
    //    var configuration = new Mock<IConfiguration>();
    //    configuration.Setup(c => c.GetSection("CSVFileToImport")["path"])
    //                 .Returns("test.csv");

    //    var endpoint = new ImportPollingStationsEndpoint(repository.Object, logger.Object, configuration.Object);
    //    var cancellationToken = new CancellationToken();

    //    // Act
    //    await endpoint.HandleAsync(cancellationToken);

    //    // Assert
    //    repository.Verify(r => r.DeleteAllAsync(), Times.Once);
    //    repository.Verify(r => r.AddAsync(It.IsAny<PollingStationModel>()), Times.Exactly(5));
    //    logger.VerifyNoOtherCalls();

    //    // Assert.Equal(5, importResult); 
    //    // Assert.Equal(0, importResult); 
    //}

    //[Fact]
    //public async Task HandleAsync_ImportFailure_LogsErrorAndReturnsZero()
    //{
    //    // Arrange
    //    var repository = new Mock<IPollingStationRepository>();

    //    var logger = new Mock<ILogger<ImportPollingStationsEndpoint>>();
    //    var configuration = new Mock<IConfiguration>();
    //    configuration.Setup(c => c.GetSection("CSVFileToImport")["path"])
    //                 .Returns("test.csv");

    //    var endpoint = new ImportPollingStationsEndpoint(repository.Object, logger.Object, configuration.Object);
    //    var cancellationToken = new CancellationToken();

    //    // Act
    //    await endpoint.HandleAsync(cancellationToken);

    //    // Assert
    //    repository.Verify(r => r.DeleteAllAsync(), Times.Never);
    //    logger.Verify(l => l.LogError($"CSV file not found at path: test.csv"), Times.Once);
    //    Assert.Contains("CSV file not found.", endpoint.Env.ToString());
    //}
}
