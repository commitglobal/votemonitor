using System.Security.Claims;
using Feature.QuickReports.Get;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.QuickReports.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly Get.Endpoint _endpoint;

    public GetEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        INpgsqlConnectionFactory dbConnectionFactory = Substitute.For<INpgsqlConnectionFactory>();
        IFileStorageService fileStorageService = Substitute.For<IFileStorageService>();
        ICurrentUserRoleProvider userRoleProvider = Substitute.For<ICurrentUserRoleProvider>();
        ICurrentUserProvider userProvider = Substitute.For<ICurrentUserProvider>();
        
        _endpoint = Factory.Create<Get.Endpoint>(_authorizationService,
            dbConnectionFactory,
            fileStorageService,
            userRoleProvider,
            userProvider);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorised()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var quickReportId = Guid.NewGuid();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Id = quickReportId
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<QuickReportDetailedModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
