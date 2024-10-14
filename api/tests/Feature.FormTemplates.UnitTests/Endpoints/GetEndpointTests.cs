using System.Security.Claims;
using Feature.FormTemplates.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.Security;

namespace Feature.FormTemplates.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IReadRepository<FormTemplateAggregate> _repository =
        Substitute.For<IReadRepository<FormTemplateAggregate>>();

    private readonly ICurrentUserRoleProvider _userRoleProvider = Substitute.For<ICurrentUserRoleProvider>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly Get.Endpoint _endpoint;

    public GetEndpointTests()
    {
        _endpoint = Factory.Create<Get.Endpoint>(_repository, _userRoleProvider, _authorizationService);
    }

    [Fact]
    public async Task Should_Return_NotAuthorize_WhenPlatformAdmin()
    {
        // Arrange
        _userRoleProvider.IsNgoAdmin().Returns(false);
        _userRoleProvider.IsPlatformAdmin().Returns(true);

        // Act
        var request = new Get.Request();
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _authorizationService
            .DidNotReceiveWithAnyArgs()
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(),
                Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>());
    }

    [Fact]
    public async Task Should_Return_NotFound_When_User_Not_Authorized()
    {
        // Arrange
        _userRoleProvider.IsNgoAdmin().Returns(true);
        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new Get.Request();
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should()
            .BeOfType<Results<Ok<FormTemplateFullModel>, NotFound>>()
            .Which.Result.Should().BeOfType<NotFound>();

        await _repository.DidNotReceiveWithAnyArgs().ListAsync(Arg.Any<ListFormTemplatesSpecification>());
        await _repository.DidNotReceiveWithAnyArgs().CountAsync(Arg.Any<ListFormTemplatesSpecification>());
    }

    [Fact]
    public async Task Should_ReturnFormTemplate_WhenFormTemplateExists()
    {
        // Arrange
        var formTemplate = new FormTemplateAggregateFaker().Generate();
        _userRoleProvider.IsNgoAdmin().Returns(false);
        _userRoleProvider.IsPlatformAdmin().Returns(true);

        _repository
            .SingleOrDefaultAsync(Arg.Any<GetFormTemplateSpecification>())
            .Returns(formTemplate);

        // Act
        var request = new Get.Request { Id = formTemplate.Id };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<FormTemplateFullModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<FormTemplateFullModel>>()
            .Which.Value.Should().BeEquivalentTo(formTemplate, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenFormTemplateDoesNotExist()
    {
        // Arrange 
        _userRoleProvider.IsNgoAdmin().Returns(false);
        _userRoleProvider.IsPlatformAdmin().Returns(true);
        // Act
        var request = new Get.Request { Id = Guid.NewGuid() };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<FormTemplateFullModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}