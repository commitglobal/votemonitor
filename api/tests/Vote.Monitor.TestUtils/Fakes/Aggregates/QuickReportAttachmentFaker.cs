using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class QuickReportAttachmentFaker : PrivateFaker<QuickReportAttachment>
{
    public QuickReportAttachmentFaker(Guid? id = null, Guid? quickReportId = null)
    {
        UsePrivateConstructor();

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.QuickReportId, fake => quickReportId ?? fake.Random.Guid());
        RuleFor(fake => fake.FilePath, f => f.System.FilePath());
        RuleFor(fake => fake.FileName, f => f.System.FileName("jpg"));
        RuleFor(fake => fake.UploadedFileName, f => f.System.FileName("jpg"));
        RuleFor(fake => fake.ElectionRound, fakeElectionRound);
        RuleFor(fake => fake.ElectionRoundId, fakeElectionRound.Id);
        RuleFor(fake => fake.MonitoringObserver, fakeMonitoringObserver);
        RuleFor(fake => fake.MonitoringObserverId, fakeMonitoringObserver.Id);
    }
}
