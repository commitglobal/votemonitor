using System.Text.Json;
using Module.Answers.Requests;
using Module.Forms.Requests;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.Review;

using static ApiTesting;

public class AnswerHistoryTests : BaseApiTestFixture
{
    public record StageResponse(Guid Id, string Name, string Status);

    public record AnswerChangeResponse(Guid QuestionId, JsonElement? PreviousAnswer, JsonElement? CurrentAnswer,
        DateTime ChangedAt);

    public record AnswerHistoryResponse(AnswerChangeResponse[] Changes, DateTime? ChangedAfterReviewAt);

    public record EntryResponse(Guid SubmissionId, bool ChangedAfterReview);

    public record PagedResponse<T>(List<T> Items, int TotalCount);

    private ScenarioData Scenario() =>
        ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Alice))
                .WithMonitoringNgo(ScenarioNgos.Beta)
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Common", [ScenarioNgos.Alfa, ScenarioNgos.Beta])))
            .Please();

    /// <summary>Submits (or edits) the common form at Iasi, answering its number question.</summary>
    private static Guid Submit(ScenarioData data, int number, ScenarioObserver? observer = null) =>
        data.ObserverByName(observer ?? ScenarioObserver.Alice).PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{data.ElectionRoundId}/form-submissions",
            new FormSubmissionRequest
            {
                PollingStationId = data.ElectionRound.PollingStationByName(ScenarioPollingStation.Iasi),
                FormId = data.ElectionRound.Coalition.FormId,
                Answers =
                [
                    new NumberAnswerRequest
                    {
                        QuestionId = data.ElectionRound.Coalition.Form.Questions.OfType<NumberQuestionRequest>()
                            .Single().Id,
                        Value = number
                    }
                ]
            }).Id;

    private static List<EntryResponse> Entries(HttpClient client, Guid electionRoundId, object? filter = null) =>
        client.PostWithResponse<PagedResponse<EntryResponse>>(
            $"/api/election-rounds/{electionRoundId}/form-submissions:byEntryV2",
            new { dataSource = "Ngo", filter, pageSize = 100 }).Items;

    [Test]
    public void AnEditKeepsEachChangedAnswer_AndMarksItChangedAfterReviewUntilReviewedAgain()
    {
        var data = Scenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;
        var submissionId = Submit(data, 1);
        var uri = $"/api/election-rounds/{er}/reviews/FormSubmission/{submissionId}";
        var reviewed = alfa.GetResponse<StageResponse[]>($"/api/election-rounds/{er}/review-stages")
            .Single(x => x.Name == "Reviewed");

        alfa.PutWithoutResponse($"{uri}:stage", new { stageId = reviewed.Id });
        Submit(data, 2);
        // Re-sending the same answers isn't a change.
        Submit(data, 2);

        var history = alfa.GetResponse<AnswerHistoryResponse>($"{uri}/answer-history");
        history.Changes.Should().ContainSingle();
        history.Changes[0].PreviousAnswer!.Value.GetProperty("value").GetInt32().Should().Be(1);
        history.Changes[0].CurrentAnswer!.Value.GetProperty("value").GetInt32().Should().Be(2);
        history.ChangedAfterReviewAt.Should().NotBeNull();
        Entries(alfa, er).Single(x => x.SubmissionId == submissionId).ChangedAfterReview.Should().BeTrue();
        Entries(alfa, er, new { field = "changedAfterReview", op = "eq", value = true })
            .Select(x => x.SubmissionId).Should().Equal(submissionId);

        // Reviewing it again clears the mark; the history stays.
        alfa.PutWithoutResponse($"{uri}:stage", new { stageId = reviewed.Id });

        var again = alfa.GetResponse<AnswerHistoryResponse>($"{uri}/answer-history");
        again.ChangedAfterReviewAt.Should().BeNull();
        again.Changes.Should().ContainSingle();
        Entries(alfa, er, new { field = "changedAfterReview", op = "eq", value = true }).Should().BeEmpty();
    }

    [Test]
    public void AnEditBeforeAnyReviewIsKeptButNotMarked_AndOtherNgosKeepTheirOwnMark()
    {
        // Bob observes for Beta, a coalition member; Alfa leads the coalition and sees Beta's submissions too.
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithMonitoringNgo(ScenarioNgos.Alfa)
                .WithMonitoringNgo(ScenarioNgos.Beta, ngo => ngo.WithMonitoringObserver(ScenarioObserver.Bob))
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithForm("Common", [ScenarioNgos.Alfa, ScenarioNgos.Beta])))
            .Please();
        var alfa = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var beta = data.NgoByName(ScenarioNgos.Beta).Admin;
        var er = data.ElectionRoundId;
        var submissionId = Submit(data, 1, ScenarioObserver.Bob);
        var uri = $"/api/election-rounds/{er}/reviews/FormSubmission/{submissionId}";
        var alfaReviewed = alfa.GetResponse<StageResponse[]>($"/api/election-rounds/{er}/review-stages")
            .Single(x => x.Name == "Reviewed");

        // Only Alfa reviewed it.
        alfa.PutWithoutResponse($"{uri}:stage", new { stageId = alfaReviewed.Id });
        Submit(data, 5, ScenarioObserver.Bob);

        alfa.GetResponse<AnswerHistoryResponse>($"{uri}/answer-history").ChangedAfterReviewAt.Should().NotBeNull();
        beta.GetResponse<AnswerHistoryResponse>($"{uri}/answer-history").ChangedAfterReviewAt.Should().BeNull();
        beta.GetResponse<AnswerHistoryResponse>($"{uri}/answer-history").Changes.Should().ContainSingle();
    }
}
