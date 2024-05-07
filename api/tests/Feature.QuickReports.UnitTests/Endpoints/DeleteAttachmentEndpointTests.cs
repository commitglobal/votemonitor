using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Feature.QuickReports.UnitTests.Endpoints;

public class DeleteAttachmentEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly DeleteAttachment.Endpoint _endpoint;

    public DeleteAttachmentEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _endpoint = Factory.Create<DeleteAttachment.Endpoint>(_authorizationService, TestContext.Fake());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorised()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new DeleteAttachment.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
