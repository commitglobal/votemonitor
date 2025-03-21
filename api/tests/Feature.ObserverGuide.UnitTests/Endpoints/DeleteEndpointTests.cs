﻿using System.Security.Claims;
using FastEndpoints;
using Feature.ObserverGuide.Delete;
using Feature.ObserverGuide.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.ObserverGuide.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    private readonly IRepository<ObserverGuideAggregate> _repository;
    private readonly IAuthorizationService _authorizationService;
    private readonly Endpoint _endpoint;

    public DeleteEndpointTests()
    {
        _repository = Substitute.For<IRepository<ObserverGuideAggregate>>();
        _authorizationService = Substitute.For<IAuthorizationService>();

        _endpoint = Factory.Create<Endpoint>(_authorizationService,
            _repository);
    }

    [Fact]
    public async Task ShouldReturnNoContent_WhenObserverGuideExists()
    {
        // Arrange
        var observerGuideId = Guid.NewGuid();
        var fileName = "photo.jpg";

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeObserverGuide = new ObserverGuideAggregateFaker(observerGuideId, fileName).Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _repository.FirstOrDefaultAsync(Arg.Any<GetObserverGuideByIdSpecification>())
            .Returns(fakeObserverGuide);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Id = observerGuideId
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository
            .Received(1)
            .UpdateAsync(Arg.Is<ObserverGuideAggregate>(x => x.Id == fakeObserverGuide.Id
                                                             && x.IsDeleted));


        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorized()
    {
        // Arrange
        var observerGuideId = Guid.NewGuid();
        var fileName = "photo.jpg";

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeObserverGuide = new ObserverGuideAggregateFaker(observerGuideId, fileName).Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        _repository.FirstOrDefaultAsync(Arg.Any<GetObserverGuideByIdSpecification>())
            .Returns(fakeObserverGuide);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Id = observerGuideId
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
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

        _repository.FirstOrDefaultAsync(Arg.Any<GetObserverGuideByIdSpecification>())
            .ReturnsNull();

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Id = observerGuideId
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
