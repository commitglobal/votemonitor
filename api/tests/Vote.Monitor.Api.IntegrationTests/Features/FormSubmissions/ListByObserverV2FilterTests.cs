using Feature.Form.Submissions.ListByObserver;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.IntegrationTests.Features.FormSubmissions;

using static ApiTesting;

/// <summary>
/// <c>:byObserverV2</c> lists only the observers with at least one submission matching the filter; without a filter it
/// lists every observer of the NGO (those without submissions with zeros).
/// </summary>
public class ListByObserverV2FilterTests : BaseApiTestFixture
{
    private ScenarioData Scenario() =>
        ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi, level1: "Iasi", number: "01/001")
                .WithPollingStation(ScenarioPollingStation.Bacau, level1: "Bacau", number: "01/002")
                .WithPollingStation(ScenarioPollingStation.Cluj, level1: "Cluj", number: "02/001")
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo
                    .WithMonitoringObserver(ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioObserver.Bob)
                    .WithForm("A", form => form
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Cluj))))
            .Please();

    private static PagedResponse<ObserverSubmissionOverview> ByObserver(ScenarioData data, object? filter) =>
        data.NgoByName(ScenarioNgos.Alfa).Admin.PostWithResponse<PagedResponse<ObserverSubmissionOverview>>(
            $"/api/election-rounds/{data.ElectionRoundId}/form-submissions:byObserverV2",
            filter is null ? new { dataSource = "Ngo" } : new { dataSource = "Ngo", filter });

    private static object Level1(params string[] names) =>
        new { op = "and", values = new object[] { new { field = "level1", op = "in", values = names } } };

    [Test]
    public void WithoutFilter_ShouldListEveryObserver()
    {
        var data = Scenario();

        var result = ByObserver(data, null);

        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Items.Single(x => x.NumberOfFormsSubmitted == 0).Should().NotBeNull();
    }

    [Test]
    public void WithFilter_ShouldListOnlyObserversWithMatchingSubmissions()
    {
        var data = Scenario();
        var alice = data.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).ObserverByName(ScenarioObserver.Alice);

        var result = ByObserver(data, Level1("Iasi"));

        result.TotalCount.Should().Be(1);
        var row = result.Items.Should().ContainSingle().Subject;
        row.MonitoringObserverId.Should().Be(alice.MonitoringObserverId);
        // The counts are of the matching submissions only.
        row.NumberOfFormsSubmitted.Should().Be(1);
    }

    [Test]
    public void WithFilterMatchingNothing_ShouldListNoObservers()
    {
        var data = Scenario();

        var result = ByObserver(data, Level1("Bacau"));

        result.TotalCount.Should().Be(0);
        result.Items.Should().BeEmpty();
    }
}
