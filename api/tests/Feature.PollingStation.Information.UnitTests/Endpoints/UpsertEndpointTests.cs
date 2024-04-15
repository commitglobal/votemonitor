using System.Security.Claims;
using Feature.PollingStation.Information.Specifications;
using Feature.PollingStation.Information.Upsert;
using Microsoft.AspNetCore.Authorization;

namespace Feature.PollingStation.Information.UnitTests.Endpoints;

public class UpsertEndpointTests
{
    private readonly IRepository<PollingStationInformation> _repository;
    private readonly IReadRepository<PollingStationAggregate> _pollingStationRepository;
    private readonly IReadRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly IReadRepository<PollingStationInformationForm> _formRepository;
    private readonly IAuthorizationService _authorizationService;

    private readonly Endpoint _endpoint;

    public UpsertEndpointTests()
    {
        _repository = Substitute.For<IRepository<PollingStationInformation>>();
        _pollingStationRepository = Substitute.For<IReadRepository<PollingStationAggregate>>();
        _monitoringObserverRepository = Substitute.For<IReadRepository<MonitoringObserver>>();
        _formRepository = Substitute.For<IReadRepository<PollingStationInformationForm>>();
        _authorizationService = Substitute.For<IAuthorizationService>();

        _endpoint = Factory.Create<Endpoint>(_repository, _pollingStationRepository, _monitoringObserverRepository, _formRepository, _authorizationService);

        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenUserIsNotAuthorized()
    {
        // Arrange
        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };

        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<PollingStationInformationModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenPollingStationInformationFormNotFound()
    {
        // Arrange
        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationFormSpecification>())
            .ReturnsNull();

        // Act
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };

        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<PollingStationInformationModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldUpdatePollingStationInformation_WhenPollingStationInformationExists()
    {
        // Arrange
        var pollingStationInformationForm = new PollingStationInformationFormFaker().Generate();
        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationFormSpecification>())
            .Returns(pollingStationInformationForm);

        var pollingStationInformation = new PollingStationInformationFaker().Generate();
        _repository.FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationSpecification>())
            .Returns(pollingStationInformation);

        // Act
        var numberQuestionId = pollingStationInformationForm.Questions.First(x => x.Discriminator == QuestionTypes.NumberQuestionType).Id;
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            Answers = [
                new NumberAnswerRequest
                {
                    QuestionId = numberQuestionId,
                    Value = 69420
                }
            ]
        };

        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository.Received(1).UpdateAsync(pollingStationInformation);

        result
            .Should().BeOfType<Results<Ok<PollingStationInformationModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<PollingStationInformationModel>>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenPollingStationNotExists()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var pollingStationId = Guid.NewGuid();
        var observerId = Guid.NewGuid();

        _repository.FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationSpecification>())
            .ReturnsNull();

        var request = new Request
        {
            ElectionRoundId = electionRoundId,
            PollingStationId = pollingStationId,
            ObserverId = observerId
        };

        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .ReturnsNull();

        // Act
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<PollingStationInformationModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenMonitoringObserverNotExists()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var pollingStationId = Guid.NewGuid();
        var observerId = Guid.NewGuid();

        var electionRound = new ElectionRoundAggregateFaker(electionRoundId).Generate();

        _repository.FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationSpecification>())
            .ReturnsNull();

        var request = new Request
        {
            ElectionRoundId = electionRoundId,
            PollingStationId = pollingStationId,
            ObserverId = observerId
        };

        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .Returns(new PollingStationFaker(pollingStationId, electionRound).Generate());

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .ReturnsNull();

        // Act
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<PollingStationInformationModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldThrow_WhenInvalidAnswersReceived()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var pollingStationId = Guid.NewGuid();
        var observerId = Guid.NewGuid();

        var electionRound = new ElectionRoundAggregateFaker(electionRoundId).Generate();

        var pollingStationInformationForm = new PollingStationInformationFormFaker(electionRound).Generate();
        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationFormSpecification>())
            .Returns(pollingStationInformationForm);

        _repository.FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationSpecification>())
            .ReturnsNull();

        var request = new Request
        {
            ElectionRoundId = electionRoundId,
            PollingStationId = pollingStationId,
            ObserverId = observerId,
            Answers = [
                new NumberAnswerRequest
                {
                    QuestionId = Guid.NewGuid(),
                    Value = 69420
                }
            ]
        };

        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .Returns(new PollingStationFaker(pollingStationId, electionRound).Generate());

        var monitoringObserver = new MonitoringObserverFaker(observerId: observerId).Generate();
        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(monitoringObserver);

        // Act
        Func<Task> act = () => _endpoint.ExecuteAsync(request, default);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidationFailureException>();
        exception.Which.Failures.Should().HaveCount(1);
    }

    [Fact]
    public async Task ShouldCreatePollingStationInformation_WhenPollingStationInformationNotExists()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var pollingStationId = Guid.NewGuid();
        var observerId = Guid.NewGuid();

        var electionRound = new ElectionRoundAggregateFaker(electionRoundId).Generate();

        var pollingStationInformationForm = new PollingStationInformationFormFaker(electionRound).Generate();
        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationFormSpecification>())
            .Returns(pollingStationInformationForm);

        _repository.FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationSpecification>())
            .ReturnsNull();

        var numberQuestionId = pollingStationInformationForm.Questions.First(x => x.Discriminator == QuestionTypes.NumberQuestionType).Id;

        var request = new Request
        {
            ElectionRoundId = electionRoundId,
            PollingStationId = pollingStationId,
            ObserverId = observerId,
            Answers = [
                new NumberAnswerRequest
                {
                    QuestionId = numberQuestionId,
                    Value = 69420
                }
            ]
        };

        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .Returns(new PollingStationFaker(pollingStationId, electionRound).Generate());

        var monitoringObserver = new MonitoringObserverFaker(observerId: observerId).Generate();
        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(monitoringObserver);

        // Act
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository
            .Received(1)
            .AddAsync(Arg.Is<PollingStationInformation>(x => x.ElectionRoundId == electionRoundId
                                                             && x.PollingStationId == pollingStationId
                                                             && x.MonitoringObserverId == monitoringObserver.Id));

        result
            .Should().BeOfType<Results<Ok<PollingStationInformationModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<PollingStationInformationModel>>();
    }
}
