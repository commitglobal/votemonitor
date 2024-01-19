using Vote.Monitor.Api.Feature.CSO;
using CreateEndpoint = Vote.Monitor.Api.Feature.CSO.Create.Endpoint;
using CreateRequest = Vote.Monitor.Api.Feature.CSO.Create.Request;
using DeactivateEndpoint = Vote.Monitor.Api.Feature.CSO.Deactivate.Endpoint;
using DeactivateRequest = Vote.Monitor.Api.Feature.CSO.Deactivate.Request;

namespace Vote.Monitor.Api.IntegrationTests.CSO;

public static class CsoTestData
{
    private static bool _isGeneratingData = false;
    private static bool _isTestDataGenerated = false;
    private static readonly object _lock = new();

    public static async Task GenerateCSOTestData(this HttpClient client, ITestOutputHelper outputHelper)
    {
        outputHelper.WriteLine("Generating CSO Test Data");

        while (_isGeneratingData)
        {
            await Task.Delay(1000);
        }

        if (_isTestDataGenerated)
        {
            return;
        }

        lock (_lock)
        {

            if (_isTestDataGenerated)
            {
                return;
            }
            _isGeneratingData = true;
            _isTestDataGenerated = true;
        }

        for (int i = 0; i < 20; i++)
        {

            await client.POSTAsync<CreateEndpoint, CreateRequest, CSOModel>(new CreateRequest
            {
                Name = $"Activated{i}"
            });
            var (_, createResult) = await client.POSTAsync<CreateEndpoint, CreateRequest, CSOModel>(new CreateRequest
            {
                Name = $"Deactivated{i}"
            });

            var deactivateRequest = new DeactivateRequest
            {
                Id = createResult.Id
            };
            await client.POSTAsync<DeactivateEndpoint, DeactivateRequest, CSOModel>(deactivateRequest);


        }
        _isGeneratingData = false;
    }
}
