using FastEndpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Vote.Monitor.Api.Feature.PollingStation.Notes.Get;
using Vote.Monitor.Api.Feature.PollingStation.Notes.Specifications;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.PollingStationNoteAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IReadRepository<PollingStationNoteAggregate> _repository;
    private readonly IRepository<ElectionRound> _electionRoundRepository;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;
    private readonly Endpoint _endpoint;

    public GetEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<PollingStationNote>>();
        _pollingStationRepository = Substitute.For<IRepository<PollingStationAggregate>>();
        _electionRoundRepository = Substitute.For<IRepository<ElectionRound>>();
        _endpoint = Factory.Create<Endpoint>(_repository,
            _pollingStationRepository,
            _electionRoundRepository);
    }

    [Fact]
    public async Task ShouldReturnOkWithNoteModel_WhenAllIdsExist()
    {
        // Arrange
        var noteId = Guid.NewGuid();
        var noteText = "a polling station note";
        
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var fakePollingStationNote = new PollingStationNoteFaker(noteId, noteText).Generate();

        _electionRoundRepository
            .FirstOrDefaultAsync(Arg.Any<GetElectionRoundSpecification>())
            .Returns(fakeElectionRound);

        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .Returns(fakePollingStation);

        _repository.FirstOrDefaultAsync(Arg.Any<GetPollingStationNoteSpecification>())
            .Returns(fakePollingStationNote);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
            Id = noteId
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<NoteModel>>();
        model.Value!.Text.Should().Be(noteText);
        model.Value.Id.Should().Be(noteId);
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
            Id = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<NoteModel>, BadRequest<ProblemDetails>, NotFound>>()
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
            Id = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<NoteModel>, BadRequest<ProblemDetails>, NotFound>>()
            .Which
            .Result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
