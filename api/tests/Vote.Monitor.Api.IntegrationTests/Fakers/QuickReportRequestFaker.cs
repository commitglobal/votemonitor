using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.TestUtils.Utils;

namespace Vote.Monitor.Api.IntegrationTests.Fakers;

public sealed class QuickReportRequestFaker: Faker<QuickReportRequest>
{
    public QuickReportRequestFaker(Guid pollingStationId)
    {
        RuleFor(x => x.PollingStationId, pollingStationId);
        RuleFor(x => x.Id, f => f.Random.Guid());
        RuleFor(x => x.Title, f => f.Lorem.Sentence(20).OfLength(1000));
        RuleFor(x => x.Description, f => f.Lorem.Sentence(100).OfLength(10000));
    }
}
