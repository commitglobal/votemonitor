﻿using FastEndpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Api.Feature.PollingStation.Attachments.Get;
using Vote.Monitor.Api.Feature.PollingStation.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IReadRepository<PollingStationAttachmentAggregate> _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IRepository<ElectionRound> _electionRoundRepository;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;
    private readonly IRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly Endpoint _endpoint;

    public GetEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<PollingStationAttachmentAggregate>>();
        _fileStorageService = Substitute.For<IFileStorageService>();
        _electionRoundRepository = Substitute.For<IRepository<ElectionRound>>();
        _pollingStationRepository = Substitute.For<IRepository<PollingStationAggregate>>();
        _monitoringObserverRepository = Substitute.For<IRepository<MonitoringObserver>>();
        _endpoint = Factory.Create<Endpoint>(_repository,
            _fileStorageService,
            _electionRoundRepository,
            _pollingStationRepository);
    }

    [Fact]
    public async Task ShouldReturnOkWithAttachmentModel_WhenAllIdsExist()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var fileName = "photo.jpg";
        
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var fakePollingStationAttachment = new PollingStationAttachmentFaker(attachmentId, fileName).Generate();

        _electionRoundRepository
            .GetByIdAsync(fakeElectionRound.Id)
            .Returns(fakeElectionRound);

        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .Returns(fakePollingStation);

        _repository.FirstOrDefaultAsync(Arg.Any<GetPollingStationAttachmentSpecification>())
            .Returns(fakePollingStationAttachment);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
            Id = attachmentId
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<AttachmentModel>>();
        model.Value!.FileName.Should().Be(fileName);
        model.Value.Id.Should().Be(attachmentId);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenElectionRoundDoesNotExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _electionRoundRepository
            .GetByIdAsync(fakeElectionRound.Id)
            .ReturnsNull();

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<AttachmentModel>, BadRequest<ProblemDetails>, NotFound>>()
            .Which
            .Result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenPollingStationDoesNotExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _electionRoundRepository
            .GetByIdAsync(fakeElectionRound.Id)
            .Returns(fakeElectionRound);

        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .ReturnsNull();

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<AttachmentModel>, BadRequest<ProblemDetails>, NotFound>>()
            .Which
            .Result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenPollingStationAttachmentDoesNotExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _electionRoundRepository
            .GetByIdAsync(fakeElectionRound.Id)
            .Returns(fakeElectionRound);

        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .Returns(fakePollingStation);

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(fakeMonitoringObserver);

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationAttachmentSpecification>())
            .ReturnsNull();

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<AttachmentModel>, BadRequest<ProblemDetails>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
