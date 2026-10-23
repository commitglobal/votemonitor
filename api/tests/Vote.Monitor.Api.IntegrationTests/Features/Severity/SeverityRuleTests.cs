using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Module.Answers.Requests;
using Module.Forms.Requests;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.Severity;

using static ApiTesting;

public class SeverityRuleTests : BaseApiTestFixture
{
    private const string Sam = "Sam";

    public record RuleResponse(Guid Id, string Name, string[] RecordTypes, JsonElement? Conditions, string Severity,
        Guid UpdatedById);

    public record SeverityResponse(string Severity);

    public record HistoryEntryResponse(string Kind, Guid? ActorId, string? ActorName, Guid? RuleId, string? RuleName,
        string? RuleKind, string? FromSeverity, string? ToSeverity);

    public record InboxItemResponse(string Reason, Guid RecordId, HistoryEntryResponse Activity);

    public record PagedResponse<T>(List<T> Items, int TotalCount);

    /// <summary>
    /// Alfa leads a coalition with Beta; Alice (Alfa) submitted the common form in Cluj and Bob (Beta) in Iasi. Each
    /// polling station's level 1 is its city.
    /// </summary>
    private (ScenarioData Data, Guid AliceSubmissionId, Guid BobSubmissionId) CoalitionScenario()
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

        var coalition = data.ElectionRound.Coalition;
        return (data,
            coalition.GetSubmissionId("Common", ScenarioObserver.Alice, ScenarioPollingStation.Cluj),
            coalition.GetSubmissionId("Common", ScenarioObserver.Bob, ScenarioPollingStation.Iasi));
    }

    private static string RulesUri(Guid electionRoundId) => $"/api/election-rounds/{electionRoundId}/severity-rules";

    private static string SeverityUri(Guid electionRoundId, Guid submissionId) =>
        $"/api/election-rounds/{electionRoundId}/severities/FormSubmission/{submissionId}";

    private static string ReviewUri(Guid electionRoundId, Guid submissionId) =>
        $"/api/election-rounds/{electionRoundId}/reviews/FormSubmission/{submissionId}";

    private static object Rule(string name, string severity, object? conditions = null, string[]? recordTypes = null) =>
        new { name, recordTypes = recordTypes ?? ["FormSubmission"], conditions, severity };

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

    private static string Severity(HttpClient client, Guid electionRoundId, Guid submissionId) =>
        client.GetResponse<SeverityResponse>(SeverityUri(electionRoundId, submissionId)).Severity;

    private static HistoryEntryResponse[] SeverityHistory(HttpClient client, Guid electionRoundId, Guid submissionId) =>
        client.GetResponse<HistoryEntryResponse[]>($"{ReviewUri(electionRoundId, submissionId)}/history")
            .Where(x => x.Kind == "SeverityChanged")
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
    public void AdminsManageSeverityRules_StaffOnlySeeThem()
    {
        var (data, _, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var beta = data.NgoByName(ScenarioNgos.Beta);
        var er = data.ElectionRoundId;

        var created = Send(alfa.Admin, HttpMethod.Post, RulesUri(er),
            Rule("North", "High", Where(Level1("Iasi")), ["FormSubmission", "QuickReport"]));
        created.StatusCode.Should().Be(HttpStatusCode.Created);
        var rule = created.Content.ReadFromJsonAsync<RuleResponse>().GetAwaiter().GetResult()!;
        var uri = $"{RulesUri(er)}/{rule.Id}";

        rule.Should().BeEquivalentTo(new
        {
            Name = "North",
            RecordTypes = new[] { "FormSubmission", "QuickReport" },
            Severity = "High",
            UpdatedById = alfa.AdminId
        });
        alfa.StaffByName(Sam).GetResponse<RuleResponse[]>(RulesUri(er)).Select(x => x.Id).Should().Equal(rule.Id);
        alfa.StaffByName(Sam).GetResponse<RuleResponse>(uri).Severity.Should().Be("High");
        Send(alfa.StaffByName(Sam), HttpMethod.Post, RulesUri(er), Rule("South", "Low")).StatusCode
            .Should().Be(HttpStatusCode.Forbidden);
        Send(alfa.StaffByName(Sam), HttpMethod.Put, uri, Rule("North", "Low")).StatusCode
            .Should().Be(HttpStatusCode.Forbidden);
        Send(alfa.StaffByName(Sam), HttpMethod.Delete, uri).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        Send(beta.Admin, HttpMethod.Get, uri).StatusCode.Should().Be(HttpStatusCode.NotFound);
        Send(data.PlatformAdmin, HttpMethod.Get, RulesUri(er)).IsSuccessStatusCode.Should().BeFalse();

        alfa.Admin.PutWithResponse<RuleResponse>(uri, Rule("North", "Urgent")).Should().BeEquivalentTo(new
        {
            Severity = "Urgent",
            RecordTypes = new[] { "FormSubmission" },
            Conditions = (JsonElement?)null
        });

        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}:archive", new { });
        Send(alfa.Admin, HttpMethod.Delete, uri).StatusCode.Should().Be(HttpStatusCode.NoContent);
        alfa.Admin.GetResponse<RuleResponse[]>(RulesUri(er)).Should().BeEmpty();
    }

    [Test]
    public void InvalidSeverityRulesAreRejectedPerField()
    {
        var (data, _, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var answer = new { op = "answer-gte", form = data.ElectionRound.Coalition.FormId, question = NumberQuestionId(data), value = 5 };
        CreateRule(alfa.Admin, er, Rule("North", "High"));

        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("NORTH", "High")))
            .Select(x => x.Name).Should().Equal("name");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("South", "None")))
            .Select(x => x.Name).Should().Equal("severity");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("South", "High", recordTypes: ["CitizenReport"])))
            .Select(x => x.Name).Should().Equal("recordTypes");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er),
                Rule("South", "High", Where(new { field = "tags", op = "in", values = new[] { "urban" } }))))
            .Should().Equal(("conditions", "Field 'tags' can't be used in severity rules"));
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er),
                Rule("South", "High", Where(answer), ["FormSubmission", "QuickReport"])))
            .Should().Equal(("conditions", "Answer conditions can't be used for quick reports"));
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er),
                Rule("South", "High", Where(new { field = "level1", op = "in", values = Enumerable.Range(0, 101).Select(x => $"{x}") }))))
            .Should().Equal(("conditions", "A condition can have at most 100 values"));

        Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("Answers", "High", Where(answer), ["FormSubmission", "IncidentReport"]))
            .StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Test]
    public void ANewRecordGetsTheHighestMatchingSeverity_AttributedToThatRule()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        CreateRule(alfa.Admin, er, Rule("Everywhere", "Medium"));
        CreateRule(alfa.Admin, er, Rule("North", "Urgent", Where(Level1("Iasi"))));
        var alsoNorth = CreateRule(alfa.Admin, er, Rule("Also north", "Urgent", Where(Level1("Iasi"))));
        CreateRule(alfa.Admin, er, Rule("South", "High", Where(Level1("Cluj"))));
        CreateRule(alfa.Admin, er, Rule("Quick reports", "High", recordTypes: ["QuickReport"]));

        var aliceInIasi = Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Iasi);

        Severity(alfa.Admin, er, aliceInIasi).Should().Be("Urgent");
        SeverityHistory(alfa.Admin, er, aliceInIasi).Should().ContainSingle().Which.Should().BeEquivalentTo(
            new HistoryEntryResponse("SeverityChanged", null, null, alsoNorth.Id, "Also north", "Severity", "None",
                "Urgent"));
        // Rules act on future events only: the existing record kept its severity.
        Severity(alfa.Admin, er, aliceSubmissionId).Should().Be("None");
    }

    [Test]
    public void EditsRaiseTheSeverity_ButRulesNeverLowerIt()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        alfa.Admin.PutWithoutResponse($"{ReviewUri(er, aliceSubmissionId)}:reviewers",
            new { userIds = new[] { alfa.StaffIdByName(Sam) } });
        var medium = CreateRule(alfa.Admin, er, Rule("Many members", "Medium",
            Where(new { op = "answer-gte", form = data.ElectionRound.Coalition.FormId, question = NumberQuestionId(data), value = 5 })));

        Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Cluj, number: 3);
        Severity(alfa.Admin, er, aliceSubmissionId).Should().Be("None");

        Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Cluj, number: 7);
        Severity(alfa.Admin, er, aliceSubmissionId).Should().Be("Medium");
        var inbox = alfa.StaffByName(Sam).GetResponse<PagedResponse<InboxItemResponse>>("/api/inbox");
        inbox.Items.Should().ContainSingle(x => x.Activity.Kind == "SeverityChanged").Which.Should().BeEquivalentTo(new
        {
            Reason = "ReviewingRecordChanged",
            RecordId = aliceSubmissionId,
            Activity = new { RuleId = medium.Id, RuleName = "Many members", ActorId = (Guid?)null, ToSeverity = "Medium" }
        });

        // Equal: nothing changes.
        Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Cluj, number: 8);
        SeverityHistory(alfa.Admin, er, aliceSubmissionId).Should().ContainSingle();

        // A person raised it higher: the rule doesn't lower it.
        alfa.Admin.PutWithoutResponse(SeverityUri(er, aliceSubmissionId), new { severity = "Urgent" });
        Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Cluj, number: 9);
        Severity(alfa.Admin, er, aliceSubmissionId).Should().Be("Urgent");
        SeverityHistory(alfa.Admin, er, aliceSubmissionId).Select(x => x.RuleName).Should().Equal(null, "Many members");
    }

    [Test]
    public void MovingARecordRunsSeverityRules()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        CreateRule(alfa.Admin, er, Rule("North", "High", Where(Level1("Iasi"))));

        data.PlatformAdmin.PutWithoutResponse($"/api/election-rounds/{er}/form-submissions/{aliceSubmissionId}:move",
            new { pollingStationId = data.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi) });

        Severity(alfa.Admin, er, aliceSubmissionId).Should().Be("High");
    }

    [Test]
    public void EachNgosRulesSeeTheRecordsItSees_TheLeaderSeesMembersRecords()
    {
        var (data, _, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var beta = data.NgoByName(ScenarioNgos.Beta);
        var er = data.ElectionRoundId;
        CreateRule(alfa.Admin, er, Rule("Leader", "Urgent"));
        CreateRule(beta.Admin, er, Rule("Member", "Low", Where(Level1("Cluj"))));

        var bobInCluj = Submit(data, ScenarioObserver.Bob, ScenarioPollingStation.Cluj);
        var aliceInIasi = Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Iasi);

        Severity(alfa.Admin, er, bobInCluj).Should().Be("Urgent");
        Severity(beta.Admin, er, bobInCluj).Should().Be("Low");
        Severity(alfa.Admin, er, aliceInIasi).Should().Be("Urgent");
        SeverityHistory(beta.Admin, er, bobInCluj).Should().ContainSingle().Which.RuleName.Should().Be("Member");
    }

    [Test]
    public void QuickReportsRunTheirSeverityRules()
    {
        var (data, _, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        CreateRule(alfa.Admin, er, Rule("North", "High", Where(Level1("Iasi")), ["QuickReport"]));

        data.ElectionRound.WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);
        data.ElectionRound.WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Cluj);

        var iasi = data.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);
        var cluj = data.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Cluj);
        alfa.Admin.GetResponse<SeverityResponse>($"/api/election-rounds/{er}/severities/QuickReport/{iasi}").Severity
            .Should().Be("High");
        alfa.Admin.GetResponse<SeverityResponse>($"/api/election-rounds/{er}/severities/QuickReport/{cluj}").Severity
            .Should().Be("None");
    }
}
