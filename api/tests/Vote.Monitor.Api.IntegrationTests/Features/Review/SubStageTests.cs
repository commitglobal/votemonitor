using System.Net;
using System.Net.Http.Json;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;

namespace Vote.Monitor.Api.IntegrationTests.Features.Review;

using static ApiTesting;

public class SubStageTests : BaseApiTestFixture
{
    public record StageResponse(Guid Id, string Name, string Status, double Sequence, bool IsDefault,
        Guid? ParentStageId, string? Color, string? Icon);

    public record EntryResponse(Guid SubmissionId, Guid StageId, string StageName);

    public record PagedResponse<T>(List<T> Items, int TotalCount);

    private (ScenarioData Data, Guid AliceSubmissionId, Guid BobSubmissionId) Scenario()
    {
        var data = ScenarioBuilder.New(CreateClient)
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa, ngo => ngo
                    .WithMonitoringObserver(ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioObserver.Bob)
                    .WithForm("A", form => form
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                        .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Cluj))))
            .Please();

        var ngo = data.ElectionRound.MonitoringNgoByName(ScenarioNgos.Alfa);
        return (data,
            ngo.FormByCode("A").GetSubmissionId(ScenarioObserver.Alice, ScenarioPollingStation.Iasi),
            ngo.FormByCode("A").GetSubmissionId(ScenarioObserver.Bob, ScenarioPollingStation.Cluj));
    }

    private static string StagesUri(Guid electionRoundId) => $"/api/election-rounds/{electionRoundId}/review-stages";

    private static StageResponse[] Stages(HttpClient client, Guid electionRoundId) =>
        client.GetResponse<StageResponse[]>(StagesUri(electionRoundId));

    private static List<EntryResponse> Entries(HttpClient client, Guid electionRoundId, object filter) =>
        client.PostWithResponse<PagedResponse<EntryResponse>>(
            $"/api/election-rounds/{electionRoundId}/form-submissions:byEntryV2",
            new { dataSource = "Ngo", filter, pageSize = 100 }).Items;

    [Test]
    public void SubStagesFollowTheirParent_AndKeepTheirColourAndIcon()
    {
        var (data, _, _) = Scenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;
        var inReview = Stages(admin, er).Single(x => x.Name == "In review");

        // The requested status is ignored: a sub-stage takes its parent's.
        var calling = admin.PostWithResponse<StageResponse>(StagesUri(er),
            new { name = "Calling", status = "Reviewed", parentStageId = inReview.Id, color = "orange", icon = "phone" });

        calling.Status.Should().Be("InReview");
        calling.ParentStageId.Should().Be(inReview.Id);
        (calling.Color, calling.Icon).Should().Be(("orange", "phone"));

        // Moving the parent to another status moves its sub-stages with it.
        admin.PutWithoutResponse($"{StagesUri(er)}/{inReview.Id}", new { name = "In review", status = "Reviewed" });

        var stages = Stages(admin, er);
        stages.Single(x => x.Id == calling.Id).Status.Should().Be("Reviewed");
        // Listed right after their parent.
        stages.Select(x => x.Name).Should().ContainInConsecutiveOrder("In review", "Calling");
    }

    [Test]
    public void SubStagesAreOneLevelDeep_AndAParentWithSubStagesCantBeDeleted()
    {
        var (data, _, _) = Scenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;
        var stages = Stages(admin, er);
        var inReview = stages.Single(x => x.Name == "In review");
        var reviewed = stages.Single(x => x.Name == "Reviewed");
        var calling = admin.PostWithResponse<StageResponse>(StagesUri(er),
            new { name = "Calling", status = "InReview", parentStageId = inReview.Id });

        var grandchild = admin.PostAsJsonAsync(StagesUri(er),
            new { name = "Deeper", status = "InReview", parentStageId = calling.Id }).GetAwaiter().GetResult();
        var parentUnderOther = admin.PutAsJsonAsync($"{StagesUri(er)}/{inReview.Id}",
            new { name = "In review", status = "InReview", parentStageId = reviewed.Id }).GetAwaiter().GetResult();
        var badColour = admin.PostAsJsonAsync(StagesUri(er),
            new { name = "Odd", status = "InReview", color = "#ff0000" }).GetAwaiter().GetResult();
        var deleteParent = admin.DeleteAsync($"{StagesUri(er)}/{inReview.Id}?moveTo={reviewed.Id}")
            .GetAwaiter().GetResult();

        grandchild.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        parentUnderOther.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        badColour.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        deleteParent.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Test]
    public void FilteringByAStageMatchesItsSubStages_AndASubStageOnlyItself()
    {
        var (data, aliceSubmissionId, bobSubmissionId) = Scenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;
        var inReview = Stages(admin, er).Single(x => x.Name == "In review");
        var calling = admin.PostWithResponse<StageResponse>(StagesUri(er),
            new { name = "Calling", status = "InReview", parentStageId = inReview.Id });
        admin.PutWithoutResponse($"/api/election-rounds/{er}/reviews/FormSubmission/{aliceSubmissionId}:stage",
            new { stageId = calling.Id });
        admin.PutWithoutResponse($"/api/election-rounds/{er}/reviews/FormSubmission/{bobSubmissionId}:stage",
            new { stageId = inReview.Id });

        Entries(admin, er, new { field = "stageId", op = "eq", value = inReview.Id })
            .Select(x => x.SubmissionId).Should().BeEquivalentTo([aliceSubmissionId, bobSubmissionId]);
        Entries(admin, er, new { field = "stageId", op = "in", values = new[] { calling.Id } })
            .Select(x => x.SubmissionId).Should().Equal(aliceSubmissionId);
        Entries(admin, er, new { field = "stageId", op = "ne", value = calling.Id })
            .Select(x => x.SubmissionId).Should().Equal(bobSubmissionId);
    }

    [Test]
    public void TheListSearchMatchesObserverNameEmailAndSubmissionId()
    {
        var (data, aliceSubmissionId, bobSubmissionId) = Scenario();
        var admin = data.NgoByName(ScenarioNgos.Alfa).Admin;
        var er = data.ElectionRoundId;
        var search = (string text) => Entries(admin, er, new { field = "searchText", op = "contains", value = text })
            .Select(x => x.SubmissionId).ToList();

        // Any part of the id, any case; observers are named "Observer <name>-<guid>".
        search(aliceSubmissionId.ToString()[4..12].ToUpperInvariant()).Should().Equal(aliceSubmissionId);
        search("BOB-").Should().Equal(bobSubmissionId);
        search("example.org").Should().BeEquivalentTo([aliceSubmissionId, bobSubmissionId]);
    }
}
