using Microsoft.AspNetCore.Http;

namespace Vote.Monitor.Feature.PollingStation.IntegrationTests;

static class ClientHelpers
{
    public static async Task<TestResult<Import.Response>> ImportPollingStations(this HttpClient client, string file = "test-data.csv")
    {
        using var testDataStream = File.OpenRead(file);
        var importRequest = new Import.Request
        {
            File = new FormFile(testDataStream, 0, testDataStream.Length, file, file),
        };
        return await client.POSTAsync<Import.Endpoint, Import.Request, Import.Response>(importRequest, true);

    }
}
