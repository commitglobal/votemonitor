using FastEndpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Vote.Monitor.Api.Feature.PollingStation.Notes.Specifications;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationNoteAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes.UnitTests.Endpoints;

public class CreateEndpointTests
{
    [Fact]
    public async Task ShouldReturnOkWithNoteModel_WhenAllIdsExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        var timeService = Substitute.For<ITimeProvider>();
        var repository = Substitute.For<IRepository<PollingStationNote>>();
        var electionRoundRepository = Substitute.For<IRepository<ElectionRound>>();
        var pollingStationRepository = Substitute.For<IRepository<PollingStationAggregate>>();
        var monitoringObserverRepository = Substitute.For<IRepository<MonitoringObserver>>();

        electionRoundRepository
            .FirstOrDefaultAsync(Arg.Any<GetElectionRoundSpecification>())
            .Returns(fakeElectionRound);

        pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .Returns(fakePollingStation);

        monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(fakeMonitoringObserver);

        var endpoint = Factory.Create<Create.Endpoint>(repository, 
            electionRoundRepository, 
            pollingStationRepository,
            monitoringObserverRepository,
            timeService);

        // Act
        var noteText = "a polling station note";
        var request = new Create.Request
        {
           ElectionRoundId = fakeElectionRound.Id,
           PollingStationId = fakePollingStation.Id,
           ObserverId = fakeMonitoringObserver.Id,
           Text = noteText
        };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        await repository
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

        var timeService = Substitute.For<ITimeProvider>();
        var repository = Substitute.For<IRepository<PollingStationNote>>();
        var electionRoundRepository = Substitute.For<IRepository<ElectionRound>>();
        var pollingStationRepository = Substitute.For<IRepository<PollingStationAggregate>>();
        var monitoringObserverRepository = Substitute.For<IRepository<MonitoringObserver>>();

        electionRoundRepository
            .FirstOrDefaultAsync(Arg.Any<GetElectionRoundSpecification>())
            .Returns((ElectionRound)null!);

        var endpoint = Factory.Create<Create.Endpoint>(repository,
            electionRoundRepository,
            pollingStationRepository,
            monitoringObserverRepository,
            timeService);

        // Act
        var noteText = "a polling station note";
        var request = new Create.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
            Text = noteText
        };
        var result = await endpoint.ExecuteAsync(request, default);

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

        var timeService = Substitute.For<ITimeProvider>();
        var repository = Substitute.For<IRepository<PollingStationNote>>();
        var electionRoundRepository = Substitute.For<IRepository<ElectionRound>>();
        var pollingStationRepository = Substitute.For<IRepository<PollingStationAggregate>>();
        var monitoringObserverRepository = Substitute.For<IRepository<MonitoringObserver>>();

        electionRoundRepository
            .FirstOrDefaultAsync(Arg.Any<GetElectionRoundSpecification>())
            .Returns((fakeElectionRound));

        pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .Returns((PollingStationAggregate)null!);

        var endpoint = Factory.Create<Create.Endpoint>(repository,
            electionRoundRepository,
            pollingStationRepository,
            monitoringObserverRepository,
            timeService);

        // Act
        var noteText = "a polling station note";
        var request = new Create.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
            Text = noteText
        };
        var result = await endpoint.ExecuteAsync(request, default);

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

        var timeService = Substitute.For<ITimeProvider>();
        var repository = Substitute.For<IRepository<PollingStationNote>>();
        var electionRoundRepository = Substitute.For<IRepository<ElectionRound>>();
        var pollingStationRepository = Substitute.For<IRepository<PollingStationAggregate>>();
        var monitoringObserverRepository = Substitute.For<IRepository<MonitoringObserver>>();

        electionRoundRepository
            .FirstOrDefaultAsync(Arg.Any<GetElectionRoundSpecification>())
            .Returns((fakeElectionRound));

        pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .Returns(fakePollingStation);

        monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns((MonitoringObserver)null!);

        var endpoint = Factory.Create<Create.Endpoint>(repository,
            electionRoundRepository,
            pollingStationRepository,
            monitoringObserverRepository,
            timeService);

        // Act
        var noteText = "a polling station note";
        var request = new Create.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
            Text = noteText
        };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<NoteModel>, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
