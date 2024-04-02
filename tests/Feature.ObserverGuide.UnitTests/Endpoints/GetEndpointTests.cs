using System.Security.Claims;
using FastEndpoints;
using Feature.ObserverGuide.Get;
using Feature.ObserverGuide.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.ObserverGuide.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IReadRepository<ObserverGuideAggregate> _repository;
    private readonly IAuthorizationService _authorizationService;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly Endpoint _endpoint;

    public GetEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<ObserverGuideAggregate>>();
        var fileStorageService = Substitute.For<IFileStorageService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _currentUserProvider = Substitute.For<ICurrentUserProvider>();

        _endpoint = Factory.Create<Endpoint>(_authorizationService,
            _currentUserProvider,
            _repository,
            fileStorageService);
    }

    [Fact]
    public async Task ShouldReturnOkWithObserverGuideModel_WhenObserverGuideExists_UserIsObserver()
    {
        // Arrange
        var observerGuideId = Guid.NewGuid();
        var fileName = "photo.jpg";

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeObserverGuide = new ObserverGuideAggregateFaker(observerGuideId, fileName).Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _currentUserProvider.IsObserver().Returns(true);

        _repository.FirstOrDefaultAsync(Arg.Any<GetObserverGuideSpecification>())
            .Returns(fakeObserverGuide);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Id = observerGuideId
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<ObserverGuideModel>>();
        model.Value!.FileName.Should().Be(fakeObserverGuide.FileName);
        model.Value.Id.Should().Be(fakeObserverGuide.Id);
    }

    [Fact]
    public async Task ShouldReturnOkWithObserverGuideModel_WhenObserverGuideExists_UserIsNgoAdmin()
    {
        // Arrange
        var observerGuideId = Guid.NewGuid();
        var fileName = "photo.jpg";

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeObserverGuide = new ObserverGuideAggregateFaker(observerGuideId, fileName).Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _currentUserProvider.IsObserver().Returns(false);
        _currentUserProvider.IsNgoAdmin().Returns(true);

        _repository.FirstOrDefaultAsync(Arg.Any<GetObserverGuideForNgoAdminSpecification>())
            .Returns(fakeObserverGuide);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Id = observerGuideId
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<ObserverGuideModel>>();
        model.Value!.FileName.Should().Be(fakeObserverGuide.FileName);
        model.Value.Id.Should().Be(fakeObserverGuide.Id);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorised()
    {
        // Arrange
        var observerGuideId = Guid.NewGuid();
        var fileName = "photo.jpg";

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeObserverGuide = new ObserverGuideAggregateFaker(observerGuideId, fileName).Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        _repository.FirstOrDefaultAsync(Arg.Any<GetObserverGuideSpecification>())
            .Returns(fakeObserverGuide);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Id = observerGuideId
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<ObserverGuideModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotFound()
    {
        // Arrange
        var observerGuideId = Guid.NewGuid();

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _repository.FirstOrDefaultAsync(Arg.Any<GetObserverGuideSpecification>())
            .ReturnsNull();

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Id = observerGuideId
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<ObserverGuideModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
