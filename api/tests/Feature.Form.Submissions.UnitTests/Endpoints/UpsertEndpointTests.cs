using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Feature.Form.Submissions.UnitTests.Endpoints;

public class UpsertEndpointTests
{
    private readonly IRepository<FormSubmission> _repository;
    private readonly IReadRepository<PollingStationAggregate> _pollingStationRepository;
    private readonly IReadRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly IReadRepository<FormAggregate> _formRepository;
    private readonly IReadRepository<Coalition> _coalitionRepository;
    private readonly IAuthorizationService _authorizationService;

    private readonly Upsert.Endpoint _endpoint;

    public UpsertEndpointTests()
    {
        _repository = Substitute.For<IRepository<FormSubmission>>();
        _pollingStationRepository = Substitute.For<IReadRepository<PollingStationAggregate>>();
        _monitoringObserverRepository = Substitute.For<IReadRepository<MonitoringObserver>>();
        _coalitionRepository = Substitute.For<IReadRepository<Coalition>>();
        _formRepository = Substitute.For<IReadRepository<FormAggregate>>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        
        _endpoint = Factory.Create<Upsert.Endpoint>(_repository,
            _pollingStationRepository,
            _monitoringObserverRepository,
            _coalitionRepository,
            _formRepository,
            _authorizationService);

        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenUserIsNotAuthorized()
    {
        // Arrange
        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new Upsert.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };

        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<FormSubmissionModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenFormSubmissionFormNotFound()
    {
        // Arrange
        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoFormSpecification>())
            .ReturnsNull();

        // Act
        var request = new Upsert.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };

        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<FormSubmissionModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldUpdateFormSubmission_WhenFormSubmissionExists()
    {
        // Arrange
        var form = new FormAggregateFaker(status: FormStatus.Published).Generate();
        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoFormSpecification>())
            .Returns(form);

        var formSubmission = new FormSubmissionFaker().Generate();
        _repository.FirstOrDefaultAsync(Arg.Any<GetFormSubmissionSpecification>())
            .Returns(formSubmission);

        // Act
        var numberQuestionId = form.Questions.First(x => x.Discriminator == QuestionTypes.NumberQuestionType).Id;
        var request = new Upsert.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            Answers =
            [
                new NumberAnswerRequest
                {
                    QuestionId = numberQuestionId,
                    Value = 69420
                }
            ]
        };

        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository.Received(1).UpdateAsync(formSubmission);

        result
            .Should().BeOfType<Results<Ok<FormSubmissionModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<FormSubmissionModel>>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenPollingStationNotExists()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var pollingStationId = Guid.NewGuid();
        var observerId = Guid.NewGuid();

        _repository.FirstOrDefaultAsync(Arg.Any<GetFormSubmissionSpecification>())
            .ReturnsNull();

        var request = new Upsert.Request
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
            .Should().BeOfType<Results<Ok<FormSubmissionModel>, NotFound>>()
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

        _repository.FirstOrDefaultAsync(Arg.Any<GetFormSubmissionSpecification>())
            .ReturnsNull();

        var request = new Upsert.Request
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
            .Should().BeOfType<Results<Ok<FormSubmissionModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldThrow_WhenFormIsDrafted()
    {
        // Arrange
        var form = new FormAggregateFaker().Generate();
        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoFormSpecification>())
            .Returns(form);

        var request = new Upsert.Request();
        
        // Act
        Func<Task> act = () => _endpoint.ExecuteAsync(request, default);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidationFailureException>();
        exception.Which.Failures.Should().HaveCount(1).And.Subject.First().ErrorMessage.Should().Be("Form is drafted");
    }

    [Fact]
    public async Task ShouldThrow_WhenInvalidAnswersReceived()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var pollingStationId = Guid.NewGuid();
        var observerId = Guid.NewGuid();

        var electionRound = new ElectionRoundAggregateFaker(electionRoundId).Generate();

        var formSubmission = new FormAggregateFaker(electionRound: electionRound).Generate();
        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoFormSpecification>())
            .Returns(formSubmission);

        _repository.FirstOrDefaultAsync(Arg.Any<GetFormSubmissionSpecification>())
            .ReturnsNull();

        var request = new Upsert.Request
        {
            ElectionRoundId = electionRoundId,
            PollingStationId = pollingStationId,
            ObserverId = observerId,
            Answers =
            [
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
    public async Task ShouldCreateFormSubmission_WhenNotExists()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var pollingStationId = Guid.NewGuid();
        var observerId = Guid.NewGuid();

        var electionRound = new ElectionRoundAggregateFaker(electionRoundId).Generate();

        var form = new FormAggregateFaker(electionRound, status: FormStatus.Published).Generate();
        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoFormSpecification>())
            .Returns(form);

        _repository.FirstOrDefaultAsync(Arg.Any<GetFormSubmissionSpecification>())
            .ReturnsNull();

        var numberQuestionId = form.Questions.First(x => x.Discriminator == QuestionTypes.NumberQuestionType).Id;

        var request = new Upsert.Request
        {
            ElectionRoundId = electionRoundId,
            PollingStationId = pollingStationId,
            ObserverId = observerId,
            Answers =
            [
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
            .AddAsync(Arg.Is<FormSubmission>(x => x.ElectionRoundId == electionRoundId
                                                  && x.PollingStationId == pollingStationId
                                                  && x.MonitoringObserverId == monitoringObserver.Id));

        result
            .Should().BeOfType<Results<Ok<FormSubmissionModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<FormSubmissionModel>>();
    }
}
