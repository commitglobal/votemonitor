using System.Net;
using System.Net.Http.Json;
using Module.Answers.Requests;
using Module.Forms.Requests;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Api.IntegrationTests.Features.FormSubmissions;

using static ApiTesting;

/// <summary>
/// Answered and unanswered questions follow display logic: only the questions the observer was shown count
/// (advanced filters contract, section 6).
/// </summary>
public class UnansweredQuestionsTests : BaseApiTestFixture
{
    public record EntryResponse(Guid SubmissionId, int NumberOfQuestionsAnswered, int? NumberOfUnansweredQuestions);

    public record IncidentEntryResponse(Guid IncidentReportId, int NumberOfQuestionsAnswered,
        int? NumberOfUnansweredQuestions);

    public record CountsResponse(int NumberOfQuestionsAnswered, int NumberOfUnansweredQuestions);

    public record RecipientResponse(Guid Id);

    public record PagedResponse<T>(List<T> Items, int TotalCount);

    /// <summary>A form's question ids and A's options, as the API returned them.</summary>
    private sealed record FormIds(Guid FormId, Guid A, Guid B, Guid C, Guid Yes, Guid No);

    /// <summary>Alfa monitors Alice and Bob; the round has polling stations in Iasi and Cluj.</summary>
    private ScenarioData Scenario() =>
        ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo
                    .WithMonitoringObserver(ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioObserver.Bob)))
            .Please();

    private static TranslatedString Text(string text) => new() { { "EN", text }, { "RO", text } };

    /// <summary>
    /// Creates and publishes a form with A (single select: Yes / No), B (text, shown by <paramref name="displayLogic"/>
    /// of A's Yes option) and C (text).
    /// </summary>
    private static FormIds CreateForm(HttpClient admin, Guid electionRoundId, FormType formType,
        Func<Guid, Guid, DisplayLogicRequest> displayLogic)
    {
        var a = Guid.NewGuid();
        var yes = Guid.NewGuid();
        var request = new CreateFormRequest
        {
            Code = "DL",
            DefaultLanguage = "EN",
            Languages = ["EN", "RO"],
            Name = Text("Display logic"),
            Description = Text("Display logic"),
            FormType = formType,
            Questions =
            [
                new SingleSelectQuestionRequest
                {
                    Id = a,
                    Code = "A",
                    Text = Text("A"),
                    Options =
                    [
                        new SelectOptionRequest { Id = yes, Text = Text("Yes") },
                        new SelectOptionRequest { Id = Guid.NewGuid(), Text = Text("No") }
                    ]
                },
                new TextQuestionRequest
                {
                    Id = Guid.NewGuid(), Code = "B", Text = Text("B"), DisplayLogic = displayLogic(a, yes)
                },
                new TextQuestionRequest { Id = Guid.NewGuid(), Code = "C", Text = Text("C") }
            ]
        };

        var form = admin.PostWithResponse<CreateFormRequest>($"/api/election-rounds/{electionRoundId}/forms", request);
        admin.PostWithoutResponse($"/api/election-rounds/{electionRoundId}/forms/{form.Id}:publish");

        var questionA = form.Questions.OfType<SingleSelectQuestionRequest>().Single(x => x.Code == "A");
        return new FormIds(form.Id,
            questionA.Id,
            form.Questions.Single(x => x.Code == "B").Id,
            form.Questions.Single(x => x.Code == "C").Id,
            questionA.Options.Single(x => x.Text["EN"] == "Yes").Id,
            questionA.Options.Single(x => x.Text["EN"] == "No").Id);
    }

    private static DisplayLogicRequest AnyOfYes(Guid parent, Guid yes) => new()
    {
        ParentQuestionId = parent, Condition = DisplayLogicCondition.AnyOf, OptionIds = [yes.ToString()]
    };

    /// <summary>The shape older rules were stored in: a condition naming one option in value.</summary>
    private static DisplayLogicRequest LegacyIncludesYes(Guid parent, Guid yes) => new()
    {
        ParentQuestionId = parent, Condition = DisplayLogicCondition.Includes, Value = yes.ToString()
    };

    private static List<BaseAnswerRequest> Answers(FormIds form, Guid pickedInA, string? c = "fine")
    {
        List<BaseAnswerRequest> answers =
        [
            new SingleSelectAnswerRequest
            {
                QuestionId = form.A,
                Selection = new SelectedOptionRequest { OptionId = pickedInA, Text = pickedInA == form.Yes ? "Yes" : "No" }
            }
        ];

        if (c is not null)
        {
            answers.Add(new TextAnswerRequest { QuestionId = form.C, Text = c });
        }

        return answers;
    }

    private static Guid Submit(ScenarioData data, ScenarioObserver observer, ScenarioPollingStation pollingStation,
        FormIds form, List<BaseAnswerRequest> answers) =>
        data.ObserverByName(observer).PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{data.ElectionRoundId}/form-submissions",
            new FormSubmissionRequest
            {
                PollingStationId = data.ElectionRound.PollingStationByName(pollingStation),
                FormId = form.FormId,
                Answers = answers
            }).Id;

    private static List<EntryResponse> Entries(HttpClient client, Guid electionRoundId, object? filter = null) =>
        client.PostWithResponse<PagedResponse<EntryResponse>>(
            $"/api/election-rounds/{electionRoundId}/form-submissions:byEntryV2",
            new { dataSource = "Ngo", filter, pageSize = 100 }).Items;

    private static List<Guid> EntriesV1(HttpClient client, Guid electionRoundId, string query) =>
        client.GetResponse<PagedResponse<EntryResponse>>(
                $"/api/election-rounds/{electionRoundId}/form-submissions:byEntry?dataSource=Ngo&pageSize=100&{query}")
            .Items.Select(x => x.SubmissionId).ToList();

    private static List<Guid> Recipients(HttpClient admin, Guid electionRoundId, object filter) =>
        admin.PostWithResponse<PagedResponse<RecipientResponse>>(
                $"/api/election-rounds/{electionRoundId}/notifications:listRecipientsV2",
                new { filter, pageSize = 100 })
            .Items.Select(x => x.Id).ToList();

    private static object Rule(string field, object value) => new { field, op = "eq", value };

    [Test]
    public void AHiddenQuestionCountsForNeither_AShownOneLeftEmptyIsUnanswered()
    {
        var data = Scenario();
        var er = data.ElectionRoundId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var form = CreateForm(admin, er, FormType.Opening, AnyOfYes);

        // Bob answered A "No" and C: B is hidden. Alice answered A "Yes" and C: B is shown and unanswered.
        var bobs = Submit(data, ScenarioObserver.Bob, ScenarioPollingStation.Iasi, form, Answers(form, form.No));
        var alices = Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Iasi, form, Answers(form, form.Yes));
        // An answer to B left over from before A changed to "No" counts for neither.
        var leftOver = Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Cluj, form,
            [.. Answers(form, form.No), new TextAnswerRequest { QuestionId = form.B, Text = "left over" }]);

        var entries = Entries(admin, er);
        entries.Single(x => x.SubmissionId == bobs).Should().BeEquivalentTo(new EntryResponse(bobs, 2, 0));
        entries.Single(x => x.SubmissionId == alices).Should().BeEquivalentTo(new EntryResponse(alices, 2, 1));
        entries.Single(x => x.SubmissionId == leftOver).Should().BeEquivalentTo(new EntryResponse(leftOver, 2, 0));

        // The submission's page reads both counts (the view function didn't return the answered count).
        admin.GetResponse<CountsResponse>($"/api/election-rounds/{er}/form-submissions/{alices}:v2")
            .Should().Be(new CountsResponse(2, 1));
        admin.GetResponse<CountsResponse>($"/api/election-rounds/{er}/form-submissions/{bobs}:v2")
            .Should().Be(new CountsResponse(2, 0));

        // V2 filters.
        Entries(admin, er, Rule("questionsAnswered", "Some")).Select(x => x.SubmissionId)
            .Should().BeEquivalentTo([alices]);
        Entries(admin, er, Rule("questionsAnswered", "All")).Select(x => x.SubmissionId)
            .Should().BeEquivalentTo([bobs, leftOver]);
        Entries(admin, er, Rule("questionsAnswered", "None")).Should().BeEmpty();

        // Simple (V1) query parameters.
        EntriesV1(admin, er, "questionsAnswered=All").Should().BeEquivalentTo([bobs, leftOver]);
        EntriesV1(admin, er, "questionsAnswered=Some").Should().BeEquivalentTo([alices]);

        // Push-message recipients: observers with such a submission.
        var monitoringNgo = data.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa);
        var alice = monitoringNgo.ObserverByName(ScenarioObserver.Alice).MonitoringObserverId;
        var bob = monitoringNgo.ObserverByName(ScenarioObserver.Bob).MonitoringObserverId;
        Recipients(admin, er, Rule("fs:questionsAnswered", "Some")).Should().BeEquivalentTo([alice]);
        Recipients(admin, er, Rule("fs:questionsAnswered", "All")).Should().BeEquivalentTo([alice, bob]);
    }

    [Test]
    public void AnsweringTheShownQuestionLeavesNoneUnanswered_AndNothingAnsweredIsNone()
    {
        var data = Scenario();
        var er = data.ElectionRoundId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var form = CreateForm(admin, er, FormType.Opening, AnyOfYes);

        var submission = Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Iasi, form,
            [.. Answers(form, form.Yes), new TextAnswerRequest { QuestionId = form.B, Text = "shown" }]);
        Entries(admin, er).Single().Should().BeEquivalentTo(new EntryResponse(submission, 3, 0));

        // Editing it to nothing: A and C are unanswered, B is hidden again.
        Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Iasi, form, []);
        Entries(admin, er).Single().Should().BeEquivalentTo(new EntryResponse(submission, 0, 2));
        Entries(admin, er, Rule("questionsAnswered", "None")).Select(x => x.SubmissionId)
            .Should().Equal(submission);
    }

    [Test]
    public void AnOlderRuleNamingTheOptionInValue_ShowsTheQuestionWhenThatOptionIsPicked()
    {
        var data = Scenario();
        var er = data.ElectionRoundId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var form = CreateForm(admin, er, FormType.Opening, LegacyIncludesYes);

        var yes = Submit(data, ScenarioObserver.Alice, ScenarioPollingStation.Iasi, form, Answers(form, form.Yes));
        var no = Submit(data, ScenarioObserver.Bob, ScenarioPollingStation.Iasi, form, Answers(form, form.No));

        var entries = Entries(admin, er);
        entries.Single(x => x.SubmissionId == yes).Should().BeEquivalentTo(new EntryResponse(yes, 2, 1));
        entries.Single(x => x.SubmissionId == no).Should().BeEquivalentTo(new EntryResponse(no, 2, 0));
    }

    [Test]
    public void IncidentReports_CountShownQuestions_InTheirPageListFiltersAndRecipients()
    {
        var data = Scenario();
        var er = data.ElectionRoundId;
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var form = CreateForm(admin, er, FormType.IncidentReporting, AnyOfYes);
        var monitoringNgo = data.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa);

        Guid Report(ScenarioObserver observer, Guid pickedInA)
        {
            var id = Guid.NewGuid();
            data.ObserverByName(observer)
                .PostAsJsonAsync($"/api/election-rounds/{er}/incident-reports",
                    new
                    {
                        id,
                        formId = form.FormId,
                        locationType = "NotRelatedToAPollingStation",
                        locationDescription = "At the town hall",
                        answers = Answers(form, pickedInA, c: null)
                    })
                .GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.OK);
            return id;
        }

        // Alice: A "Yes", so B and C are unanswered. Bob: A "No", so only C is.
        var alices = Report(ScenarioObserver.Alice, form.Yes);
        var bobs = Report(ScenarioObserver.Bob, form.No);

        admin.GetResponse<CountsResponse>($"/api/election-rounds/{er}/incident-reports/{alices}")
            .Should().Be(new CountsResponse(1, 2));
        admin.GetResponse<CountsResponse>($"/api/election-rounds/{er}/incident-reports/{bobs}")
            .Should().Be(new CountsResponse(1, 1));

        List<IncidentEntryResponse> Entries(object? filter = null) =>
            admin.PostWithResponse<PagedResponse<IncidentEntryResponse>>(
                $"/api/election-rounds/{er}/incident-reports:byEntryV2",
                new { dataSource = "Ngo", filter, pageSize = 100 }).Items;

        Entries().Should().BeEquivalentTo([
            new IncidentEntryResponse(alices, 1, 2), new IncidentEntryResponse(bobs, 1, 1)
        ]);
        Entries(Rule("questionsAnswered", "Some")).Should().HaveCount(2);
        Entries(Rule("questionsAnswered", "All")).Should().BeEmpty();

        // Bob answers C too: nothing he was shown is left unanswered.
        data.ObserverByName(ScenarioObserver.Bob)
            .PostAsJsonAsync($"/api/election-rounds/{er}/incident-reports",
                new
                {
                    id = bobs,
                    formId = form.FormId,
                    locationType = "NotRelatedToAPollingStation",
                    locationDescription = "At the town hall",
                    answers = Answers(form, form.No)
                })
            .GetAwaiter().GetResult().StatusCode.Should().Be(HttpStatusCode.OK);

        Entries(Rule("questionsAnswered", "All")).Select(x => x.IncidentReportId).Should().Equal(bobs);
        Recipients(admin, er, Rule("ir:questionsAnswered", "Some")).Should()
            .BeEquivalentTo([monitoringNgo.ObserverByName(ScenarioObserver.Alice).MonitoringObserverId]);
    }
}
