using Microsoft.AspNetCore.Http;
using ImportEndpoint = Vote.Monitor.Api.Feature.PollingStation.Import.Endpoint;
using ImportRequest = Vote.Monitor.Api.Feature.PollingStation.Import.Request;
using ImportResponse = Vote.Monitor.Api.Feature.PollingStation.Import.Response;

namespace Vote.Monitor.Api.IntegrationTests.PollingStation;

static class ClientHelpers
{
    public static async Task<TestResult<ImportResponse>> ImportPollingStations(this HttpClient client, string file = "test-data.csv")
    {
        using var testDataStream = File.OpenRead(Path.Combine("PollingStation", file));
        var importRequest = new ImportRequest
        {
            ElectionRoundId = Guid.NewGuid(),
            File = new FormFile(testDataStream, 0, testDataStream.Length, file, file)
        };
        return await client.POSTAsync<ImportEndpoint, ImportRequest, ImportResponse>(importRequest, true);

    }
}
