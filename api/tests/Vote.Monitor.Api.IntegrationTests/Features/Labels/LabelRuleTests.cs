using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Module.Answers.Requests;
using Module.Forms.Requests;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.Labels;

using static ApiTesting;

public class LabelRuleTests : BaseApiTestFixture
{
    private const string Sam = "Sam";

    public record RuleResponse(Guid Id, string Name, string[] RecordTypes, JsonElement? Conditions, string[] Labels,
        Guid UpdatedById);

    public record LabelsResponse(string[] Labels);

    public record ReviewStateResponse(string Severity, string[] Labels);

    public record RuleExistsResponse(string Kind, Guid? RuleId, bool RuleExists);

    public record HistoryEntryResponse(string Kind, Guid? ActorId, string? ActorName, Guid? RuleId, string? RuleName,
        string? RuleKind, string[]? AddedLabels, string[]? RemovedLabels);

    public record InboxItemResponse(string Reason, Guid RecordId, HistoryEntryResponse Activity);

    public record PagedResponse<T>(List<T> Items, int TotalCount);

    /// <summary>
    /// Alfa leads a coalition with Beta; Alice (Alfa) submitted the common form in Cluj and Bob (Beta) in Iasi. Each
    /// polling station's level 1 is its city.
    /// </summary>
    private (ScenarioData Data, Guid AliceSubmissionId) CoalitionScenario()
    {
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa, ngo => ngo.WithStaff(Sam))
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi, level1: "Iasi")
                .WithPollingStation(ScenarioPollingStation.Cluj, level1: "Cluj")
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Common", [ScenarioNgos.Alfa, ScenarioNgos.Beta], form => form
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Cluj)
                        .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Iasi))))
            .Please();

        return (data,
            data.ElectionRound.Coalition.GetSubmissionId("Common", ScenarioObserver.Alice, ScenarioPollingStation.Cluj));
    }

    private static string RulesUri(Guid electionRoundId) => $"/api/election-rounds/{electionRoundId}/label-rules";

    private static string LabelsUri(Guid electionRoundId, Guid submissionId) =>
        $"/api/election-rounds/{electionRoundId}/labels/FormSubmission/{submissionId}";

    private static string ReviewUri(Guid electionRoundId, Guid submissionId) =>
        $"/api/election-rounds/{electionRoundId}/reviews/FormSubmission/{submissionId}";

    private static object Rule(string name, string[] labels, object? conditions = null, string[]? recordTypes = null) =>
        new { name, recordTypes = recordTypes ?? ["FormSubmission"], conditions, labels };

    private static object Where(params object[] values) => new { op = "and", values };

    private static object Level1(params string[] values) => new { field = "level1", op = "in", values };

    private static RuleResponse CreateRule(HttpClient admin, Guid electionRoundId, object rule) =>
        admin.PostWithResponse<RuleResponse>(RulesUri(electionRoundId), rule);

    private static HttpResponseMessage Send(HttpClient client, HttpMethod method, string uri, object? body = null) =>
        client.SendAsync(new HttpRequestMessage(method, uri) { Content = body is null ? null : JsonContent.Create(body) })
            .GetAwaiter().GetResult();

    /// <summary>The validation errors of a 400 response, as (field, message) pairs.</summary>
    private static (string Name, string Reason)[] Errors(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var json = JsonDocument.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult()).RootElement;
        return json.GetProperty("errors").EnumerateArray()
            .Select(x => (x.GetProperty("name").GetString()!, x.GetProperty("reason").GetString()!))
            .ToArray();
    }

    private static string[] Labels(HttpClient client, Guid electionRoundId, Guid submissionId) =>
        client.GetResponse<LabelsResponse>(LabelsUri(electionRoundId, submissionId)).Labels;

    private static HistoryEntryResponse[] LabelHistory(HttpClient client, Guid electionRoundId, Guid submissionId) =>
        client.GetResponse<HistoryEntryResponse[]>($"{ReviewUri(electionRoundId, submissionId)}/history")
            .Where(x => x.Kind == "LabelsChanged")
            .ToArray();

    private static Guid NumberQuestionId(ScenarioData data) =>
        data.ElectionRound.Coalition.Form.Questions.OfType<NumberQuestionRequest>().Single().Id;

    /// <summary>Submits (or, at a place where the observer already did, edits) the common form, answering its number question.</summary>
    private static Guid Submit(ScenarioData data, ScenarioObserver observer, ScenarioPollingStation pollingStation,
        int? number = null) =>
        data.ObserverByName(observer).PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{data.ElectionRoundId}/form-submissions",
            new FormSubmissionRequest
            {
                PollingStationId = data.ElectionRound.PollingStationByName(pollingStation),
                FormId = data.ElectionRound.Coalition.FormId,
                Answers = number is null
                    ? []
                    : [new NumberAnswerRequest { QuestionId = NumberQuestionId(data), Value = number.Value }]
            }).Id;

    [Test]
    public void AdminsManageLabelRules_StaffOnlySeeThem()
    {
        var (data, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var beta = data.NgoByName(ScenarioNgos.Beta);
        var er = data.ElectionRoundId;

        var created = Send(alfa.Admin, HttpMethod.Post, RulesUri(er),
            Rule("North", [" Ballot ", "escalate", "BALLOT"], Where(Level1("Iasi")), ["FormSubmission", "QuickReport"]));
        created.StatusCode.Should().Be(HttpStatusCode.Created);
        var rule = created.Content.ReadFromJsonAsync<RuleResponse>().GetAwaiter().GetResult()!;
        var uri = $"{RulesUri(er)}/{rule.Id}";

        // Trimmed, each once ignoring case: the first spelling is kept.
        rule.Should().BeEquivalentTo(new
        {
            Name = "North",
            RecordTypes = new[] { "FormSubmission", "QuickReport" },
            Labels = new[] { "Ballot", "escalate" },
            UpdatedById = alfa.AdminId
        });
        alfa.StaffByName(Sam).GetResponse<RuleResponse[]>(RulesUri(er)).Select(x => x.Id).Should().Equal(rule.Id);
        alfa.StaffByName(Sam).GetResponse<RuleResponse>(uri).Labels.Should().Equal("Ballot", "escalate");
        Send(alfa.StaffByName(Sam), HttpMethod.Post, RulesUri(er), Rule("South", ["x"])).StatusCode
            .Should().Be(HttpStatusCode.Forbidden);
        Send(alfa.StaffByName(Sam), HttpMethod.Put, uri, Rule("North", ["x"])).StatusCode
            .Should().Be(HttpStatusCode.Forbidden);
        Send(alfa.StaffByName(Sam), HttpMethod.Delete, uri).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        Send(beta.Admin, HttpMethod.Get, uri).StatusCode.Should().Be(HttpStatusCode.NotFound);
        Send(data.PlatformAdmin, HttpMethod.Get, RulesUri(er)).IsSuccessStatusCode.Should().BeFalse();

        alfa.Admin.PutWithResponse<RuleResponse>(uri, Rule("North", ["Media"])).Should().BeEquivalentTo(new
        {
            Labels = new[] { "Media" },
            RecordTypes = new[] { "FormSubmission" },
            Conditions = (JsonElement?)null
        });

        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}:archive", new { });
        Send(alfa.Admin, HttpMethod.Delete, uri).StatusCode.Should().Be(HttpStatusCode.NoContent);
        alfa.Admin.GetResponse<RuleResponse[]>(RulesUri(er)).Should().BeEmpty();
    }

    [Test]
    public void InvalidLabelRulesAreRejectedPerField()
    {
        var (data, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        CreateRule(alfa.Admin, er, Rule("North", ["Ballot"]));

        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("NORTH", ["Ballot"])))
            .Select(x => x.Name).Should().Equal("name");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("South", [])))
            .Select(x => x.Name).Should().Equal("labels");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("South", ["Ballot", "  "])))
            .Select(x => x.Name).Should().Equal("labels");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("South", [new string('x', 65)])))
            .Select(x => x.Name).Should().Equal("labels");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er),
                Rule("South", Enumerable.Range(0, 11).Select(x => $"label {x}").ToArray())))
            .Select(x => x.Name).Should().Equal("labels");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("South", ["Ballot"], recordTypes: ["CitizenReport"])))
            .Select(x => x.Name).Should().Equal("recordTypes");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er),
                Rule("South", ["Ballot"], Where(new { field = "tags", op = "in", values = new[] { "urban" } }))))
            .Should().Equal(("conditions", "Field 'tags' can't be used in label rules"));

        // Ten distinct labels are fine, also with case duplicates and padding beyond them.
        var ten = Enumerable.Range(0, 10).Select(x => $"label {x}").ToList();
        CreateRule(alfa.Admin, er, Rule("Ten", [..ten, "LABEL 0", "  label 1  "])).Labels.Should().Equal(ten);
    }

    [Test]
    public void ANewMatchingRecordGetsTheRulesLabels_OneHistoryEntryPerRuleThatAddedSome()
    {
        var (data, aliceSubmissionId) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var first = CreateRule(alfa.Admin, er, Rule("A north", ["Ballot", "Escalate"], Where(Level1("Iasi"))));
        var second = CreateRule(alfa.Admin, er, Rule("B everywhere", ["escalate", "Media"]));
        CreateRule(alfa.Admin, er, Rule("C nothing new", ["BALLOT"]));
        CreateRule(alfa.Admin, er, Rule("D south", ["South"], Where(Level1("Cluj"))));

        var aliceInIasi = Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Iasi);

        Labels(alfa.Admin, er, aliceInIasi).Should().Equal("Ballot", "Escalate", "Media");
        LabelHistory(alfa.Admin, er, aliceInIasi).Should().BeEquivalentTo(new[]
        {
            new HistoryEntryResponse("LabelsChanged", null, null, first.Id, "A north", "Labels", ["Ballot", "Escalate"], []),
            new HistoryEntryResponse("LabelsChanged", null, null, second.Id, "B everywhere", "Labels", ["Media"], [])
        });
        // Rules act on future events only.
        Labels(alfa.Admin, er, aliceSubmissionId).Should().BeEmpty();

        // The record's review state carries its labels; history entries say whether their rule still exists.
        alfa.Admin.GetResponse<ReviewStateResponse>(ReviewUri(er, aliceInIasi)).Labels
            .Should().Equal("Ballot", "Escalate", "Media");
        Send(alfa.Admin, HttpMethod.Delete, $"{RulesUri(er)}/{first.Id}").StatusCode.Should().Be(HttpStatusCode.NoContent);
        alfa.Admin.GetResponse<RuleExistsResponse[]>($"{ReviewUri(er, aliceInIasi)}/history")
            .Where(x => x.Kind == "LabelsChanged")
            .Select(x => (x.RuleId, x.RuleExists))
            .Should().BeEquivalentTo(new (Guid?, bool)[] { (first.Id, false), (second.Id, true) });
    }

    [Test]
    public void EditsAddLabels_RulesNeverRemoveOne_AndReviewersAreNotifiedOnce()
    {
        var (data, aliceSubmissionId) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        alfa.Admin.PutAsJsonAsync(LabelsUri(er, aliceSubmissionId), new { labels = new[] { "Media", "Legal" } })
            .GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.NoContent);
        alfa.Admin.PutWithoutResponse($"{ReviewUri(er, aliceSubmissionId)}:reviewers",
            new { userIds = new[] { alfa.StaffIdByName(Sam) } });
        var answer = Where(new
        {
            op = "answer-gte", form = data.ElectionRound.Coalition.FormId, question = NumberQuestionId(data), value = 5
        });
        var many = CreateRule(alfa.Admin, er, Rule("Many members", ["MEDIA", "Crowded"], answer));
        var also = CreateRule(alfa.Admin, er, Rule("Many members too", ["crowded", "Queue"], answer));

        Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Cluj, number: 3);
        Labels(alfa.Admin, er, aliceSubmissionId).Should().Equal("Legal", "Media");

        Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Cluj, number: 7);
        Labels(alfa.Admin, er, aliceSubmissionId).Should().Equal("Crowded", "Legal", "Media", "Queue");
        var history = LabelHistory(alfa.Admin, er, aliceSubmissionId);
        history.Should().HaveCount(3);
        history.Single(x => x.RuleId == many.Id).AddedLabels.Should().Equal("Crowded");
        history.Single(x => x.RuleId == also.Id).AddedLabels.Should().Equal("Queue");
        history.Single(x => x.RuleId == null).AddedLabels.Should().Equal("Legal", "Media");

        var inbox = alfa.StaffByName(Sam).GetResponse<PagedResponse<InboxItemResponse>>("/api/inbox");
        inbox.Items.Should().ContainSingle(x => x.Activity.Kind == "LabelsChanged").Which.Should().BeEquivalentTo(new
        {
            Reason = "ReviewingRecordChanged",
            RecordId = aliceSubmissionId,
            Activity = new { RuleKind = "Labels", ActorId = (Guid?)null }
        });

        // A label a person removed stays removed while no rule matches.
        alfa.Admin.PutAsJsonAsync(LabelsUri(er, aliceSubmissionId), new { labels = new[] { "Legal" } })
            .GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.NoContent);
        Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Cluj, number: 2);
        Labels(alfa.Admin, er, aliceSubmissionId).Should().Equal("Legal");
    }

    [Test]
    public void RulesUseTheSpellingTheNgoAlreadyUses_AndStopAtFiftyLabels()
    {
        var (data, aliceSubmissionId) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var fortyNine = Enumerable.Range(0, 49).Select(x => $"Existing {x:D2}").ToArray();
        alfa.Admin.PutAsJsonAsync(LabelsUri(er, aliceSubmissionId), new { labels = fortyNine })
            .GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.NoContent);
        CreateRule(alfa.Admin, er, Rule("A", ["New one", "New two"]));
        CreateRule(alfa.Admin, er, Rule("B", ["EXISTING 00"]));

        Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Cluj, number: 1);
        Labels(alfa.Admin, er, aliceSubmissionId).Should().HaveCount(50).And.Contain("New one").And.NotContain("New two");

        var aliceInIasi = Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Iasi);
        Labels(alfa.Admin, er, aliceInIasi).Should().Equal("Existing 00", "New one", "New two");
    }
}
