using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.Notifications;

using static ApiTesting;

public class RecipientsTests : BaseApiTestFixture
{
    public record RecipientResponse(Guid Id, string ObserverName);

    public record SentResponse(Guid Id, string Title);

    public record ReceiverResponse(Guid Id, string Name);

    public record DetailResponse(Guid Id, string Title, ReceiverResponse[] Receivers);

    public record PagedResponse<T>(List<T> Items, int TotalCount);

    /// <summary>Alfa monitors Alice, who sent nothing, and Bob, who submitted form A.</summary>
    private ScenarioData Scenario() =>
        ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo
                    .WithMonitoringObserver(ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioObserver.Bob)
                    .WithForm("A", form => form.WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Cluj))))
            .Please();

    private static string Uri(Guid electionRoundId, string action) =>
        $"/api/election-rounds/{electionRoundId}/notifications:{action}";

    [Test]
    public void SpecificObserversAreRecipientsEvenWithoutSubmissions()
    {
        var data = Scenario();
        var monitoringNgo = data.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa);
        var alice = monitoringNgo.ObserverByName(ScenarioObserver.Alice);
        var formId = monitoringNgo.FormByCode("A").FormId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;

        admin.GetResponse<PagedResponse<RecipientResponse>>(Uri(data.ElectionRoundId, "listRecipients")).TotalCount
            .Should().Be(2);

        var picked = admin.GetResponse<PagedResponse<RecipientResponse>>(
            $"{Uri(data.ElectionRoundId, "listRecipients")}?monitoringObserverIds={alice.MonitoringObserverId}");
        picked.TotalCount.Should().Be(1);
        picked.Items.Select(x => x.Id).Should().Equal(alice.MonitoringObserverId);

        // The picked observers are ANDed with the other filters: Alice submitted nothing.
        admin.GetResponse<PagedResponse<RecipientResponse>>(
                $"{Uri(data.ElectionRoundId, "listRecipients")}?monitoringObserverIds={alice.MonitoringObserverId}&formId={formId}")
            .TotalCount.Should().Be(0);
    }

    [Test]
    public void SendingToSpecificObserversReachesOnlyThem()
    {
        var data = Scenario();
        var alice = data.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).ObserverByName(ScenarioObserver.Alice);
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;

        admin.PostWithoutResponse(Uri(data.ElectionRoundId, "send"), new
        {
            title = "Check in",
            body = "<p>Please check in</p>",
            monitoringObserverIds = new[] { alice.MonitoringObserverId }
        });

        var sent = admin.GetResponse<PagedResponse<SentResponse>>(Uri(data.ElectionRoundId, "listSent")).Items.Single();
        var detail = admin.GetResponse<DetailResponse>($"/api/election-rounds/{data.ElectionRoundId}/notifications/{sent.Id}");
        detail.Receivers.Select(x => x.Name).Should().Equal(alice.DisplayName);
    }
}
