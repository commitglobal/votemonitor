using FastEndpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Vote.Monitor.Api.Feature.PollingStation.Attachments.Delete;
using Vote.Monitor.Api.Feature.PollingStation.Attachments.Specifications;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    private readonly IRepository<PollingStationAttachmentAggregate> _repository;
    private readonly IRepository<ElectionRound> _electionRoundRepository;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;
    private readonly IRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly Endpoint _endpoint;

    public DeleteEndpointTests()
    {
        _repository = Substitute.For<IRepository<PollingStationAttachmentAggregate>>();
        _electionRoundRepository = Substitute.For<IRepository<ElectionRound>>();
        _pollingStationRepository = Substitute.For<IRepository<PollingStationAggregate>>();
        _monitoringObserverRepository = Substitute.For<IRepository<MonitoringObserver>>();
        _endpoint = Factory.Create<Endpoint>(_repository,
            _electionRoundRepository,
            _pollingStationRepository);
    }

    [Fact]
    public async Task ShouldReturnOkWithNoteModel_WhenAllIdsExist()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var fileName = "photo.jpg";

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var fakePollingStationAttachment = new PollingStationAttachmentFaker(attachmentId, fileName).Generate();

        _electionRoundRepository
            .FirstOrDefaultAsync(Arg.Any<GetElectionRoundSpecification>())
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
        result
            .Should().BeOfType<Results<NoContent, NotFound, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenElectionRoundDoesNotExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _electionRoundRepository
            .FirstOrDefaultAsync(Arg.Any<GetElectionRoundSpecification>())
            .Returns((ElectionRound)null!);

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
            .Should().BeOfType<Results<NoContent, NotFound, BadRequest<ProblemDetails>>>()
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
            .FirstOrDefaultAsync(Arg.Any<GetElectionRoundSpecification>())
            .Returns(fakeElectionRound);

        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .Returns((PollingStationAggregate)null!);

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
            .Should().BeOfType<Results<NoContent, NotFound, BadRequest<ProblemDetails>>>()
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
            .FirstOrDefaultAsync(Arg.Any<GetElectionRoundSpecification>())
            .Returns(fakeElectionRound);

        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .Returns(fakePollingStation);

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(fakeMonitoringObserver);

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationAttachmentSpecification>(), CancellationToken.None)
            .Returns((PollingStationAttachmentAggregate)null!);

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
            .Should().BeOfType<Results<NoContent, NotFound, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
