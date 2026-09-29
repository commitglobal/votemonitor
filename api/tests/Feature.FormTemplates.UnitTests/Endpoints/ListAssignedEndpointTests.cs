using System.Security.Claims;
using Authorization.Policies.Requirements;
using Feature.FormTemplates.ListAssignedTemplates;
using Feature.FormTemplates.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Entities.ElectionRoundFormTemplateAggregate;

namespace Feature.FormTemplates.UnitTests.Endpoints;

public class ListAssignedEndpointTests
{
    private readonly IReadRepository<ElectionRoundFormTemplate> _electionRoundFormTemplateRepository;
    private readonly ICurrentUserRoleProvider _userRoleProvider;
    private readonly IAuthorizationService _authorizationService;
    private readonly Endpoint _endpoint;

    public ListAssignedEndpointTests()
    {
        _electionRoundFormTemplateRepository = Substitute.For<IReadRepository<ElectionRoundFormTemplate>>();
        _userRoleProvider = Substitute.For<ICurrentUserRoleProvider>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _endpoint = Factory.Create<Endpoint>
        (_electionRoundFormTemplateRepository,
            _userRoleProvider,
            _authorizationService);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_NgoAdmin_is_False()
    {
        // Arrange
        _userRoleProvider.IsNgoAdmin().Returns(false);

        // Act
        var request = new Request();
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _authorizationService
            .DidNotReceiveWithAnyArgs()
            .AuthorizeAsync(
                Arg.Any<ClaimsPrincipal>(),
                Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>());
    }

    [Fact]
    public async Task Should_Return_NotFound_When_NgoAdmin_is_True_and_Authorization_Fails()
    {
        // Arrange
        _userRoleProvider.IsNgoAdmin().Returns(true);
        _authorizationService.AuthorizeAsync(
                Arg.Any<ClaimsPrincipal>(),
                Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new Request();
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result.Should()
            .BeOfType<Results<Ok<List<FormTemplateSlimModel>>, NotFound>>()
            .Which.Result.Should()
            .BeOfType<NotFound>();
    }

    [Fact]
    public async Task Should_Return_Ok_With_Empty_List_When_No_Assigned_Templates()
    {
        // Arrange
        _userRoleProvider.IsNgoAdmin().Returns(true);

        _authorizationService.AuthorizeAsync(
                Arg.Any<ClaimsPrincipal>(),
                Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _electionRoundFormTemplateRepository
            .ListAsync(Arg.Any<ListAssignedFormTemplateSpecification>())
            .Returns([]);

        // Act
        var request = new Request();
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result.Should()
            .BeOfType<Results<Ok<List<FormTemplateSlimModel>>, NotFound>>()
            .Which.Result.Should()
            .BeOfType<Ok<List<FormTemplateSlimModel>>>()
            .Which.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Return_Ok_With_List_When_Assigned_Templates_Exist()
    {
        // Arrange
        _userRoleProvider.IsNgoAdmin().Returns(true);

        _authorizationService.AuthorizeAsync(
                Arg.Any<ClaimsPrincipal>(),
                Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        var formTemplates = new FormTemplateSlimModelFaker().Generate(3);

        _electionRoundFormTemplateRepository
            .ListAsync(Arg.Any<ListAssignedFormTemplateSpecification>())
            .Returns(formTemplates);

        // Act
        var request = new Request();
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<List<FormTemplateSlimModel>>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<List<FormTemplateSlimModel>>>()
            .Which.Value.Should().BeEquivalentTo(formTemplates);
    }
}
