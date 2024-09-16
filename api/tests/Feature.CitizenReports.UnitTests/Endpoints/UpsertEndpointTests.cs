using Feature.CitizenReports.Models;
using Feature.CitizenReports.Specifications;
using Feature.CitizenReports.Upsert;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.LocationAggregate;

namespace Feature.CitizenReports.UnitTests.Endpoints;

public class UpsertEndpointTests
{
    private readonly IRepository<CitizenReport> _repository;
    private readonly IReadRepository<Location> _locationsRepository;
    private readonly IReadRepository<FormAggregate> _formRepository;

    private readonly Endpoint _endpoint;

    public UpsertEndpointTests()
    {
        _repository = Substitute.For<IRepository<CitizenReport>>();
        _formRepository = Substitute.For<IReadRepository<FormAggregate>>();
        _locationsRepository = Substitute.For<IReadRepository<Location>>();

        _endpoint = Factory.Create<Endpoint>(_repository, _locationsRepository, _formRepository);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenLocationNotFound()
    {
        // Arrange
        _locationsRepository
            .FirstOrDefaultAsync(Arg.Any<GetLocationSpecification>())
            .ReturnsNull();

        // Act
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            CitizenReportId = Guid.NewGuid(),
        };

        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<CitizenReportModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenFormNotFound()
    {
        // Arrange
        _locationsRepository
            .FirstOrDefaultAsync(Arg.Any<GetLocationSpecification>())
            .Returns(new LocationAggregateFaker());

        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetFormSpecification>())
            .ReturnsNull();

        // Act
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            CitizenReportId = Guid.NewGuid(),
        };

        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<CitizenReportModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldUpdateFormSubmission_WhenFormSubmissionExists()
    {
        // Arrange
        var location = new LocationAggregateFaker().Generate();
        _locationsRepository
            .FirstOrDefaultAsync(Arg.Any<GetLocationSpecification>())
            .Returns(location);
        
        var form = new FormAggregateFaker().Generate();
        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetFormSpecification>())
            .Returns(form);

        var citizenReport = new CitizenReportFaker().Generate();
        _repository.FirstOrDefaultAsync(Arg.Any<GetCitizenReportByIdSpecification>())
            .Returns(citizenReport);

        // Act
        var numberQuestionId = form.Questions.First(x => x.Discriminator == QuestionTypes.NumberQuestionType).Id;
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            CitizenReportId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
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
        await _repository.Received(1).UpdateAsync(citizenReport);

        result
            .Should().BeOfType<Results<Ok<CitizenReportModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<CitizenReportModel>>();
    }
    
    [Fact]
    public async Task ShouldThrow_WhenInvalidAnswersReceived()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var citizenReportId = Guid.NewGuid();
        var locationId = Guid.NewGuid();

        var electionRound = new ElectionRoundAggregateFaker(electionRoundId).Generate();

        var location = new LocationAggregateFaker(electionRound, id: locationId).Generate();
        _locationsRepository
            .FirstOrDefaultAsync(Arg.Any<GetLocationSpecification>())
            .Returns(location);
        
        var formSubmission = new FormAggregateFaker(electionRound: electionRound).Generate();
        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetFormSpecification>())
            .Returns(formSubmission);

        _repository.FirstOrDefaultAsync(Arg.Any<GetCitizenReportByIdSpecification>())
            .ReturnsNull();

        var request = new Request
        {
            ElectionRoundId = electionRoundId,
            CitizenReportId = citizenReportId,
            LocationId = locationId,
            Answers =
            [
                new NumberAnswerRequest
                {
                    QuestionId = Guid.NewGuid(),
                    Value = 69420
                }
            ]
        };

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
        var citizenReportId = Guid.NewGuid();
        var locationId = Guid.NewGuid();

        var electionRound = new ElectionRoundAggregateFaker(electionRoundId).Generate();

        var location = new LocationAggregateFaker(electionRound, id: locationId).Generate();
        _locationsRepository
            .FirstOrDefaultAsync(Arg.Any<GetLocationSpecification>())
            .Returns(location);
        
        var form = new FormAggregateFaker(electionRound).Generate();
        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetFormSpecification>())
            .Returns(form);

        _repository.FirstOrDefaultAsync(Arg.Any<GetCitizenReportByIdSpecification>())
            .ReturnsNull();

        var numberQuestionId = form.Questions.First(x => x.Discriminator == QuestionTypes.NumberQuestionType).Id;

        var request = new Request
        {
            ElectionRoundId = electionRoundId,
            CitizenReportId = citizenReportId,
            LocationId = locationId,
            Answers =
            [
                new NumberAnswerRequest
                {
                    QuestionId = numberQuestionId,
                    Value = 69420
                }
            ]
        };

        // Act
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository
            .Received(1)
            .AddAsync(Arg.Is<CitizenReport>(x => x.ElectionRoundId == electionRoundId
                                                 && x.Id == citizenReportId && x.LocationId == locationId));

        result
            .Should().BeOfType<Results<Ok<CitizenReportModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<CitizenReportModel>>();
    }
}