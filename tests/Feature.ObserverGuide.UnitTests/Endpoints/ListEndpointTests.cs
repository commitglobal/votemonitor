using System.Security.Claims;
using FastEndpoints;
using Feature.ObserverGuide.List;
using Feature.ObserverGuide.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.ObserverGuide.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IReadRepository<ObserverGuideAggregate> _repository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IReadRepository<MonitoringNgo> _monitoringNgoRepository;
    private readonly Endpoint _endpoint;

    public ListEndpointTests()
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
    public async Task ShouldReturnOkWithObserverGuideModel_WhenObserverGuidesExist()
    {
        // Arrange
        var observerGuideId = Guid.NewGuid();
        var fileName = "photo.jpg";

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringNgo = new MonitoringNgoAggregateFaker().Generate();
        var fakeObserverGuide = new ObserverGuideAggregateFaker(observerGuideId, fileName).Generate();
        var anotherFakeObserverGuide = new ObserverGuideAggregateFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(fakeMonitoringNgo);

        _repository.ListAsync(Arg.Any<GetObserverGuidesSpecification>())
            .Returns([fakeObserverGuide, anotherFakeObserverGuide]);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<Response>>();

        model.Value!.Guides.Should().HaveCount(2);
        model.Value.Guides.First().FileName.Should().Be(fakeObserverGuide.FileName);
        model.Value.Guides.First().Id.Should().Be(fakeObserverGuide.Id);
        model.Value.Guides.Last().FileName.Should().Be(anotherFakeObserverGuide.FileName);
        model.Value.Guides.Last().Id.Should().Be(anotherFakeObserverGuide.Id);
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

        _repository.FirstOrDefaultAsync(Arg.Any<GetObserverGuidesSpecification>())
            .Returns(fakeObserverGuide);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<Response>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenNoObserverGuidesFound()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringNgo = new MonitoringNgoAggregateFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(fakeMonitoringNgo);

        _repository.ListAsync(Arg.Any<GetObserverGuidesSpecification>())
            .Returns([]);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<Response>>();

        model.Value!.Guides.Should().BeEmpty();
    }
}
