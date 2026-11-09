using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.IntegrationTests.Features.Review;

using static ApiTesting;

/// <summary>
/// The old web app keeps working on the records' fixed follow-up value (NotApplicable / NeedsFollowUp / Resolved):
/// it sets it with the V1 <c>…/{id}:status</c> endpoints and filters the V1 lists with <c>followUpStatus</c> /
/// <c>quickReportFollowUpStatus</c>, exactly as it did before the NGOs' own follow-up statuses.
/// </summary>
public class OldAppFollowUpTests : BaseApiTestFixture
{
    private record SubmissionRow(Guid SubmissionId, string FollowUpStatus);
    private record ObserverRow(Guid MonitoringObserverId, string? FollowUpStatus);
    private record QuickReportRow(Guid Id, string FollowUpStatus);
    private record StatusResponse(Guid Id, string Name);

    private ScenarioData Scenario() =>
        ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Bacau)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo
                    .WithMonitoringObserver(ScenarioObserver.Alice)
                    .WithForm("A", form => form
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Cluj)))
                .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Bacau))
            .Please();

    [Test]
    public void FormSubmissions_SetAndFilterTheFixedValue()
    {
        var data = Scenario();
        var er = data.ElectionRoundId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var form = data.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).FormByCode("A");
        var iasi = form.GetSubmissionId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);
        var cluj = form.GetSubmissionId(ScenarioObserver.Alice, ScenarioPollingStation.Cluj);

        admin.PutWithoutResponse($"/api/election-rounds/{er}/form-submissions/{iasi}:status", new { followUpStatus = "NeedsFollowUp" });

        var needs = admin.GetResponse<PagedResponse<SubmissionRow>>(
            $"/api/election-rounds/{er}/form-submissions:byEntry?dataSource=Ngo&followUpStatus=NeedsFollowUp");
        needs.Items.Should().ContainSingle().Which.Should().Be(new SubmissionRow(iasi, "NeedsFollowUp"));

        var notApplicable = admin.GetResponse<PagedResponse<SubmissionRow>>(
            $"/api/election-rounds/{er}/form-submissions:byEntry?dataSource=Ngo&followUpStatus=NotApplicable");
        notApplicable.Items.Should().ContainSingle().Which.SubmissionId.Should().Be(cluj);

        // By observer: the old app's "Needs followup" flag and filter.
        admin.GetResponse<PagedResponse<ObserverRow>>(
                $"/api/election-rounds/{er}/form-submissions:byObserver?dataSource=Ngo&followUpStatus=NeedsFollowUp")
            .Items.Should().ContainSingle().Which.FollowUpStatus.Should().Be("NeedsFollowUp");

        // Back to Resolved, then Not applicable: the old values keep round-tripping.
        admin.PutWithoutResponse($"/api/election-rounds/{er}/form-submissions/{iasi}:status", new { followUpStatus = "Resolved" });
        admin.GetResponse<PagedResponse<SubmissionRow>>(
                $"/api/election-rounds/{er}/form-submissions:byEntry?dataSource=Ngo&followUpStatus=Resolved")
            .Items.Should().ContainSingle().Which.SubmissionId.Should().Be(iasi);
        admin.GetResponse<PagedResponse<SubmissionRow>>(
                $"/api/election-rounds/{er}/form-submissions:byEntry?dataSource=Ngo&followUpStatus=NeedsFollowUp")
            .Items.Should().BeEmpty();
    }

    [Test]
    public void QuickReports_SetAndFilterTheFixedValue()
    {
        var data = Scenario();
        var er = data.ElectionRoundId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var iasi = data.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);
        var bacau = data.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Bacau);

        admin.PutWithoutResponse($"/api/election-rounds/{er}/quick-reports/{iasi}:status", new { followUpStatus = "Resolved" });

        admin.GetResponse<PagedResponse<QuickReportRow>>($"/api/election-rounds/{er}/quick-reports?quickReportFollowUpStatus=Resolved")
            .Items.Should().ContainSingle().Which.Should().Be(new QuickReportRow(iasi, "Resolved"));
        admin.GetResponse<PagedResponse<QuickReportRow>>($"/api/election-rounds/{er}/quick-reports?quickReportFollowUpStatus=NotApplicable")
            .Items.Should().ContainSingle().Which.Id.Should().Be(bacau);
    }

    [Test]
    public void TheNewAppsBuiltInStatuses_ShowInTheOldApp()
    {
        var data = Scenario();
        var er = data.ElectionRoundId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var iasi = data.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);
        var needs = admin.GetResponse<List<StatusResponse>>($"/api/election-rounds/{er}/follow-up-statuses")
            .Single(x => x.Name == "Needs follow-up");

        // Set in the new app…
        admin.PutWithoutResponse($"/api/election-rounds/{er}/follow-ups/QuickReport/{iasi}", new { followUpStatusId = needs.Id });

        // …seen and filtered in the old one.
        admin.GetResponse<PagedResponse<QuickReportRow>>($"/api/election-rounds/{er}/quick-reports?quickReportFollowUpStatus=NeedsFollowUp")
            .Items.Should().ContainSingle().Which.Id.Should().Be(iasi);
    }
}
