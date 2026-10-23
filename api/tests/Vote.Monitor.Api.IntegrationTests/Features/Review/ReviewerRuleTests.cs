using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Module.Answers.Requests;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.Review;

using static ApiTesting;

public class ReviewerRuleTests : BaseApiTestFixture
{
    private const string Sam = "Sam";
    private const string Tom = "Tom";

    public record StageResponse(Guid Id, string Name);

    public record RuleResponse(Guid Id, string Name, string[] RecordTypes, Guid[] StageIds, JsonElement? Conditions,
        Guid[] ReviewerIds, DateTime CreatedAt, DateTime UpdatedAt, Guid UpdatedById, string UpdatedByName);

    public record ReviewerResponse(Guid Id);

    public record ReviewResponse(ReviewerResponse[] Reviewers);

    public record HistoryEntryResponse(string Kind, Guid? ActorId, string? ActorName, Guid? RuleId, string? RuleName,
        string? RuleKind, Guid? ReviewerId);

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
            .WithNgo(ScenarioNgos.Alfa, ngo => ngo.WithStaff(Sam).WithStaff(Tom))
            .WithNgo(ScenarioNgos.Beta, ngo => ngo.WithStaff(Sam))
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

    private static string RulesUri(Guid electionRoundId) => $"/api/election-rounds/{electionRoundId}/reviewer-rules";

    private static string ReviewUri(Guid electionRoundId, Guid submissionId) =>
        $"/api/election-rounds/{electionRoundId}/reviews/FormSubmission/{submissionId}";

    private static Guid StageId(HttpClient client, Guid electionRoundId, string name) =>
        client.GetResponse<StageResponse[]>($"/api/election-rounds/{electionRoundId}/review-stages")
            .Single(x => x.Name == name).Id;

    private static object Rule(string name, Guid[] stageIds, Guid[] reviewerIds, object? conditions = null,
        string[]? recordTypes = null) =>
        new { name, recordTypes = recordTypes ?? ["FormSubmission"], stageIds, conditions, reviewerIds };

    private static object Level1(params string[] values) =>
        new { op = "and", values = new object[] { new { field = "level1", op = "in", values } } };

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

    private static void MoveTo(HttpClient client, Guid electionRoundId, Guid submissionId, Guid stageId) =>
        client.PutWithoutResponse($"{ReviewUri(electionRoundId, submissionId)}:stage", new { stageId });

    private static Guid[] Reviewers(HttpClient client, Guid electionRoundId, Guid submissionId) =>
        client.GetResponse<ReviewResponse>(ReviewUri(electionRoundId, submissionId)).Reviewers.Select(x => x.Id)
            .ToArray();

    private static HistoryEntryResponse[] History(HttpClient client, Guid electionRoundId, Guid submissionId) =>
        client.GetResponse<HistoryEntryResponse[]>($"{ReviewUri(electionRoundId, submissionId)}/history");

    private static Guid Submit(ScenarioData data, ScenarioObserver observer, ScenarioPollingStation pollingStation) =>
        data.ObserverByName(observer).PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{data.ElectionRoundId}/form-submissions",
            new FormSubmissionRequest
            {
                PollingStationId = data.ElectionRound.PollingStationByName(pollingStation),
                FormId = data.ElectionRound.Coalition.FormId,
                Answers = []
            }).Id;

    [Test]
    public void AdminsCreateReadUpdateAndDeleteRules()
    {
        var (data, _, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var inReview = StageId(alfa.Admin, er, "In review");
        var reviewed = StageId(alfa.Admin, er, "Reviewed");
        var conditions = Level1("Iasi");

        var created = Send(alfa.Admin, HttpMethod.Post, RulesUri(er),
            Rule("  North  ", [inReview], [alfa.StaffIdByName(Sam)], conditions));
        created.StatusCode.Should().Be(HttpStatusCode.Created);
        var rule = created.Content.ReadFromJsonAsync<RuleResponse>().GetAwaiter().GetResult()!;
        CreateRule(alfa.Admin, er, Rule("Everything", [reviewed], [alfa.AdminId], recordTypes: ["QuickReport"]));

        rule.Name.Should().Be("North");
        rule.RecordTypes.Should().Equal("FormSubmission");
        rule.StageIds.Should().Equal(inReview);
        rule.ReviewerIds.Should().Equal(alfa.StaffIdByName(Sam));
        JsonNode.DeepEquals(JsonNode.Parse(rule.Conditions!.Value.GetRawText()), JsonSerializer.SerializeToNode(conditions))
            .Should().BeTrue();
        rule.UpdatedById.Should().Be(alfa.AdminId);
        rule.UpdatedByName.Should().NotBeEmpty();
        alfa.Admin.GetResponse<RuleResponse>($"{RulesUri(er)}/{rule.Id}").Should().BeEquivalentTo(rule,
            options => options.Excluding(x => x.Conditions)
                .Using<DateTime>(x => x.Subject.Should().BeCloseTo(x.Expectation, TimeSpan.FromMilliseconds(1)))
                .WhenTypeIs<DateTime>());
        alfa.Admin.GetResponse<RuleResponse[]>(RulesUri(er)).Select(x => x.Name).Should().Equal("Everything", "North");

        var updated = alfa.Admin.PutWithResponse<RuleResponse>($"{RulesUri(er)}/{rule.Id}",
            Rule("North and Cluj", [inReview, reviewed], [alfa.StaffIdByName(Sam), alfa.StaffIdByName(Tom)],
                recordTypes: ["FormSubmission", "IncidentReport"]));
        updated.Name.Should().Be("North and Cluj");
        updated.RecordTypes.Should().Equal("FormSubmission", "IncidentReport");
        updated.StageIds.Should().Equal(inReview, reviewed);
        updated.Conditions.Should().BeNull();
        updated.UpdatedAt.Should().BeOnOrAfter(updated.CreatedAt);

        Send(alfa.Admin, HttpMethod.Delete, $"{RulesUri(er)}/{rule.Id}").StatusCode.Should().Be(HttpStatusCode.NoContent);
        Send(alfa.Admin, HttpMethod.Get, $"{RulesUri(er)}/{rule.Id}").StatusCode.Should().Be(HttpStatusCode.NotFound);
        alfa.Admin.GetResponse<RuleResponse[]>(RulesUri(er)).Select(x => x.Name).Should().Equal("Everything");
    }

    [Test]
    public void StaffSeeRulesButOnlyAdminsChangeThem_OtherNgosAndPlatformAdminsCannot()
    {
        var (data, _, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var beta = data.NgoByName(ScenarioNgos.Beta);
        var er = data.ElectionRoundId;
        var inReview = StageId(alfa.Admin, er, "In review");
        var rule = CreateRule(alfa.Admin, er, Rule("North", [inReview], [alfa.AdminId]));
        var uri = $"{RulesUri(er)}/{rule.Id}";
        var body = Rule("Renamed", [inReview], [alfa.AdminId]);

        alfa.StaffByName(Sam).GetResponse<RuleResponse[]>(RulesUri(er)).Should().ContainSingle();
        alfa.StaffByName(Sam).GetResponse<RuleResponse>(uri).Name.Should().Be("North");
        Send(alfa.StaffByName(Sam), HttpMethod.Post, RulesUri(er), body).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        Send(alfa.StaffByName(Sam), HttpMethod.Put, uri, body).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        Send(alfa.StaffByName(Sam), HttpMethod.Delete, uri).StatusCode.Should().Be(HttpStatusCode.Forbidden);

        beta.Admin.GetResponse<RuleResponse[]>(RulesUri(er)).Should().BeEmpty();
        Send(beta.Admin, HttpMethod.Get, uri).StatusCode.Should().Be(HttpStatusCode.NotFound);
        Send(beta.Admin, HttpMethod.Put, uri, Rule("Renamed", [StageId(beta.Admin, er, "In review")], [beta.AdminId]))
            .StatusCode.Should().Be(HttpStatusCode.NotFound);
        Send(beta.Admin, HttpMethod.Delete, uri).StatusCode.Should().Be(HttpStatusCode.NotFound);

        Send(data.PlatformAdmin, HttpMethod.Get, RulesUri(er)).IsSuccessStatusCode.Should().BeFalse();
        Send(data.PlatformAdmin, HttpMethod.Post, RulesUri(er), body).IsSuccessStatusCode.Should().BeFalse();

        alfa.Admin.GetResponse<RuleResponse>(uri).Name.Should().Be("North");
    }

    [Test]
    public void AdminsChangeRulesInAnArchivedRound()
    {
        var (data, _, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var inReview = StageId(alfa.Admin, er, "In review");
        var rule = CreateRule(alfa.Admin, er, Rule("North", [inReview], [alfa.AdminId]));
        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}:archive", new { });

        Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("South", [inReview], [alfa.AdminId])).StatusCode
            .Should().Be(HttpStatusCode.Created);
        Send(alfa.Admin, HttpMethod.Put, $"{RulesUri(er)}/{rule.Id}", Rule("Renamed", [inReview], [alfa.AdminId]))
            .StatusCode.Should().Be(HttpStatusCode.OK);
        Send(alfa.Admin, HttpMethod.Delete, $"{RulesUri(er)}/{rule.Id}").StatusCode.Should().Be(HttpStatusCode.NoContent);
        alfa.StaffByName(Sam).GetResponse<RuleResponse[]>(RulesUri(er)).Select(x => x.Name).Should().Equal("South");
    }

    [Test]
    public void InvalidRulesAreRejectedPerField()
    {
        var (data, _, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var beta = data.NgoByName(ScenarioNgos.Beta);
        var er = data.ElectionRoundId;
        var inReview = StageId(alfa.Admin, er, "In review");
        var existing = CreateRule(alfa.Admin, er, Rule("North", [inReview], [alfa.AdminId]));
        var other = CreateRule(alfa.Admin, er, Rule("South", [inReview], [alfa.AdminId]));

        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule(" ", [inReview], [alfa.AdminId])))
            .Select(x => x.Name).Should().Equal("name");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("nORTH", [inReview], [alfa.AdminId])))
            .Select(x => x.Name).Should().Equal("name");
        Errors(Send(alfa.Admin, HttpMethod.Put, $"{RulesUri(er)}/{other.Id}", Rule("north", [inReview], [alfa.AdminId])))
            .Select(x => x.Name).Should().Equal("name");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("New", [inReview], [alfa.AdminId], recordTypes: [])))
            .Select(x => x.Name).Should().Equal("recordTypes");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er),
                Rule("New", [inReview], [alfa.AdminId], recordTypes: ["QuickReport", "QuickReport"])))
            .Select(x => x.Name).Should().Equal("recordTypes");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("New", [], [alfa.AdminId])))
            .Select(x => x.Name).Should().Equal("stageIds");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er),
                Rule("New", [StageId(beta.Admin, er, "In review")], [alfa.AdminId])))
            .Select(x => x.Name).Should().Equal("stageIds");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("New", [inReview], [])))
            .Select(x => x.Name).Should().Equal("reviewerIds");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("New", [inReview], [beta.AdminId])))
            .Select(x => x.Name).Should().Equal("reviewerIds");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er),
                Rule("New", [inReview], Enumerable.Range(0, 11).Select(_ => Guid.NewGuid()).ToArray())))
            .Select(x => x.Name).Should().Equal("reviewerIds");

        alfa.Admin.PutWithResponse<RuleResponse>($"{RulesUri(er)}/{existing.Id}",
            Rule("NORTH", [inReview], [alfa.AdminId])).Name.Should().Be("NORTH");
    }

    [Test]
    public void ConditionsMayOnlyUseTheFieldsOfEveryRecordType()
    {
        var (data, _, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var inReview = StageId(alfa.Admin, er, "In review");
        var formType = new { op = "and", values = new object[] { new { field = "formType", op = "in", values = new[] { "Opening" } } } };
        var tags = new { op = "and", values = new object[] { new { field = "tags", op = "in", values = new[] { "urban" } } } };
        var answer = new
        {
            op = "and",
            values = new object[] { new { op = "answer-isEmpty", form = Guid.NewGuid(), question = Guid.NewGuid() } }
        };
        var tooDeep = new
        {
            op = "and",
            values = new object[]
            {
                new { op = "or", values = new object[] { new { op = "and", values = new object[] { Level1("Iasi") } } } }
            }
        };

        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er),
                Rule("New", [inReview], [alfa.AdminId], formType, ["FormSubmission", "QuickReport"])))
            .Should().Equal(("conditions", "Field 'formType' can't be used for quick reports"));
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("New", [inReview], [alfa.AdminId], answer)))
            .Should().Equal(("conditions", "Answer conditions can't be used in reviewer rules"));
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("New", [inReview], [alfa.AdminId], tooDeep)))
            .Select(x => x.Name).Should().Equal("conditions");
        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("New", [inReview], [alfa.AdminId],
                new { op = "and", values = new object[] { new { field = "level1", op = "contains", value = "x" } } })))
            .Select(x => x.Name).Should().Equal("conditions");

        Send(alfa.Admin, HttpMethod.Post, RulesUri(er),
                Rule("Tags", [inReview], [alfa.AdminId], tags, ["FormSubmission", "QuickReport", "IncidentReport"]))
            .StatusCode.Should().Be(HttpStatusCode.Created);
        Send(alfa.Admin, HttpMethod.Post, RulesUri(er),
                Rule("Everything", [inReview], [alfa.AdminId], new { op = "and", values = Array.Empty<object>() }))
            .StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Test]
    public void CitizenReportsNeedTheNgoToDoCitizenReporting()
    {
        var (data, _, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var rule = Rule("Citizens", [StageId(alfa.Admin, er, "In review")], [alfa.AdminId],
            recordTypes: ["CitizenReport"]);

        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), rule)).Select(x => x.Name).Should().Equal("recordTypes");

        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}:enableCitizenReporting",
            new { ngoId = alfa.NgoId });

        Send(alfa.Admin, HttpMethod.Post, RulesUri(er), rule).StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Test]
    public void ARecordEnteringARuleStageGetsItsReviewers_IfItMatches()
    {
        var (data, aliceSubmissionId, bobSubmissionId) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var inReview = StageId(alfa.Admin, er, "In review");
        var rule = CreateRule(alfa.Admin, er, Rule("North", [inReview], [alfa.StaffIdByName(Sam)], Level1("Iasi")));

        MoveTo(alfa.Admin, er, bobSubmissionId, inReview);
        MoveTo(alfa.Admin, er, aliceSubmissionId, inReview);

        Reviewers(alfa.Admin, er, bobSubmissionId).Should().Equal(alfa.StaffIdByName(Sam));
        Reviewers(alfa.Admin, er, aliceSubmissionId).Should().BeEmpty();

        var history = History(alfa.Admin, er, bobSubmissionId);
        history.Select(x => x.Kind).Should().Equal("ReviewerAdded", "StageChanged");
        history[0].Should().BeEquivalentTo(new HistoryEntryResponse("ReviewerAdded", null, null, rule.Id, "North",
            "Reviewer", alfa.StaffIdByName(Sam)));
        history[1].ActorId.Should().Be(alfa.AdminId);
        history[1].RuleId.Should().BeNull();

        var inbox = alfa.StaffByName(Sam).GetResponse<PagedResponse<InboxItemResponse>>("/api/inbox");
        inbox.Items.Should().ContainSingle().Which.Should().BeEquivalentTo(new
        {
            Reason = "AddedAsReviewer",
            RecordId = bobSubmissionId,
            Activity = new { Kind = "ReviewerAdded", ActorId = (Guid?)null, RuleId = rule.Id, RuleName = "North" }
        });
    }

    [Test]
    public void RulesAddTheUnionOfTheirReviewers_AndNeverRemoveAnyone()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var inReview = StageId(alfa.Admin, er, "In review");
        var sam = alfa.StaffIdByName(Sam);
        var tom = alfa.StaffIdByName(Tom);
        var first = CreateRule(alfa.Admin, er, Rule("A first", [inReview], [sam, alfa.AdminId]));
        var second = CreateRule(alfa.Admin, er, Rule("B second", [inReview], [tom, sam]));
        alfa.Admin.PutWithoutResponse($"{ReviewUri(er, aliceSubmissionId)}:reviewers", new { userIds = new[] { alfa.AdminId } });

        MoveTo(alfa.Admin, er, aliceSubmissionId, inReview);

        Reviewers(alfa.Admin, er, aliceSubmissionId).Should().BeEquivalentTo(new[] { alfa.AdminId, sam, tom });
        History(alfa.Admin, er, aliceSubmissionId).Where(x => x.Kind == "ReviewerAdded" && x.RuleId != null)
            .Select(x => (x.ReviewerId!.Value, x.RuleId!.Value))
            .Should().BeEquivalentTo(new[] { (sam, first.Id), (tom, second.Id) });
        alfa.Admin.GetResponse<PagedResponse<InboxItemResponse>>("/api/inbox").Items
            .Where(x => x.Reason == "ReviewingRecordChanged" && x.Activity.Kind == "ReviewerAdded")
            .Should().HaveCount(2);

        // Moving it out and back in: the rules find everyone already there, and nobody is removed.
        alfa.Admin.PutWithoutResponse($"{ReviewUri(er, aliceSubmissionId)}:reviewers", new { userIds = new[] { tom } });
        MoveTo(alfa.Admin, er, aliceSubmissionId, StageId(alfa.Admin, er, "Reviewed"));
        MoveTo(alfa.Admin, er, aliceSubmissionId, inReview);

        Reviewers(alfa.Admin, er, aliceSubmissionId).Should().BeEquivalentTo(new[] { tom, sam, alfa.AdminId });
    }

    [Test]
    public void RulesStopAtTenReviewers()
    {
        var staff = Enumerable.Range(1, 10).Select(i => $"Staff{i}").ToArray();
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithNgo(ScenarioNgos.Alfa, ngo =>
            {
                foreach (var name in staff)
                {
                    ngo.WithStaff(name);
                }
            })
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo
                    .WithMonitoringObserver(ScenarioObserver.Alice)
                    .WithForm("A", form => form.WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi))))
            .Please();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var submissionId = data.ElectionRound.MonitoringNgo.FormByCode("A")
            .GetSubmissionId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);
        var inReview = StageId(alfa.Admin, er, "In review");
        alfa.Admin.PutWithoutResponse($"{ReviewUri(er, submissionId)}:reviewers", new { userIds = new[] { alfa.AdminId } });
        CreateRule(alfa.Admin, er, Rule("Everyone", [inReview], staff.Select(alfa.StaffIdByName).ToArray()));

        MoveTo(alfa.Admin, er, submissionId, inReview);

        var reviewers = Reviewers(alfa.Admin, er, submissionId);
        reviewers.Should().HaveCount(10).And.Contain(alfa.AdminId);
        reviewers.Should().NotContain(alfa.StaffIdByName(staff[^1]));
    }

    [Test]
    public void ReviewersWhoAreNoLongerActiveAreSkipped()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var inReview = StageId(alfa.Admin, er, "In review");
        CreateRule(alfa.Admin, er, Rule("Both", [inReview], [alfa.StaffIdByName(Sam), alfa.StaffIdByName(Tom)]));
        data.PlatformAdmin.PostWithoutResponse($"/api/ngos/{alfa.NgoId}/staff/{alfa.StaffIdByName(Tom)}:deactivate",
            new { });

        MoveTo(alfa.Admin, er, aliceSubmissionId, inReview);

        Reviewers(alfa.Admin, er, aliceSubmissionId).Should().Equal(alfa.StaffIdByName(Sam));
    }

    [Test]
    public void ReviewersWhoLeftMayStayOnARule_ButOnlyActiveOnesCanBeAdded()
    {
        var (data, _, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var inReview = StageId(alfa.Admin, er, "In review");
        var tom = alfa.StaffIdByName(Tom);
        var sam = alfa.StaffIdByName(Sam);
        var rule = CreateRule(alfa.Admin, er, Rule("Tom's", [inReview], [tom]));
        data.PlatformAdmin.PostWithoutResponse($"/api/ngos/{alfa.NgoId}/staff/{tom}:deactivate", new { });
        data.PlatformAdmin.PostWithoutResponse($"/api/ngos/{alfa.NgoId}/staff/{sam}:deactivate", new { });

        Errors(Send(alfa.Admin, HttpMethod.Post, RulesUri(er), Rule("Copy", [inReview], [tom])))
            .Select(x => x.Name).Should().Equal("reviewerIds");
        alfa.Admin.PutWithResponse<RuleResponse>($"{RulesUri(er)}/{rule.Id}", Rule("Renamed", [inReview], [tom]))
            .Name.Should().Be("Renamed");
        Errors(Send(alfa.Admin, HttpMethod.Put, $"{RulesUri(er)}/{rule.Id}", Rule("Renamed", [inReview], [tom, sam])))
            .Select(x => x.Name).Should().Equal("reviewerIds");
    }

    [Test]
    public void NewRecordsEnterTheDefaultStage_AndTheLeadersRulesSeeMembersRecords()
    {
        var (data, aliceSubmissionId, bobSubmissionId) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var beta = data.NgoByName(ScenarioNgos.Beta);
        var er = data.ElectionRoundId;
        var notReviewed = StageId(alfa.Admin, er, "Not reviewed");
        var rule = CreateRule(alfa.Admin, er, Rule("North", [notReviewed], [alfa.StaffIdByName(Sam)], Level1("Iasi")));
        CreateRule(alfa.Admin, er, Rule("In review only", [StageId(alfa.Admin, er, "In review")], [alfa.StaffIdByName(Tom)]));

        var aliceInIasi = Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Iasi);
        var bobInCluj = Submit(data, ScenarioObserver.Bob, ScenarioPollingStation.Cluj);
        // Editing an existing record isn't entering a stage.
        Submit(data, ScenarioObserver.Bob, ScenarioPollingStation.Iasi).Should().Be(bobSubmissionId);

        Reviewers(alfa.Admin, er, aliceInIasi).Should().Equal(alfa.StaffIdByName(Sam));
        Reviewers(alfa.Admin, er, bobInCluj).Should().BeEmpty();
        Reviewers(alfa.Admin, er, bobSubmissionId).Should().BeEmpty();
        Reviewers(alfa.Admin, er, aliceSubmissionId).Should().BeEmpty();
        History(alfa.Admin, er, aliceInIasi).Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new { Kind = "ReviewerAdded", RuleId = rule.Id, ActorId = (Guid?)null });

        var bobNew = data.ObserverByName(ScenarioObserver.Bob).PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{er}/form-submissions/{Guid.NewGuid()}",
            new
            {
                pollingStationId = data.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi),
                formId = data.ElectionRound.Coalition.FormId,
                answers = Array.Empty<object>(),
                createdAt = DateTime.UtcNow,
                lastUpdatedAt = DateTime.UtcNow
            }).Id;

        Reviewers(alfa.Admin, er, bobNew).Should().Equal(alfa.StaffIdByName(Sam));
        Reviewers(beta.Admin, er, bobNew).Should().BeEmpty();
        History(beta.Admin, er, bobNew).Should().BeEmpty();
    }

    [Test]
    public void DeletingAStageRemovesItFromTheRules_RulesLeftWithoutStagesStay()
    {
        var (data, _, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var inReview = StageId(alfa.Admin, er, "In review");
        var escalated = alfa.Admin.PostWithResponse<StageResponse>($"/api/election-rounds/{er}/review-stages",
            new { name = "Escalated", status = "InReview" }).Id;
        var both = CreateRule(alfa.Admin, er, Rule("Both", [escalated, inReview], [alfa.AdminId]));
        var escalatedOnly = CreateRule(alfa.Admin, er, Rule("Escalated only", [escalated], [alfa.AdminId]));

        Send(alfa.Admin, HttpMethod.Delete, $"/api/election-rounds/{er}/review-stages/{escalated}").StatusCode
            .Should().Be(HttpStatusCode.NoContent);

        alfa.Admin.GetResponse<RuleResponse>($"{RulesUri(er)}/{both.Id}").StageIds.Should().Equal(inReview);
        alfa.Admin.GetResponse<RuleResponse>($"{RulesUri(er)}/{escalatedOnly.Id}").StageIds.Should().BeEmpty();
        Errors(Send(alfa.Admin, HttpMethod.Put, $"{RulesUri(er)}/{escalatedOnly.Id}",
                Rule("Escalated only", [], [alfa.AdminId])))
            .Select(x => x.Name).Should().Equal("stageIds");
    }
}
