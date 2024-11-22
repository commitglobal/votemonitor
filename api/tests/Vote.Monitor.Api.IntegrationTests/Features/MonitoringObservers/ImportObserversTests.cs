using System.Web;
using Feature.MonitoringObservers;
using NSubstitute;
using NSubstitute.Extensions;
using Vote.Monitor.Api.Feature.Observer;
using Vote.Monitor.Api.IntegrationTests.Consts;
using Vote.Monitor.Api.IntegrationTests.Scenarios;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Services.EmailTemplating;
using Vote.Monitor.Core.Services.EmailTemplating.Props;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Api.IntegrationTests.Features.MonitoringObservers;

using static ApiTesting;

public class ImportObserversTests : BaseApiTestFixture
{
    [Test]
    public void ReImportedObserversShouldBeInPendingUntilTheyAcceptInvite()
    {
        // Arrange
        EmailFactory.GenerateNewUserInvitationEmail(Arg.Any<InvitationNewUserEmailProps>())
            .Returns(new EmailModel(string.Empty, string.Empty));
        EmailFactory.GenerateInvitationExistingUserEmail(Arg.Any<InvitationExistingUserEmailProps>())
            .Returns(new EmailModel(string.Empty, string.Empty));

        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithMonitoringNgo(ScenarioNgo.Alfa))
            .WithElectionRound(ScenarioElectionRound.B, er => er.WithMonitoringNgo(ScenarioNgo.Alfa))
            .Please();

        var electionRoundAId = scenarioData.ElectionRoundIdByName(ScenarioElectionRound.A);
        var electionRoundBId = scenarioData.ElectionRoundIdByName(ScenarioElectionRound.B);
        var admin = scenarioData.AdminOfNgo(ScenarioNgo.Alfa);
        var platformAdmin = scenarioData.PlatformAdmin;

        string importFile =
            $"""
             "Email","FirstName","LastName","PhoneNumber"
             "{Guid.NewGuid()}@example.com","Alice","Smith","5551111"
             """;
        // Act
        admin.PostFileWithoutResponse($"/api/election-rounds/{electionRoundAId}/monitoring-observers:import",
            importFile);
        admin.PostFileWithoutResponse($"/api/election-rounds/{electionRoundBId}/monitoring-observers:import",
            importFile);

        // Assert
        var observersElectionA =
            admin.GetResponse<PagedResponse<MonitoringObserverModel>>(
                $"/api/election-rounds/{electionRoundAId}/monitoring-observers");
        var observersElectionB =
            admin.GetResponse<PagedResponse<MonitoringObserverModel>>(
                $"/api/election-rounds/{electionRoundBId}/monitoring-observers");

        observersElectionA.Items.Should().ContainSingle();
        observersElectionB.Items.Should().ContainSingle();

        observersElectionA.Items.First().Status.Should().Be(MonitoringObserverStatus.Pending);
        observersElectionB.Items.First().Status.Should().Be(MonitoringObserverStatus.Pending);

        var observers = platformAdmin.GetResponse<PagedResponse<ObserverModel>>("/api/observers");
        observers.Items.Should().ContainSingle();
        observers.Items.First().Status.Should().Be(UserStatus.Pending);
    }

    [Test]
    public void ReImportedObserversShouldBeActiveIfTheyAcceptInvite()
    {
        // Arrange
        EmailFactory.GenerateNewUserInvitationEmail(Arg.Any<InvitationNewUserEmailProps>())
            .Returns(new EmailModel(string.Empty, string.Empty));
        EmailFactory.GenerateInvitationExistingUserEmail(Arg.Any<InvitationExistingUserEmailProps>())
            .Returns(new EmailModel(string.Empty, string.Empty));

        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithMonitoringNgo(ScenarioNgo.Alfa))
            .WithElectionRound(ScenarioElectionRound.B, er => er.WithMonitoringNgo(ScenarioNgo.Alfa))
            .Please();

        var electionRoundAId = scenarioData.ElectionRoundIdByName(ScenarioElectionRound.A);
        var electionRoundBId = scenarioData.ElectionRoundIdByName(ScenarioElectionRound.B);
        var admin = scenarioData.AdminOfNgo(ScenarioNgo.Alfa);
        var platformAdmin = scenarioData.PlatformAdmin;
        var invitationToken = string.Empty;

        string importFile =
            $"""
             "Email","FirstName","LastName","PhoneNumber"
             "{Guid.NewGuid()}@example.com","Alice","Smith","5551111"
             """;

        EmailFactory
            .GenerateNewUserInvitationEmail(Arg.Do<InvitationNewUserEmailProps>(x =>
            {
                invitationToken = GetInvitationTokenFromInviteUrl(x.AcceptUrl);
            }))
            .Returns(new EmailModel(string.Empty, string.Empty));

        admin.PostFileWithoutResponse($"/api/election-rounds/{electionRoundAId}/monitoring-observers:import",
            importFile);

        CreateClient().PostWithoutResponse("/api/auth/accept-invite",
            new { InvitationToken = invitationToken, Password = "parola123", ConfirmPassword = "parola123" });

        // Act
        admin.PostFileWithoutResponse($"/api/election-rounds/{electionRoundBId}/monitoring-observers:import",
            importFile);

        // Assert
        var observersElectionA =
            admin.GetResponse<PagedResponse<MonitoringObserverModel>>(
                $"/api/election-rounds/{electionRoundAId}/monitoring-observers");
        var observersElectionB =
            admin.GetResponse<PagedResponse<MonitoringObserverModel>>(
                $"/api/election-rounds/{electionRoundBId}/monitoring-observers");

        observersElectionA.Items.Should().ContainSingle();
        observersElectionB.Items.Should().ContainSingle();

        observersElectionA.Items.First().Status.Should().Be(MonitoringObserverStatus.Active);
        observersElectionB.Items.First().Status.Should().Be(MonitoringObserverStatus.Active);

        var observers = platformAdmin.GetResponse<PagedResponse<ObserverModel>>("/api/observers");
        observers.Items.Should().ContainSingle();
        observers.Items.First().Status.Should().Be(UserStatus.Active);
    }

    [Test]
    public void ReImportedObserversThatHavePendingAccountShouldReceiveAcceptInviteEmail()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithMonitoringNgo(ScenarioNgo.Alfa))
            .WithElectionRound(ScenarioElectionRound.B, er => er.WithMonitoringNgo(ScenarioNgo.Alfa))
            .Please();

        var aliceEmail = $"{Guid.NewGuid()}@example.com";
        string importFile =
            $"""
             "Email","FirstName","LastName","PhoneNumber"
             "{aliceEmail}","Alice","Smith","5551111"
             """;

        var electionRoundAId = scenarioData.ElectionRoundIdByName(ScenarioElectionRound.A);
        var electionRoundBId = scenarioData.ElectionRoundIdByName(ScenarioElectionRound.B);
        var admin = scenarioData.AdminOfNgo(ScenarioNgo.Alfa);
        var acceptUrl = Guid.NewGuid().ToString();

        EmailFactory
            .GenerateNewUserInvitationEmail(Arg.Any<InvitationNewUserEmailProps>())
            .Returns(new EmailModel("This is a confirm account email", acceptUrl));

        // Act
        admin.PostFileWithoutResponse($"/api/election-rounds/{electionRoundAId}/monitoring-observers:import",
            importFile);

        admin.PostFileWithoutResponse($"/api/election-rounds/{electionRoundBId}/monitoring-observers:import",
            importFile);

        // Assert
        JobService
            .Received(2)
            .EnqueueSendEmail(aliceEmail, "This is a confirm account email", acceptUrl);
    }

    [Test]
    public void ReImportedObserversThatHaveActiveAccountShouldReceiveNotificationEmail()
    {
        // Arrange
        var scenarioData = ScenarioBuilder.New(CreateClient)
            .WithNgo(ScenarioNgos.Alfa)
            .WithElectionRound(ScenarioElectionRound.A, er => er.WithMonitoringNgo(ScenarioNgo.Alfa))
            .WithElectionRound(ScenarioElectionRound.B, er => er.WithMonitoringNgo(ScenarioNgo.Alfa))
            .Please();

        var electionRoundAId = scenarioData.ElectionRoundIdByName(ScenarioElectionRound.A);
        var electionRoundBId = scenarioData.ElectionRoundIdByName(ScenarioElectionRound.B);
        var admin = scenarioData.AdminOfNgo(ScenarioNgo.Alfa);
        var invitationToken = string.Empty;

        var aliceEmail = $"{Guid.NewGuid()}@example.com";
        string importFile =
            $"""
             "Email","FirstName","LastName","PhoneNumber"
             "{aliceEmail}","Alice","Smith","5551111"
             """;

        EmailFactory
            .Configure()
            .GenerateNewUserInvitationEmail(Arg.Do<InvitationNewUserEmailProps>(x =>
            {
                invitationToken = GetInvitationTokenFromInviteUrl(x.AcceptUrl);
            }))
            .Returns(x => new EmailModel("This is a confirm account email",
                GetInvitationTokenFromInviteUrl(x.Arg<InvitationNewUserEmailProps>().AcceptUrl)));

        EmailFactory
            .GenerateInvitationExistingUserEmail(Arg.Any<InvitationExistingUserEmailProps>())
            .Returns(new EmailModel("This is a notification email", "Notification email body"));

        admin.PostFileWithoutResponse($"/api/election-rounds/{electionRoundAId}/monitoring-observers:import",
            importFile);

        CreateClient().PostWithoutResponse("/api/auth/accept-invite",
            new { InvitationToken = invitationToken, Password = "parola123", ConfirmPassword = "parola123" });

        // Act
        admin.PostFileWithoutResponse($"/api/election-rounds/{electionRoundBId}/monitoring-observers:import",
            importFile);

        // Assert
        JobService
            .Received(1)
            .EnqueueSendEmail(aliceEmail,
                "This is a confirm account email", Arg.Is(invitationToken));

        JobService
            .Received(1)
            .EnqueueSendEmail(aliceEmail, "This is a notification email", "Notification email body");
    }

    private string GetInvitationTokenFromInviteUrl(string inviteUrl)
    {
        // Parse the string as a Uri
        Uri uri = new Uri(inviteUrl);

        // Extract query parameters
        var queryParams = HttpUtility.ParseQueryString(uri.Query);

        return queryParams["invitationToken"]!;
    }
}
