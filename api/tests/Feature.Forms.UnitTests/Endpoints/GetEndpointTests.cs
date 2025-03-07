using System.Security.Claims;
using Feature.Forms.Models;
using Microsoft.AspNetCore.Authorization;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Forms.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IReadRepository<Form> _repository = Substitute.For<IReadRepository<Form>>();
    private readonly IReadRepository<Coalition> _coalitionRepository = Substitute.For<IReadRepository<Coalition>>();
    private readonly Get.Endpoint _endpoint;

    public GetEndpointTests()
    {
        _endpoint = Factory.Create<Get.Endpoint>(_authorizationService, _repository, _coalitionRepository);
        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenUserIsNotAuthorized()
    {
        // Arrange
        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new Get.Request();

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<FormFullModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task Should_ReturnForm_WhenFormExists()
    {
        // Arrange
        var form = new FormAggregateFaker().Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormByIdSpecification>())
            .Returns(form);

        // Act
        var request = new Get.Request { Id = form.Id };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<FormFullModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<FormFullModel>>()
            .Which.Value.Should().BeEquivalentTo(form, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenFormDoesNotExist()
    {
        // Arrange
        var request = new Get.Request { Id = Guid.NewGuid() };

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormByIdSpecification>())
            .ReturnsNullForAnyArgs();

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<FormFullModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
