using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Module.Answers.Requests;
using Module.Forms.Requests;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.Review;

using static ApiTesting;

public class FollowUpRuleTests : BaseApiTestFixture
{
    private const string Sam = "Sam";

    public record RuleResponse(Guid Id, string Name, string[] RecordTypes, JsonElement? Conditions, Guid FollowUpStatusId,
        Guid UpdatedById, string UpdatedByName);

    public record StatusResponse(Guid Id, string Name, bool IsDefault);

    public record ViewResponse(string FollowUpStatus, Guid? FollowUpStatusId, string? FollowUpStatusName);

    public record HistoryEntryResponse(string Kind, Guid? ActorId, string? ActorName, Guid? RuleId, string? RuleName,
        string? RuleKind, string? FollowUpStatus);

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

    private static string RulesUri(Guid electionRoundId) => $"/api/election-rounds/{electionRoundId}/follow-up-rules";

    private static string StatusesUri(Guid electionRoundId) => $"/api/election-rounds/{electionRoundId}/follow-up-statuses";

    private static string ReviewUri(Guid electionRoundId, Guid submissionId) =>
        $"/api/election-rounds/{electionRoundId}/reviews/FormSubmission/{submissionId}";

    private static object Rule(string name, Guid followUpStatusId, object? conditions = null, string[]? recordTypes = null) =>
        new { name, recordTypes = recordTypes ?? ["FormSubmission"], conditions, followUpStatusId };

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

    private static StatusResponse Status(HttpClient admin, Guid electionRoundId, string name) =>
        admin.GetResponse<StatusResponse[]>(StatusesUri(electionRoundId)).Single(x => x.Name == name);

    private static ViewResponse FollowUp(HttpClient client, Guid electionRoundId, Guid submissionId) =>
        client.GetResponse<ViewResponse>($"/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}:v2");

    private static HistoryEntryResponse[] FollowUpHistory(HttpClient client, Guid electionRoundId, Guid submissionId) =>
        client.GetResponse<HistoryEntryResponse[]>($"{ReviewUri(electionRoundId, submissionId)}/history")
            .Where(x => x.Kind == "FollowUpStatusChanged")
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
    public void AdminsManageFollowUpRules_StaffOnlySeeThem()
    {
        var (data, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var beta = data.NgoByName(ScenarioNgos.Beta);
        var er = data.ElectionRoundId;
        var needs = Status(alfa.Admin, er, "Needs follow-up");
        var resolved = Status(alfa.Admin, er, "Resolved");

        var created = Send(alfa.Admin, HttpMethod.Post, RulesUri(er),
            Rule("North", needs.Id, Where(Level1("Iasi")), ["FormSubmission", "QuickReport"]));
        created.StatusCode.Should().Be(HttpStatusCode.Created);
        var rule = created.Content.ReadFromJsonAsync<RuleResponse>().GetAwaiter().GetResult()!;
        var uri = $"{RulesUri(er)}/{rule.Id}";

        rule.Should().BeEquivalentTo(new
        {
            Name = "North",
            RecordTypes = new[] { "FormSubmission", "QuickReport" },
            FollowUpStatusId = needs.Id,
            UpdatedById = alfa.AdminId
        });
        alfa.StaffByName(Sam).GetResponse<RuleResponse[]>(RulesUri(er)).Select(x => x.Id).Should().Equal(rule.Id);
        alfa.StaffByName(Sam).GetResponse<RuleResponse>(uri).FollowUpStatusId.Should().Be(needs.Id);
        Send(alfa.StaffByName(Sam), HttpMethod.Post, RulesUri(er), Rule("South", needs.Id)).StatusCode
            .Should().Be(HttpStatusCode.Forbidden);
        Send(alfa.StaffByName(Sam), HttpMethod.Put, uri, Rule("North", resolved.Id)).StatusCode
            .Should().Be(HttpStatusCode.Forbidden);
        Send(alfa.StaffByName(Sam), HttpMethod.Delete, uri).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        Send(beta.Admin, HttpMethod.Get, uri).StatusCode.Should().Be(HttpStatusCode.NotFound);
        Send(data.PlatformAdmin, HttpMethod.Get, RulesUri(er)).IsSuccessStatusCode.Should().BeFalse();

        alfa.Admin.PutWithResponse<RuleResponse>(uri, Rule("North", resolved.Id)).Should().BeEquivalentTo(new
        {
            FollowUpStatusId = resolved.Id,
            RecordTypes = new[] { "FormSubmission" },
            Conditions = (JsonElement?)null
        });

        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}:archive", new { });
        CreateRule(alfa.Admin, er, Rule("While archived", needs.Id)).Name.Should().Be("While archived");
        Send(alfa.Admin, HttpMethod.Delete, uri).StatusCode.Should().Be(HttpStatusCode.NoContent);
        alfa.Admin.GetResponse<RuleResponse[]>(RulesUri(er)).Select(x => x.Name).Should().Equal("While archived");
    }

    [Test]
    public void InvalidFollowUpRulesAreRejectedPerField()
    {
        var (data, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var beta = data.NgoByName(ScenarioNgos.Beta);
        var er = data.ElectionRoundId;
        var needs = Status(alfa.Admin, er, "Needs follow-up");
        var betaNeeds = Status(beta.Admin, er, "Needs follow-up");
        CreateRule(alfa.Admin, er, Rule("North", needs.Id));

        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("NORTH", needs.Id)))
            .Select(x => x.Name).Should().Equal("name");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("South", betaNeeds.Id)))
            .Select(x => x.Name).Should().Equal("followUpStatusId");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("South", Guid.NewGuid())))
            .Select(x => x.Name).Should().Equal("followUpStatusId");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("South", Guid.Empty)))
            .Select(x => x.Name).Should().Equal("followUpStatusId");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("South", needs.Id, recordTypes: ["CitizenReport"])))
            .Select(x => x.Name).Should().Equal("recordTypes");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er),
                Rule("South", needs.Id, Where(new { field = "tags", op = "in", values = new[] { "urban" } }))))
            .Should().Equal(("conditions", "Field 'tags' can't be used in follow-up rules"));

        // The default is allowed; it never changes anything.
        Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("Default", Status(alfa.Admin, er, "Not applicable").Id))
            .StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Test]
    public void ANewRecordGetsTheStatusOfTheFirstMatchingRuleByName_AttributedToIt()
    {
        var (data, aliceSubmissionId) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var notApplicable = Status(alfa.Admin, er, "Not applicable");
        var needs = Status(alfa.Admin, er, "Needs follow-up");
        var resolved = Status(alfa.Admin, er, "Resolved");
        // Before the others by name, but it sets the default: skipped.
        CreateRule(alfa.Admin, er, Rule("A default", notApplicable.Id));
        var north = CreateRule(alfa.Admin, er, Rule("B north", needs.Id, Where(Level1("Iasi"))));
        CreateRule(alfa.Admin, er, Rule("C everywhere", resolved.Id));
        CreateRule(alfa.Admin, er, Rule("A south", resolved.Id, Where(Level1("Cluj"))));
        CreateRule(alfa.Admin, er, Rule("A quick reports", resolved.Id, recordTypes: ["QuickReport"]));

        var aliceInIasi = Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Iasi);

        // Alfa owns the record and the status is one it started with: the record's own column follows.
        FollowUp(alfa.Admin, er, aliceInIasi).Should().BeEquivalentTo(
            new ViewResponse("NeedsFollowUp", needs.Id, "Needs follow-up"));
        FollowUpHistory(alfa.Admin, er, aliceInIasi).Should().ContainSingle().Which.Should().BeEquivalentTo(
            new HistoryEntryResponse("FollowUpStatusChanged", null, null, north.Id, "B north", "FollowUp",
                "Needs follow-up"));
        // Rules act on future events only: the existing record kept its status.
        FollowUp(alfa.Admin, er, aliceSubmissionId).FollowUpStatusName.Should().Be("Not applicable");
        FollowUpHistory(alfa.Admin, er, aliceSubmissionId).Should().BeEmpty();
    }

    [Test]
    public void EditsSetTheStatusWhileItIsTheDefault_ButRulesNeverOverrideAnother()
    {
        var (data, aliceSubmissionId) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var needs = Status(alfa.Admin, er, "Needs follow-up");
        var resolved = Status(alfa.Admin, er, "Resolved");
        alfa.Admin.PutWithoutResponse($"{ReviewUri(er, aliceSubmissionId)}:reviewers",
            new { userIds = new[] { alfa.StaffIdByName(Sam) } });
        var rule = CreateRule(alfa.Admin, er, Rule("Many members", needs.Id,
            Where(new { op = "answer-gte", form = data.ElectionRound.Coalition.FormId, question = NumberQuestionId(data), value = 5 })));

        Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Cluj, number: 3);
        FollowUp(alfa.Admin, er, aliceSubmissionId).FollowUpStatusName.Should().Be("Not applicable");

        Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Cluj, number: 7);
        FollowUp(alfa.Admin, er, aliceSubmissionId).FollowUpStatusName.Should().Be("Needs follow-up");
        var inbox = alfa.StaffByName(Sam).GetResponse<PagedResponse<InboxItemResponse>>("/api/inbox");
        inbox.Items.Should().ContainSingle(x => x.Activity.Kind == "FollowUpStatusChanged").Which.Should().BeEquivalentTo(new
        {
            Reason = "ReviewingRecordChanged",
            RecordId = aliceSubmissionId,
            Activity = new { RuleId = rule.Id, RuleName = "Many members", RuleKind = "FollowUp", ActorId = (Guid?)null }
        });

        // A person chose another status: rules leave it alone.
        alfa.Admin.PutWithoutResponse($"/api/election-rounds/{er}/follow-ups/FormSubmission/{aliceSubmissionId}",
            new { followUpStatusId = resolved.Id });
        Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Cluj, number: 9);
        FollowUp(alfa.Admin, er, aliceSubmissionId).FollowUpStatusName.Should().Be("Resolved");
        FollowUpHistory(alfa.Admin, er, aliceSubmissionId).Select(x => x.RuleName).Should().Equal(null, "Many members");
    }

    [Test]
    public void EachNgosRulesSetItsOwnFollowUp()
    {
        var (data, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var beta = data.NgoByName(ScenarioNgos.Beta);
        var er = data.ElectionRoundId;
        CreateRule(beta.Admin, er, Rule("Member", Status(beta.Admin, er, "Resolved").Id));

        var bobInCluj = Submit(data, ScenarioObserver.Bob, ScenarioPollingStation.Cluj);

        FollowUp(beta.Admin, er, bobInCluj).Should().BeEquivalentTo(
            new { FollowUpStatus = "Resolved", FollowUpStatusName = "Resolved" });
        alfa.Admin.GetResponse<HistoryEntryResponse[]>($"{ReviewUri(er, bobInCluj)}/history")
            .Should().NotContain(x => x.Kind == "FollowUpStatusChanged");
    }

    [Test]
    public void DeletingAStatusARuleUsesNeedsMoveTo_AndMovesTheRule()
    {
        var (data, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var needs = Status(alfa.Admin, er, "Needs follow-up");
        var calling = alfa.Admin.PostWithResponse<StatusResponse>(StatusesUri(er), new { name = "Calling the observer" });
        var rule = CreateRule(alfa.Admin, er, Rule("Call them", calling.Id));

        Send(alfa.Admin, HttpMethod.Delete, $"{StatusesUri(er)}/{calling.Id}").StatusCode
            .Should().Be(HttpStatusCode.Conflict);
        alfa.Admin.GetResponse<RuleResponse>($"{RulesUri(er)}/{rule.Id}").FollowUpStatusId.Should().Be(calling.Id);

        Send(alfa.Admin, HttpMethod.Delete, $"{StatusesUri(er)}/{calling.Id}?moveTo={needs.Id}").StatusCode
            .Should().Be(HttpStatusCode.NoContent);
        alfa.Admin.GetResponse<RuleResponse>($"{RulesUri(er)}/{rule.Id}").FollowUpStatusId.Should().Be(needs.Id);
        alfa.Admin.GetResponse<StatusResponse[]>(StatusesUri(er)).Should().NotContain(x => x.Id == calling.Id);

        // The moved rule runs with its new status.
        var aliceInIasi = Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Iasi);
        FollowUp(alfa.Admin, er, aliceInIasi).FollowUpStatusName.Should().Be("Needs follow-up");
    }

    [Test]
    public void RemovingTheNgoOrDeletingTheRoundTakesItsRulesAlong()
    {
        var data = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithMonitoringNgo(ScenarioNgos.Alfa)
                .WithMonitoringNgo(ScenarioNgos.Beta))
            .Please();
        var er = data.ElectionRoundId;
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var beta = data.NgoByName(ScenarioNgos.Beta);
        CreateRule(alfa.Admin, er, Rule("Alfa", Status(alfa.Admin, er, "Resolved").Id));
        CreateRule(beta.Admin, er, Rule("Beta", Status(beta.Admin, er, "Resolved").Id));

        Send(data.PlatformAdmin, HttpMethod.Delete, $"/api/election-rounds/{er}/monitoring-ngos/{data.NgoIdByName(ScenarioNgos.Beta)}")
            .StatusCode.Should().Be(HttpStatusCode.NoContent);
        Send(data.PlatformAdmin, HttpMethod.Delete, $"/api/election-rounds/{er}").StatusCode
            .Should().Be(HttpStatusCode.NoContent);
    }
}
