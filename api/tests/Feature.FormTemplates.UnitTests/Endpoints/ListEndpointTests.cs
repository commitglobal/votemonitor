using System.Security.Claims;
using Feature.FormTemplates.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Services.Security;
using Substitute = NSubstitute.Substitute;

namespace Feature.FormTemplates.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IReadRepository<FormTemplateAggregate> _repository = Substitute.For<IReadRepository<FormTemplateAggregate>>();
    private readonly ICurrentUserRoleProvider _userRoleProvider = Substitute.For<ICurrentUserRoleProvider>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly List.Endpoint _endpoint;

    public ListEndpointTests()
    {
        _endpoint  = Factory.Create<List.Endpoint>(_repository, _userRoleProvider, _authorizationService);
    }
    
    [Fact]
    public async Task Should_Return_NotAuthorize_WhenPlatformAdmin()
    {
        // Arrange
        _userRoleProvider.IsNgoAdmin().Returns(false);
        _userRoleProvider.IsPlatformAdmin().Returns(true);

        // Act
        var request = new List.Request();
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
        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new List.Request();
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should()
            .BeOfType<Results<Ok<PagedResponse<FormTemplateSlimModel>>, NotFound>>()
            .Which.Result.Should().BeOfType<NotFound>();

        await _repository.DidNotReceiveWithAnyArgs().ListAsync(Arg.Any<ListFormTemplatesSpecification>());
        await _repository.DidNotReceiveWithAnyArgs().CountAsync(Arg.Any<ListFormTemplatesSpecification>());
    }
    
    [Fact]
    public async Task Should_UseCorrectSpecification()
    {
        // Arrange
        _userRoleProvider.IsNgoAdmin().Returns(false);
        _repository
            .ListAsync(Arg.Any<ListFormTemplatesSpecification>())
            .Returns([]);

        // Act
        var request = new List.Request();
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should()
            .BeOfType<Results<Ok<PagedResponse<FormTemplateSlimModel>>, NotFound>>();

        await _repository.Received(1).ListAsync(Arg.Any<ListFormTemplatesSpecification>());
        await _repository.Received(1).CountAsync(Arg.Any<ListFormTemplatesSpecification>());
    }

    [Fact]
    public async Task Should_ReturnMappedFormTemplates()
    {
        // Arrange
        _userRoleProvider.IsNgoAdmin().Returns(false);
        var numberOfFormTemplates = 3;
        var totalCount = 154;
        var pageSize = 100;

        var formTemplates = new FormTemplateSlimModelFaker().Generate(numberOfFormTemplates);

        _repository
            .ListAsync(Arg.Any<ListFormTemplatesSpecification>())
            .Returns(formTemplates);

        _repository
            .CountAsync(Arg.Any<ListFormTemplatesSpecification>())
            .Returns(totalCount);

        // Act
        var request = new List.Request
        {
            PageSize = pageSize,
            PageNumber = numberOfFormTemplates
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<PagedResponse<FormTemplateSlimModel>>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<PagedResponse<FormTemplateSlimModel>>>()
            .Which.Value.Should().NotBeNull();

        var pagedResult = (result.Result as Ok<PagedResponse<FormTemplateSlimModel>>)!.Value!;

        pagedResult.PageSize.Should().Be(pageSize);
        pagedResult.CurrentPage.Should().Be(numberOfFormTemplates);
        pagedResult.TotalCount.Should().Be(totalCount);
        pagedResult.Items.Should().BeEquivalentTo(formTemplates);
    }
}
