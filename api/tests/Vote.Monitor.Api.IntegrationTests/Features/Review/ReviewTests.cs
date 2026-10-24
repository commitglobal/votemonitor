using System.Net;
using System.Net.Http.Json;
using Module.Answers.Requests;
using Module.Forms.Requests;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Vote.Monitor.Api.IntegrationTests.Features.Review;

using static ApiTesting;

public class ReviewTests : BaseApiTestFixture
{
    private const string Sam = "Sam";
    private const string Tom = "Tom";

    public record StageResponse(Guid Id, string Name, string Status, double Sequence, bool IsDefault);

    public record ReviewerResponse(Guid Id, string DisplayName, string Status);

    public record ReviewResponse(StageResponse Stage, DateTime? StageChangedAt, DateTime? ReviewedAt,
        ReviewerResponse[] Reviewers);

    public record HistoryEntryResponse(string Kind, string ActorName, string? FromStageName, string? ToStageName,
        Guid? ReviewerId, string? FollowUpStatus);

    public record InboxItemResponse(Guid Id, string Reason, DateTime? ReadAt, Guid RecordId, HistoryEntryResponse Activity);

    public record PagedResponse<T>(List<T> Items, int TotalCount);

    public record UnreadCountResponse(int Count);

    public record EntryResponse(Guid SubmissionId, Guid StageId, string StageName, string ReviewStatus,
        Guid[] ReviewerIds);

    public record StageCountResponse(Guid StageId, int Count);

    /// <summary>Alfa leads a coalition with Beta; Alice (Alfa) and Bob (Beta) each submitted the common form.</summary>
    private (ScenarioData Data, Guid AliceSubmissionId, Guid BobSubmissionId) CoalitionScenario()
    {
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa, ngo => ngo.WithStaff(Sam).WithStaff(Tom))
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Cluj)
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

    private static string Review(Guid electionRoundId, Guid submissionId) =>
        $"/api/election-rounds/{electionRoundId}/reviews/FormSubmission/{submissionId}";

    private static StageResponse[] Stages(HttpClient client, Guid electionRoundId) =>
        client.GetResponse<StageResponse[]>($"/api/election-rounds/{electionRoundId}/review-stages");

    private static StageResponse StageNamed(HttpClient client, Guid electionRoundId, string name) =>
        Stages(client, electionRoundId).Single(x => x.Name == name);

    private static HttpStatusCode Put(HttpClient client, string uri, object body) =>
        client.PutAsJsonAsync(uri, body).GetAwaiter().GetResult().StatusCode;

    private static List<EntryResponse> Entries(HttpClient client, Guid electionRoundId, string dataSource,
        object? filter = null) =>
        client.PostWithResponse<PagedResponse<EntryResponse>>(
            $"/api/election-rounds/{electionRoundId}/form-submissions:byEntryV2",
            new { dataSource, filter, pageSize = 100 }).Items;

    /// <summary>Submits (or, at a place where the observer already did, edits) the common form, answering its number question.</summary>
    private static Guid Submit(ScenarioData data, ScenarioObserver observer, ScenarioPollingStation pollingStation,
        int number) =>
        data.ObserverByName(observer).PostWithResponse<ResponseWithId>(
            $"/api/election-rounds/{data.ElectionRoundId}/form-submissions",
            new FormSubmissionRequest
            {
                PollingStationId = data.ElectionRound.PollingStationByName(pollingStation),
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

    [Test]
    public void EveryMonitoringNgoStartsWithDefaultStagesAndReportsStartInTheDefaultOne()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;

        var stages = Stages(admin, data.ElectionRoundId);
        var review = admin.GetResponse<ReviewResponse>(Review(data.ElectionRoundId, aliceSubmissionId));

        stages.Select(x => (x.Name, x.Status, x.IsDefault)).Should().Equal(
            ("Not reviewed", "NotReviewed", true), ("In review", "InReview", false), ("Reviewed", "Reviewed", false));
        review.Stage.Name.Should().Be("Not reviewed");
        review.StageChangedAt.Should().BeNull();
        review.Reviewers.Should().BeEmpty();
    }

    [Test]
    public void ReviewerStaffAreNotifiedAndCanMoveTheReport_OtherStaffCannot()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var uri = Review(data.ElectionRoundId, aliceSubmissionId);
        var reviewed = StageNamed(alfa.Admin, data.ElectionRoundId, "Reviewed");

        alfa.Admin.PutWithoutResponse($"{uri}:reviewers", new { userIds = new[] { alfa.StaffIdByName(Sam) } });

        var inbox = alfa.StaffByName(Sam).GetResponse<PagedResponse<InboxItemResponse>>("/api/inbox");
        inbox.Items.Should().ContainSingle(x => x.Reason == "AddedAsReviewer" && x.RecordId == aliceSubmissionId);
        alfa.Admin.GetResponse<UnreadCountResponse>("/api/inbox:unread-count").Count.Should().Be(0);

        Put(alfa.StaffByName(Tom), $"{uri}:stage", new { stageId = reviewed.Id }).Should().Be(HttpStatusCode.Forbidden);
        Put(alfa.StaffByName(Tom), $"{uri}:reviewers", new { userIds = new[] { alfa.StaffIdByName(Sam), alfa.AdminId } })
            .Should().Be(HttpStatusCode.Forbidden);

        alfa.StaffByName(Sam).PutWithoutResponse($"{uri}:stage", new { stageId = reviewed.Id });

        var review = alfa.Admin.GetResponse<ReviewResponse>(uri);
        review.Stage.Name.Should().Be("Reviewed");
        review.ReviewedAt.Should().NotBeNull();
        review.Reviewers.Select(x => x.Id).Should().Equal(alfa.StaffIdByName(Sam));

        var history = alfa.Admin.GetResponse<HistoryEntryResponse[]>($"{uri}/history");
        history.Select(x => x.Kind).Should().Equal("StageChanged", "ReviewerAdded");
        history[0].FromStageName.Should().Be("Not reviewed");
        history[0].ToStageName.Should().Be("Reviewed");
    }

    [Test]
    public void CoalitionMembersAndLeaderKeepSeparateReviewsOnTheSameReport()
    {
        var (data, _, bobSubmissionId) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var beta = data.NgoByName(ScenarioNgos.Beta);
        var uri = Review(data.ElectionRoundId, bobSubmissionId);

        beta.Admin.PutWithoutResponse($"{uri}:stage",
            new { stageId = StageNamed(beta.Admin, data.ElectionRoundId, "In review").Id });
        beta.Admin.PutWithoutResponse($"{uri}:reviewers", new { userIds = new[] { beta.AdminId } });
        alfa.Admin.PutWithoutResponse($"{uri}:stage",
            new { stageId = StageNamed(alfa.Admin, data.ElectionRoundId, "Reviewed").Id });
        alfa.Admin.PutWithoutResponse($"{uri}:reviewers", new { userIds = new[] { alfa.StaffIdByName(Sam) } });

        var betaView = beta.Admin.GetResponse<ReviewResponse>(uri);
        var alfaView = alfa.Admin.GetResponse<ReviewResponse>(uri);
        betaView.Stage.Name.Should().Be("In review");
        betaView.Reviewers.Select(x => x.Id).Should().Equal(beta.AdminId);
        alfaView.Stage.Name.Should().Be("Reviewed");
        alfaView.Reviewers.Select(x => x.Id).Should().Equal(alfa.StaffIdByName(Sam));

        var betaHistory = beta.Admin.GetResponse<HistoryEntryResponse[]>($"{uri}/history");
        var alfaHistory = alfa.Admin.GetResponse<HistoryEntryResponse[]>($"{uri}/history");
        betaHistory.Should().HaveCount(2);
        betaHistory.Select(x => x.ToStageName).Should().NotContain("Reviewed");
        betaHistory.Select(x => x.ReviewerId).Should().NotContain(alfa.StaffIdByName(Sam));
        alfaHistory.Should().HaveCount(2);
        alfaHistory.Select(x => x.ToStageName).Should().NotContain("In review");

        var betaEntry = Entries(beta.Admin, data.ElectionRoundId, "Ngo").Single(x => x.SubmissionId == bobSubmissionId);
        var alfaEntry = Entries(alfa.Admin, data.ElectionRoundId, "Coalition")
            .Single(x => x.SubmissionId == bobSubmissionId);
        betaEntry.StageName.Should().Be("In review");
        betaEntry.ReviewerIds.Should().Equal(beta.AdminId);
        alfaEntry.StageName.Should().Be("Reviewed");
        alfaEntry.ReviewerIds.Should().Equal(alfa.StaffIdByName(Sam));

        Put(alfa.Admin, $"{uri}:reviewers", new { userIds = new[] { alfa.StaffIdByName(Sam), beta.AdminId } })
            .Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public void ListFiltersByReviewStatusAndReviewer_AndCountsMatch()
    {
        var (data, aliceSubmissionId, bobSubmissionId) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        alfa.Admin.PutWithoutResponse($"{Review(data.ElectionRoundId, aliceSubmissionId)}:stage",
            new { stageId = StageNamed(alfa.Admin, data.ElectionRoundId, "Reviewed").Id });
        alfa.Admin.PutWithoutResponse($"{Review(data.ElectionRoundId, bobSubmissionId)}:reviewers",
            new { userIds = new[] { alfa.AdminId } });

        var reviewed = Entries(alfa.Admin, data.ElectionRoundId, "Coalition",
            new { op = "in", field = "reviewStatus", values = new[] { "Reviewed" } });
        var mine = Entries(alfa.Admin, data.ElectionRoundId, "Coalition",
            new { op = "in", field = "reviewerId", values = new[] { alfa.AdminId } });
        var unassigned = Entries(alfa.Admin, data.ElectionRoundId, "Coalition",
            new { op = "eq", field = "hasReviewer", value = false });
        var counts = alfa.Admin.PostWithResponse<StageCountResponse[]>(
            $"/api/election-rounds/{data.ElectionRoundId}/reviews/FormSubmission:stage-counts",
            new { dataSource = "Coalition" });

        reviewed.Select(x => x.SubmissionId).Should().Equal(aliceSubmissionId);
        mine.Select(x => x.SubmissionId).Should().Equal(bobSubmissionId);
        unassigned.Select(x => x.SubmissionId).Should().Equal(aliceSubmissionId);
        counts.Select(x => x.Count).Sum().Should().Be(2);
        counts.Should().Contain(x => x.StageId == StageNamed(alfa.Admin, data.ElectionRoundId, "Reviewed").Id && x.Count == 1);
    }

    public record QuickReportEntryResponse(Guid Id, string StageName, Guid[] ReviewerIds);

    [Test]
    public void V1ListsReturnTheReviewColumnsAndFilterOnThem()
    {
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo
                    .WithMonitoringObserver(ScenarioObserver.Alice)
                    .WithForm("A", form => form
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Cluj)))
                .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Cluj))
            .Please();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var adminId = data.NgoByName(ScenarioNgos.Alfa).AdminId;
        var er = data.ElectionRoundId;
        var reviewed = StageNamed(admin, er, "Reviewed");
        var iasiSubmission = data.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa).FormByCode("A")
            .GetSubmissionId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);
        var iasiQuickReport = data.ElectionRound.GetQuickReportId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi);

        admin.PutWithoutResponse($"{Review(er, iasiSubmission)}:stage", new { stageId = reviewed.Id });
        admin.PutWithoutResponse($"/api/election-rounds/{er}/reviews/QuickReport/{iasiQuickReport}:reviewers",
            new { userIds = new[] { adminId } });

        var allSubmissions = admin.GetResponse<PagedResponse<EntryResponse>>(
            $"/api/election-rounds/{er}/form-submissions:byEntry?dataSource=Ngo");
        var reviewedSubmissions = admin.GetResponse<PagedResponse<EntryResponse>>(
            $"/api/election-rounds/{er}/form-submissions:byEntry?dataSource=Ngo&reviewStatus=Reviewed");
        var allQuickReports = admin.GetResponse<PagedResponse<QuickReportEntryResponse>>(
            $"/api/election-rounds/{er}/quick-reports?dataSource=Ngo");
        var myQuickReports = admin.GetResponse<PagedResponse<QuickReportEntryResponse>>(
            $"/api/election-rounds/{er}/quick-reports?dataSource=Ngo&reviewerId={adminId}");
        var unassignedQuickReports = admin.GetResponse<PagedResponse<QuickReportEntryResponse>>(
            $"/api/election-rounds/{er}/quick-reports?dataSource=Ngo&hasReviewer=false");

        allSubmissions.Items.Should().HaveCount(2);
        allSubmissions.Items.Single(x => x.SubmissionId == iasiSubmission).StageName.Should().Be("Reviewed");
        allSubmissions.Items.Single(x => x.SubmissionId != iasiSubmission).StageName.Should().Be("Not reviewed");
        reviewedSubmissions.Items.Select(x => x.SubmissionId).Should().Equal(iasiSubmission);
        allQuickReports.Items.Should().HaveCount(2).And.OnlyContain(x => x.StageName == "Not reviewed");
        myQuickReports.Items.Select(x => x.Id).Should().Equal(iasiQuickReport);
        myQuickReports.Items[0].ReviewerIds.Should().Equal(adminId);
        unassignedQuickReports.Items.Select(x => x.Id).Should().NotContain(iasiQuickReport).And.HaveCount(1);
    }

    [Test]
    public void IncidentAndCitizenV1ListsAcceptTheReviewFilters()
    {
        var (data, _, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}:enableCitizenReporting",
            new { ngoId = alfa.NgoId });
        var filters = $"stageId={StageNamed(alfa.Admin, er, "Not reviewed").Id}&reviewStatus=NotReviewed&reviewerId={alfa.AdminId}&hasReviewer=true";

        var incidents = alfa.Admin.GetAsync($"/api/election-rounds/{er}/incident-reports:byEntry?{filters}")
            .GetAwaiter().GetResult();
        var citizen = alfa.Admin.GetAsync($"/api/election-rounds/{er}/citizen-reports:byEntry?{filters}")
            .GetAwaiter().GetResult();

        incidents.StatusCode.Should().Be(HttpStatusCode.OK, incidents.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        citizen.StatusCode.Should().Be(HttpStatusCode.OK, citizen.Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }

    [Test]
    public void SwitchingTheDefaultStageDoesNotMoveExistingReports()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var inReview = StageNamed(admin, data.ElectionRoundId, "In review");

        // No body and no Content-Type, as browsers send body-less actions.
        var response = admin
            .PostAsync($"/api/election-rounds/{data.ElectionRoundId}/review-stages/{inReview.Id}:default", null)
            .GetAwaiter().GetResult();

        response.IsSuccessStatusCode.Should().BeTrue();

        StageNamed(admin, data.ElectionRoundId, "In review").IsDefault.Should().BeTrue();
        StageNamed(admin, data.ElectionRoundId, "Not reviewed").IsDefault.Should().BeFalse();
        admin.GetResponse<ReviewResponse>(Review(data.ElectionRoundId, aliceSubmissionId)).Stage.Name
            .Should().Be("Not reviewed");
    }

    [Test]
    public void DeletingAStageInUseMovesItsReportsAndRecordsTheMove()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var calling = admin.PostWithResponse<StageResponse>($"/api/election-rounds/{data.ElectionRoundId}/review-stages",
            new { name = "Calling the observer", status = "InReview" });
        var reviewed = StageNamed(admin, data.ElectionRoundId, "Reviewed");
        var uri = Review(data.ElectionRoundId, aliceSubmissionId);
        admin.PutWithoutResponse($"{uri}:stage", new { stageId = calling.Id });

        var withoutTarget = admin.DeleteAsync($"/api/election-rounds/{data.ElectionRoundId}/review-stages/{calling.Id}")
            .GetAwaiter().GetResult();
        var withTarget = admin
            .DeleteAsync($"/api/election-rounds/{data.ElectionRoundId}/review-stages/{calling.Id}?moveTo={reviewed.Id}")
            .GetAwaiter().GetResult();

        withoutTarget.StatusCode.Should().Be(HttpStatusCode.Conflict);
        withTarget.StatusCode.Should().Be(HttpStatusCode.NoContent);
        admin.GetResponse<ReviewResponse>(uri).Stage.Name.Should().Be("Reviewed");
        admin.GetResponse<HistoryEntryResponse[]>($"{uri}/history")[0].FromStageName.Should().Be("Calling the observer");
    }

    [Test]
    public void ReviewWorkContinuesInAnArchivedRound_StageDefinitionsAreLocked()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var er = data.ElectionRoundId;
        var uri = Review(er, aliceSubmissionId);
        var reviewed = StageNamed(alfa.Admin, er, "Reviewed");
        data.PlatformAdmin.PostWithoutResponse($"/api/election-rounds/{er}:archive", new { });

        Put(alfa.Admin, $"{uri}:reviewers", new { userIds = new[] { alfa.StaffIdByName(Sam) } })
            .Should().Be(HttpStatusCode.NoContent);
        Put(alfa.StaffByName(Sam), $"{uri}:stage", new { stageId = reviewed.Id }).Should().Be(HttpStatusCode.NoContent);
        var createStage = alfa.Admin.PostAsJsonAsync($"/api/election-rounds/{er}/review-stages",
            new { name = "Calling the observer", status = "InReview" }).GetAwaiter().GetResult();

        var review = alfa.Admin.GetResponse<ReviewResponse>(uri);
        review.Stage.Name.Should().Be("Reviewed");
        review.Reviewers.Select(x => x.Id).Should().Equal(alfa.StaffIdByName(Sam));
        createStage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public void FollowUpStatusChangesAppearInTheHistory()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;

        admin.PutWithoutResponse(
            $"/api/election-rounds/{data.ElectionRoundId}/form-submissions/{aliceSubmissionId}:status",
            new { FollowUpStatus = SubmissionFollowUpStatus.NeedsFollowUp.ToString() });

        var history = admin.GetResponse<HistoryEntryResponse[]>($"{Review(data.ElectionRoundId, aliceSubmissionId)}/history");
        history.Should().ContainSingle(x => x.Kind == "FollowUpStatusChanged" && x.FollowUpStatus == "NeedsFollowUp");
    }

    [Test]
    public void AnObserversChangeReturnsTheReportToEachNgosDefaultStage_ResendingItUnchangedDoesNot()
    {
        var (data, _, bobSubmissionId) = CoalitionScenario();
        var alfa = data.NgoByName(ScenarioNgos.Alfa);
        var beta = data.NgoByName(ScenarioNgos.Beta);
        var er = data.ElectionRoundId;
        var uri = Review(er, bobSubmissionId);
        Submit(data, ScenarioObserver.Bob, ScenarioPollingStation.Iasi, 5);

        alfa.Admin.PutWithoutResponse($"{uri}:stage", new { stageId = StageNamed(alfa.Admin, er, "Reviewed").Id });
        alfa.Admin.PutWithoutResponse($"{uri}:reviewers", new { userIds = new[] { alfa.StaffIdByName(Sam) } });
        beta.Admin.PutWithoutResponse($"{uri}:stage", new { stageId = StageNamed(beta.Admin, er, "In review").Id });

        Submit(data, ScenarioObserver.Bob, ScenarioPollingStation.Iasi, 5);

        alfa.Admin.GetResponse<ReviewResponse>(uri).Stage.Name.Should().Be("Reviewed");
        beta.Admin.GetResponse<ReviewResponse>(uri).Stage.Name.Should().Be("In review");

        Submit(data, ScenarioObserver.Bob, ScenarioPollingStation.Iasi, 6);

        var alfaReview = alfa.Admin.GetResponse<ReviewResponse>(uri);
        alfaReview.Stage.Name.Should().Be("Not reviewed");
        alfaReview.ReviewedAt.Should().BeNull();
        alfaReview.Reviewers.Select(x => x.Id).Should().Equal(alfa.StaffIdByName(Sam));
        beta.Admin.GetResponse<ReviewResponse>(uri).Stage.Name.Should().Be("Not reviewed");

        var history = alfa.Admin.GetResponse<HistoryEntryResponse[]>($"{uri}/history");
        history[0].Should().BeEquivalentTo(new
        {
            Kind = "ReturnedToDefaultStage",
            ActorName = null,
            FromStageName = "Reviewed",
            ToStageName = "Not reviewed"
        });
        alfa.StaffByName(Sam).GetResponse<PagedResponse<InboxItemResponse>>("/api/inbox").Items
            .Should().Contain(x => x.Reason == "ReviewingRecordChanged" && x.Activity.Kind == "ReturnedToDefaultStage");
    }

    [Test]
    public void AnObserversNoteReturnsTheReportToTheDefaultStage_ResendingItUnchangedDoesNot()
    {
        var (data, aliceSubmissionId, _) = CoalitionScenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;
        var uri = Review(er, aliceSubmissionId);
        var reviewed = StageNamed(admin, er, "Reviewed");
        var noteUri = $"/api/election-rounds/{er}/form-submissions/{aliceSubmissionId}/notes/{Guid.NewGuid()}";
        var note = new
        {
            questionId = data.ElectionRound.Coalition.Form.Questions.OfType<NumberQuestionRequest>().Single().Id,
            text = "Queue outside",
            lastUpdatedAt = DateTime.UtcNow
        };

        admin.PutWithoutResponse($"{uri}:stage", new { stageId = reviewed.Id });
        data.ObserverByName(ScenarioObserver.Alice).PostWithoutResponse(noteUri, note);

        admin.GetResponse<ReviewResponse>(uri).Stage.Name.Should().Be("Not reviewed");

        admin.PutWithoutResponse($"{uri}:stage", new { stageId = reviewed.Id });
        data.ObserverByName(ScenarioObserver.Alice).PostWithoutResponse(noteUri, note);

        admin.GetResponse<ReviewResponse>(uri).Stage.Name.Should().Be("Reviewed");
        admin.GetResponse<HistoryEntryResponse[]>($"{uri}/history")
            .Count(x => x.Kind == "ReturnedToDefaultStage").Should().Be(1);
    }
}
