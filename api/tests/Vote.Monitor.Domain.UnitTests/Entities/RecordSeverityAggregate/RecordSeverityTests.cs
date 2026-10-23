using Vote.Monitor.Domain.Entities.RecordSeverityAggregate;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.UnitTests.Entities.RecordSeverityAggregate;

public class RecordSeverityTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly DateTime Now = new(2026, 10, 21, 9, 0, 0, DateTimeKind.Utc);

    private static RecordSeverity Create(SeverityLevel severity) =>
        RecordSeverity.Create(Guid.NewGuid(), Guid.NewGuid(), ReportRecordType.IncidentReport, Guid.NewGuid(), severity,
            UserId, Now);

    [Fact]
    public void Create_SetsSeverityAndWhoChangedIt()
    {
        var severity = Create(SeverityLevel.High);

        severity.Severity.Should().Be(SeverityLevel.High);
        severity.ChangedBy.Should().Be(UserId);
        severity.ChangedAt.Should().Be(Now);
    }

    [Fact]
    public void Change_ToAnotherLevel_UpdatesItAndReturnsTrue()
    {
        var severity = Create(SeverityLevel.Low);
        var otherUser = Guid.NewGuid();
        var later = Now.AddHours(1);

        var changed = severity.Change(SeverityLevel.Urgent, otherUser, later);

        changed.Should().BeTrue();
        severity.Severity.Should().Be(SeverityLevel.Urgent);
        severity.ChangedBy.Should().Be(otherUser);
        severity.ChangedAt.Should().Be(later);
    }

    [Fact]
    public void Change_ToTheSameLevel_ChangesNothing()
    {
        var severity = Create(SeverityLevel.Medium);

        var changed = severity.Change(SeverityLevel.Medium, Guid.NewGuid(), Now.AddHours(1));

        changed.Should().BeFalse();
        severity.ChangedBy.Should().Be(UserId);
        severity.ChangedAt.Should().Be(Now);
    }

    [Fact]
    public void Levels_AscendFromNoneToUrgent()
    {
        SeverityLevel.List.OrderBy(x => x.Level).Select(x => x.Value)
            .Should().Equal("None", "Low", "Medium", "High", "Urgent");
    }

    [Fact]
    public void SeverityChangedActivity_KeepsBothLevels()
    {
        var target = new ReviewTarget(Guid.NewGuid(), Guid.NewGuid(), ReportRecordType.QuickReport, Guid.NewGuid());

        var activity = ReviewActivity.SeverityChanged(target, new ReviewActor(UserId, "Ana"), SeverityLevel.Low,
            SeverityLevel.Urgent, Now);

        activity.Kind.Should().Be(ReviewActivityKind.SeverityChanged);
        activity.FromSeverity.Should().Be(SeverityLevel.Low);
        activity.ToSeverity.Should().Be(SeverityLevel.Urgent);
        activity.FromStageId.Should().BeNull();
    }
}
