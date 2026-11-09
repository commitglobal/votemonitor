using Vote.Monitor.Domain.Entities.RecordLabelAggregate;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.UnitTests.Entities.RecordLabelAggregate;

public class RecordLabelTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly DateTime Now = new(2026, 11, 4, 9, 0, 0, DateTimeKind.Utc);

    private static readonly ReviewTarget Target =
        new(Guid.NewGuid(), Guid.NewGuid(), ReportRecordType.QuickReport, Guid.NewGuid());

    [Fact]
    public void Create_KeepsTheRecordTheLabelAndWhoAddedIt()
    {
        var label = RecordLabel.Create(Target, "Media", UserId, Now);

        label.Id.Should().NotBeEmpty();
        label.ElectionRoundId.Should().Be(Target.ElectionRoundId);
        label.MonitoringNgoId.Should().Be(Target.MonitoringNgoId);
        label.RecordType.Should().Be(ReportRecordType.QuickReport);
        label.RecordId.Should().Be(Target.RecordId);
        label.Label.Should().Be("Media");
        label.CreatedBy.Should().Be(UserId);
        label.CreatedAt.Should().Be(Now);
    }

    [Fact]
    public void LabelsChanged_RecordsTheAddedAndRemovedLabels()
    {
        var activity = ReviewActivity.LabelsChanged(Target, new ReviewActor(UserId, "Ana"), ["Escalated"], ["Media"], Now);

        activity.Kind.Should().Be(ReviewActivityKind.LabelsChanged);
        activity.ActorId.Should().Be(UserId);
        activity.ActorName.Should().Be("Ana");
        activity.AddedLabels.Should().Equal("Escalated");
        activity.RemovedLabels.Should().Equal("Media");
        activity.FromSeverity.Should().BeNull();
        activity.ToStageId.Should().BeNull();
    }

    [Fact]
    public void OtherKinds_HaveNoLabels()
    {
        var activity = ReviewActivity.FollowUpStatusChanged(Target, new ReviewActor(UserId, "Ana"), "Resolved", Now);

        activity.AddedLabels.Should().BeNull();
        activity.RemovedLabels.Should().BeNull();
    }
}
