using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly Delete.Endpoint _endpoint;

    public DeleteEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        IRepository<QuickReport> repository = Substitute.For<IRepository<QuickReport>>();
        _endpoint = Factory.Create<Delete.Endpoint>(_authorizationService, repository, TestContext.Fake());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorised()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var monitoringObserverId = Guid.NewGuid();
        var attachmentId = Guid.NewGuid();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new Delete.Request
        {
            ElectionRoundId = electionRoundId,
            ObserverId = monitoringObserverId,
            Id = attachmentId
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
