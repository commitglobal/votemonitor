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
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.ObserverGuide.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IReadRepository<ObserverGuideAggregate> _repository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IReadRepository<MonitoringNgo> _monitoringNgoRepository;
    private readonly Endpoint _endpoint;

    public GetEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<ObserverGuideAggregate>>();
        var fileStorageService = Substitute.For<IFileStorageService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        _monitoringNgoRepository = Substitute.For<IReadRepository<MonitoringNgo>>();

        _endpoint = Factory.Create<Endpoint>(_authorizationService,
            currentUserProvider,
            _repository,
            fileStorageService);
    }

    [Fact]
    public async Task ShouldReturnOkWithObserverGuideModel_WhenObserverGuideExists()
    {
        // Arrange
        var observerGuideId = Guid.NewGuid();
        var fileName = "photo.jpg";

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringNgo = new MonitoringNgoAggregateFaker().Generate();
        var fakeObserverGuide = new ObserverGuideAggregateFaker(observerGuideId, fileName).Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(fakeMonitoringNgo);

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
    public async Task ShouldReturnNotFound_WhenNotAuthorised()
    {
        // Arrange
        var observerGuideId = Guid.NewGuid();
        var fileName = "photo.jpg";

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringNgo = new MonitoringNgoAggregateFaker().Generate();
        var fakeObserverGuide = new ObserverGuideAggregateFaker(observerGuideId, fileName).Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(fakeMonitoringNgo);

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
        var fakeMonitoringNgo = new MonitoringNgoAggregateFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(fakeMonitoringNgo);

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
