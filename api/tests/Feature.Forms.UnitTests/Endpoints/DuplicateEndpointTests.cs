using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Forms.UnitTests.Endpoints;

public class DuplicateEndpointTests
{
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IRepository<Form> _repository = Substitute.For<IRepository<Form>>();
    private readonly Duplicate.Endpoint _endpoint;

    public DuplicateEndpointTests()
    {
        _endpoint = Factory.Create<Duplicate.Endpoint>(_authorizationService, _repository);
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
        var request = new Duplicate.Request();

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<Duplicate.Response>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task Should_DuplicateForm_And_Return_NewId_WhenFormExists()
    {
        // Arrange
        var form = new FormAggregateFaker().Generate();
        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormByIdSpecification>())
            .Returns(form);

        // Act
        var request = new Duplicate.Request
        {
            NgoId = Guid.NewGuid(),
            Id = form.Id
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository
            .Received(1)
            .AddAsync(Arg.Is<Form>(x => x.Status == FormStatus.Drafted));

        result
            .Should().BeOfType<Results<Ok<Duplicate.Response>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<Duplicate.Response>>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenFormDoesNotExists()
    {
        // Arrange
        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormByIdSpecification>())
            .ReturnsNullForAnyArgs();

        var request = new Duplicate.Request { Id = Guid.NewGuid() };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<Duplicate.Response>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}