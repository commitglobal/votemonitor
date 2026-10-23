using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.UnitTests.Entities.ReviewAggregate;

public class ReviewWorkflowTests
{
    private static readonly Guid MonitoringNgoId = Guid.NewGuid();
    private static readonly DateTime Now = new(2026, 10, 21, 9, 0, 0, DateTimeKind.Utc);

    private static ReviewStage Stage(ReviewStatus status, Guid? monitoringNgoId = null) =>
        ReviewStage.Create(monitoringNgoId ?? MonitoringNgoId, status.Name, null, status, ReviewStage.SequenceStep);

    private static ReviewWorkflow Create(ReviewStage stage) =>
        ReviewWorkflow.Create(Guid.NewGuid(), MonitoringNgoId, ReportRecordType.QuickReport, Guid.NewGuid(), stage, Now);

    [Fact]
    public void Create_SetsStateAndChangeTime()
    {
        var stage = Stage(ReviewStatus.InReview);

        var workflow = Create(stage);

        workflow.StageId.Should().Be(stage.Id);
        workflow.StageChangedAt.Should().Be(Now);
        workflow.ReviewedAt.Should().BeNull();
    }

    [Fact]
    public void SetStage_ToReviewedGroup_SetsReviewedAt()
    {
        var workflow = Create(Stage(ReviewStatus.NotReviewed));
        var later = Now.AddHours(1);

        workflow.SetStage(Stage(ReviewStatus.Reviewed), later);

        workflow.ReviewedAt.Should().Be(later);
        workflow.StageChangedAt.Should().Be(later);
    }

    [Fact]
    public void SetStage_OutOfReviewedGroup_ClearsReviewedAt()
    {
        var workflow = Create(Stage(ReviewStatus.Reviewed));

        workflow.SetStage(Stage(ReviewStatus.InReview), Now.AddHours(1));

        workflow.ReviewedAt.Should().BeNull();
    }

    [Fact]
    public void SetStage_WithAnotherMonitoringNgosState_Throws()
    {
        var workflow = Create(Stage(ReviewStatus.NotReviewed));

        var act = () => workflow.SetStage(Stage(ReviewStatus.Reviewed, Guid.NewGuid()), Now);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CreateDefaults_ReturnsNotReviewedInReviewReviewed_WithNotReviewedAsTheOnlyDefault()
    {
        var stages = ReviewStage.CreateDefaults(MonitoringNgoId).ToList();

        stages.Select(x => (x.Name, x.Status, x.IsDefault)).Should().Equal(
            ("Not reviewed", ReviewStatus.NotReviewed, true),
            ("In review", ReviewStatus.InReview, false),
            ("Reviewed", ReviewStatus.Reviewed, false));
        stages.Should().OnlyContain(x => x.MonitoringNgoId == MonitoringNgoId);
    }
}
