using FastEndpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Vote.Monitor.Api.Feature.PollingStation.Notes.Create;
using Vote.Monitor.Api.Feature.PollingStation.Notes.Specifications;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationNoteAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes.UnitTests.Endpoints;

public class CreateEndpointTests
{
    private readonly IRepository<PollingStationNote> _repository;
    private readonly IRepository<ElectionRound> _electionRoundRepository;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;
    private readonly IRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly Endpoint _endpoint;

    public CreateEndpointTests()
    {
        _repository = Substitute.For<IRepository<PollingStationNote>>();
        _electionRoundRepository = Substitute.For<IRepository<ElectionRound>>();
        _pollingStationRepository = Substitute.For<IRepository<PollingStationAggregate>>();
        _monitoringObserverRepository = Substitute.For<IRepository<MonitoringObserver>>();
        _endpoint = Factory.Create<Endpoint>(_repository, 
            _electionRoundRepository, 
            _pollingStationRepository,
            _monitoringObserverRepository);
    }

    [Fact]
    public async Task ShouldReturnOkWithNoteModel_WhenAllIdsExist()
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

        // Act
        var noteText = "a polling station note";
        var request = new Request
        {
           ElectionRoundId = fakeElectionRound.Id,
           PollingStationId = fakePollingStation.Id,
           ObserverId = fakeMonitoringObserver.Id,
           Text = noteText
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository
            .Received(1)
            .AddAsync(Arg.Is<PollingStationNote>(x => x.Text == noteText
                                                          && x.ElectionRoundId == fakeElectionRound.Id
                                                          && x.PollingStationId == fakePollingStation.Id));

        var model = result.Result.As<Ok<NoteModel>>();
        model.Value!.Text.Should().Be(noteText);
        model.Value.UpdatedAt.Should().BeNull();
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
        var noteText = "a polling station note";
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
            Text = noteText
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<NoteModel>, BadRequest<ProblemDetails>>>()
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
        var noteText = "a polling station note";
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
            Text = noteText
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<NoteModel>, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenMonitoringObserverDoesNotExist()
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
            .Returns((MonitoringObserver)null!);

        // Act
        var noteText = "a polling station note";
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
            Text = noteText
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<NoteModel>, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
