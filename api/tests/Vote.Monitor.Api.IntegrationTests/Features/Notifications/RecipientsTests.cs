using System.Net;
using System.Net.Http.Json;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Domain.Entities.FormBase;

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

    /// <summary>
    /// Alfa monitors Alice, who sent a quick report in Cluj (South), and Bob, who submitted form A in Iasi (North)
    /// and reported an incident away from any polling station.
    /// </summary>
    private (ScenarioData Data, Guid Alice, Guid Bob, Guid FormId) V2Scenario()
    {
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi, "North")
                .WithPollingStation(ScenarioPollingStation.Cluj, "South")
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo
                    .WithMonitoringObserver(ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioObserver.Bob)
                    .WithForm("A", form => form.WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)))
                .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Cluj))
            .Please();

        var er = data.ElectionRoundId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var incidentForm = Dummy.Form("INC");
        incidentForm.FormType = FormType.IncidentReporting;
        var form = admin.PostWithResponse<CreateFormRequest>($"/api/election-rounds/{er}/forms", incidentForm);
        admin.PostWithoutResponse($"/api/election-rounds/{er}/forms/{form.Id}:publish");
        data.ObserverByName(ScenarioObserver.Bob).PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{er}/incident-reports",
            new
            {
                id = Guid.NewGuid(),
                formId = form.Id,
                locationType = "NotRelatedToAPollingStation",
                locationDescription = "At the town hall"
            });

        var monitoringNgo = data.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa);
        return (data,
            monitoringNgo.ObserverByName(ScenarioObserver.Alice).MonitoringObserverId,
            monitoringNgo.ObserverByName(ScenarioObserver.Bob).MonitoringObserverId,
            monitoringNgo.FormByCode("A").FormId);
    }

    /// <summary>The filter, ANDed with picked observers as a monitoringObserverId rule (as the web app sends them).</summary>
    private static object? WithPicked(object? filter, Guid[]? monitoringObserverIds)
    {
        if (monitoringObserverIds is null)
        {
            return filter;
        }

        var picked = new { field = "monitoringObserverId", op = "in", values = monitoringObserverIds };
        return filter is null ? picked : And(filter, picked);
    }

    private static HttpResponseMessage PostRecipientsV2(HttpClient admin, Guid electionRoundId, object? filter,
        Guid[]? monitoringObserverIds = null) =>
        admin.PostAsJsonAsync(Uri(electionRoundId, "listRecipientsV2"),
            new { filter = WithPicked(filter, monitoringObserverIds), pageSize = 100 }).GetAwaiter().GetResult();

    private static List<Guid> RecipientsV2(HttpClient admin, Guid electionRoundId, object? filter,
        Guid[]? monitoringObserverIds = null) =>
        admin.PostWithResponse<PagedResponse<RecipientResponse>>(Uri(electionRoundId, "listRecipientsV2"),
                new { filter = WithPicked(filter, monitoringObserverIds), pageSize = 100 })
            .Items.Select(x => x.Id).ToList();

    private static object Rule(string field, object value, string op = "eq") => new { field, op, value };

    private static object And(params object[] values) => new { op = "and", values };

    private static object Or(params object[] values) => new { op = "or", values };

    private static object Not(object value) => new { op = "not", values = new[] { value } };

    [Test]
    public void V2_ObserverFieldsMatchEveryObserver_RecordFieldsMatchObserversWithSuchARecord()
    {
        var (data, alice, bob, formA) = V2Scenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;

        RecipientsV2(admin, er, null).Should().BeEquivalentTo([alice, bob]);
        RecipientsV2(admin, er, Rule("monitoringObserverStatus", "Active")).Should().BeEquivalentTo([alice, bob]);

        RecipientsV2(admin, er, Rule("fs:formId", formA)).Should().Equal(bob);
        RecipientsV2(admin, er, Rule("qr:level1", "South")).Should().Equal(alice);
        RecipientsV2(admin, er, Rule("fs:level1", "South")).Should().BeEmpty();
        RecipientsV2(admin, er, Rule("ir:locationType", "NotRelatedToAPollingStation")).Should().Equal(bob);

        RecipientsV2(admin, er, Rule("hasQuickReports", true)).Should().Equal(alice);
        RecipientsV2(admin, er, Rule("hasQuickReports", false)).Should().Equal(bob);
        RecipientsV2(admin, er, Rule("hasIncidentReports", false)).Should().Equal(alice);
        RecipientsV2(admin, er, Rule("hasFormSubmissions", true)).Should().Equal(bob);

        RecipientsV2(admin, er, Or(Rule("fs:formId", formA), Rule("monitoringObserverId", alice)))
            .Should().BeEquivalentTo([alice, bob]);
        RecipientsV2(admin, er, Not(Rule("fs:formId", formA))).Should().Equal(alice);
        RecipientsV2(admin, er, Rule("fs:formId", formA), [alice]).Should().BeEmpty();
        RecipientsV2(admin, er, null, [alice]).Should().Equal(alice);

        var aliceName = data.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa)
            .ObserverByName(ScenarioObserver.Alice).DisplayName;
        RecipientsV2(admin, er, Or(Rule("name", aliceName.ToUpperInvariant(), "contains"),
                Rule("email", aliceName.ToUpperInvariant(), "contains")))
            .Should().Equal(alice);
    }

    [Test]
    public void V2_RecordRulesOfOneKindInAnAndMatchTheSameRecord_OfDifferentKindsAnyRecords()
    {
        var (data, _, bob, formA) = V2Scenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;

        RecipientsV2(admin, er, And(Rule("fs:formId", formA), Rule("fs:level1", "North"))).Should().Equal(bob);
        RecipientsV2(admin, er, And(Rule("fs:formId", formA), Rule("fs:level1", "South"))).Should().BeEmpty();
        RecipientsV2(admin, er, And(Rule("fs:level1", "North"), Rule("ir:locationType", "NotRelatedToAPollingStation")))
            .Should().Equal(bob);
        RecipientsV2(admin, er, And(Rule("fs:level1", "North"), Rule("qr:level1", "South"))).Should().BeEmpty();
    }

    [Test]
    public void V2_UnknownOrUnprefixedRecordFieldsAreRejected()
    {
        var (data, _, _, _) = V2Scenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;

        foreach (var field in new[] { "level1", "xx:level1", "qr:formId", "fs:nope" })
        {
            PostRecipientsV2(admin, er, Rule(field, "North")).StatusCode
                .Should().Be(System.Net.HttpStatusCode.BadRequest, field);
        }
    }

    [Test]
    public void V2_SendingReachesTheObserversMatchingTheFilter()
    {
        var (data, _, _, formA) = V2Scenario();
        var monitoringNgo = data.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa);
        var alice = monitoringNgo.ObserverByName(ScenarioObserver.Alice);
        var bob = monitoringNgo.ObserverByName(ScenarioObserver.Bob);
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;

        string[] Receivers(string title)
        {
            var sent = admin.GetResponse<PagedResponse<SentResponse>>(Uri(er, "listSent")).Items.Single(x => x.Title == title);
            return admin.GetResponse<DetailResponse>($"/api/election-rounds/{er}/notifications/{sent.Id}")
                .Receivers.Select(x => x.Name).ToArray();
        }

        admin.PostWithoutResponse(Uri(er, "sendV2"),
            new { title = "Quick reports", body = "<p>Thanks</p>", filter = Rule("hasQuickReports", true) });
        admin.PostWithoutResponse(Uri(er, "sendV2"),
            new { title = "Form A", body = "<p>Thanks</p>", filter = Rule("fs:formId", formA) });

        Receivers("Quick reports").Should().Equal(alice.DisplayName);
        Receivers("Form A").Should().Equal(bob.DisplayName);

        admin.PostAsJsonAsync(Uri(er, "sendV2"), new { title = "Bad", body = "<p>x</p>", filter = Rule("level1", "North") })
            .GetAwaiter().GetResult().StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}
