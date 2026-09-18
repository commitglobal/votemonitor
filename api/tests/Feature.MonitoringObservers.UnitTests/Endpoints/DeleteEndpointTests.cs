using System.Security.Claims;
using Feature.MonitoringObservers.Delete;
using Feature.MonitoringObservers.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using FastEndpoints;

namespace Feature.MonitoringObservers.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<MonitoringObserver> _repository;
    private readonly Endpoint _endpoint;

    public DeleteEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IRepository<MonitoringObserver>>();
        _endpoint = Factory.Create<Endpoint>(_authorizationService, _repository);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorized()
    {
        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        var result = await _endpoint.ExecuteAsync(new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        }, CancellationToken.None);

        result.Result.Should().BeOfType<NotFound>();
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<MonitoringObserver>());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenMonitoringObserverDoesNotExist()
    {
        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _repository.FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .ReturnsNull();

        var result = await _endpoint.ExecuteAsync(new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        }, CancellationToken.None);

        result.Result.Should().BeOfType<NotFound>();
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<MonitoringObserver>());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenMonitoringObserverIsNotPending()
    {
        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        var monitoringObserver = new MonitoringObserverFaker(status: MonitoringObserverStatus.Active).Generate();
        _repository.FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(monitoringObserver);

        var result = await _endpoint.ExecuteAsync(new Request
        {
            ElectionRoundId = monitoringObserver.ElectionRoundId,
            NgoId = Guid.NewGuid(),
            Id = monitoringObserver.Id
        }, CancellationToken.None);

        result.Result.Should().BeOfType<NotFound>();
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<MonitoringObserver>());
    }

    [Fact]
    public async Task ShouldDelete_WhenMonitoringObserverIsPending()
    {
        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        var monitoringObserver = new MonitoringObserverFaker(status: MonitoringObserverStatus.Pending).Generate();
        _repository.FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(monitoringObserver);

        var result = await _endpoint.ExecuteAsync(new Request
        {
            ElectionRoundId = monitoringObserver.ElectionRoundId,
            NgoId = Guid.NewGuid(),
            Id = monitoringObserver.Id
        }, CancellationToken.None);

        await _repository.Received(1)
            .DeleteAsync(Arg.Is<MonitoringObserver>(x => x.Id == monitoringObserver.Id));

        result.Result.Should().BeOfType<NoContent>();
    }
}
