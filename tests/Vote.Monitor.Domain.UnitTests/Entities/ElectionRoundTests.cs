using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.TestUtils.Fakes;

namespace Vote.Monitor.Domain.UnitTests.Entities;

public class ElectionRoundTests
{
    [Fact]
    public void UpdateDetails_ShouldUpdateDetailsCorrectly()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();

        // Act
        electionRound.UpdateDetails(Guid.NewGuid(), "New Title", "New English Title", new DateOnly(2022, 2, 2));

        // Assert
        electionRound.Title.Should().Be("New Title");
        electionRound.EnglishTitle.Should().Be("New English Title");
        electionRound.StartDate.Should().Be(new DateOnly(2022, 2, 2));
        electionRound.CountryId.Should().NotBeEmpty();
    }

    [Fact]
    public void Start_ShouldSetStatusToStarted()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        
        // Act
        electionRound.Start();

        // Assert
        electionRound.Status.Should().Be(ElectionRoundStatus.Started);
    }

    [Fact]
    public void Unstart_ShouldSetStatusToNotStarted()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        electionRound.Start(); // Start the election round

        // Act
        electionRound.Unstart();

        // Assert
        electionRound.Status.Should().Be(ElectionRoundStatus.NotStarted);
    }



    [Fact]
    public void AddMonitoringNgo_ShouldAddMonitoringNgoCorrectly()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var ngoId = Guid.NewGuid();

        // Act
        electionRound.AddMonitoringNgo(ngoId);

        // Assert
        electionRound.MonitoringNgos.Should().ContainSingle().Subject.NgoId.Should().Be(ngoId);
    }

    [Fact]
    public void AddMonitoringObserver_ShouldAddMonitoringObserverCorrectly()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var observerId = Guid.NewGuid();
        var invitingNgoId = Guid.NewGuid();

        // Act
        electionRound.AddMonitoringObserver(observerId, invitingNgoId);

        // Assert
        electionRound.MonitoringObservers.Should().ContainSingle()
            .Subject.ObserverId.Should().Be(observerId);
    }

    [Fact]
    public void AddMonitoringNgo_ShouldNotAddDuplicatedMonitoringNgos()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var ngoId = Guid.NewGuid();
        electionRound.AddMonitoringNgo(ngoId);

        // Act
        electionRound.AddMonitoringNgo(ngoId);

        // Assert
        electionRound.MonitoringNgos.Should().ContainSingle().Subject.NgoId.Should().Be(ngoId);
    }

    [Fact]
    public void AddMonitoringObserver_ShouldNotAddDuplicatedMonitoringObservers()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var observerId = Guid.NewGuid();
        var invitingNgoId = Guid.NewGuid();
        electionRound.AddMonitoringObserver(observerId, invitingNgoId);

        // Act
        electionRound.AddMonitoringObserver(observerId, invitingNgoId);

        // Assert
        electionRound.MonitoringObservers.Should().ContainSingle()
            .Subject.ObserverId.Should().Be(observerId);
    }

    [Fact]
    public void RemoveMonitoringNgo_ShouldRemoveMonitoringNgoCorrectly()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var ngoId = Guid.NewGuid();
        electionRound.AddMonitoringNgo(ngoId);

        // Act
        electionRound.RemoveMonitoringNgo(ngoId);

        // Assert
        electionRound.MonitoringNgos.Should().BeEmpty();
    }

    [Fact]
    public void RemoveMonitoringObserver_ShouldRemoveMonitoringObserverCorrectly()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var observerId = Guid.NewGuid();
        var invitingNgoId = Guid.NewGuid();
        electionRound.AddMonitoringObserver(observerId, invitingNgoId);

        // Act
        electionRound.RemoveMonitoringObserver(observerId);

        // Assert
        electionRound.MonitoringObservers.Should().BeEmpty();
    }

}
