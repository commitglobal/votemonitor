using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class ObserverGuideAggregateFaker : PrivateFaker<ObserverGuide>
{
    public ObserverGuideAggregateFaker(Guid? id = null,
        string? fileName = null,
        string? filePath = null,
        string? title = null,
        MonitoringNgo? monitoringNgo = null)
    {
        UsePrivateConstructor();

        var fakeMonitoringNgo = new MonitoringNgoAggregateFaker().Generate();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.MonitoringNgoId, fake => monitoringNgo?.Id ?? fakeMonitoringNgo.Id);
        RuleFor(fake => fake.MonitoringNgo, fake => monitoringNgo ?? fakeMonitoringNgo);
        RuleFor(fake => fake.FileName, fileName ?? string.Empty);
        RuleFor(fake => fake.FilePath, filePath ?? string.Empty);
        RuleFor(fake => fake.Title, title ?? string.Empty);
    }
}
