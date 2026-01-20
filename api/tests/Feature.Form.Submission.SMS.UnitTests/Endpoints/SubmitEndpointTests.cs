namespace Feature.Form.Submission.SMS.UnitTests.Endpoints;
using FastEndpoints;
using Feature.Form.Submission.SMS.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Core.Constants;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.Ingestor.Core.Converters;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

public class SubmitEndpointTests
{
    private readonly IRepository<FormSubmissionAggregate> _repository;
    private readonly IReadRepository<FormAggregate> _formRepository;
    private readonly IReadRepository<MonitoringObserverAggregate> _monitoringObserverRepository;
    private readonly IReadRepository<PollingStationAggregate> _pollingStationRepository;
    private readonly Submit.Endpoint _endpoint;

    private const string FORM_CODE = "XA";

    private Submit.Request ValidRequest => new Submit.Request
    {
        ElectionRoundId = Guid.NewGuid(),
        MonitoringNgoId = Guid.NewGuid(),
        PhoneNumber = "555-521125",
        SmsMessage = "AXBOSSCITYQA32QB3QC13"
    };

    public SubmitEndpointTests()
    {
        _repository = Substitute.For<IRepository<FormSubmissionAggregate>>();
        _formRepository = Substitute.For<IReadRepository<FormAggregate>>();


        var form = GetSmsForm();
        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetFormSpecification>())
            .Returns(form);
        _monitoringObserverRepository = Substitute.For<IReadRepository<MonitoringObserverAggregate>>();

        var monitoringObserver = new MonitoringObserverFaker();
        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserver>())
            .Returns(monitoringObserver);

        _pollingStationRepository = Substitute.For<IReadRepository<PollingStationAggregate>>();

        var pollingStation = new PollingStationFaker();
        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStation>())
            .Returns(pollingStation);

        _endpoint = Factory.Create<Submit.Endpoint>(
            _repository,
            _formRepository,
            _monitoringObserverRepository,
            _pollingStationRepository,
            new SmsToFormSubmissionDecoder(),
            new CurrentUtcTimeProvider());
    }

    private static FormAggregate GetSmsForm()
    {
        List<string> languages = [LanguagesList.EN.Iso1];
        var translatedStringFaker = new TranslatedStringFaker(languages);

        return new FormAggregateFaker(
                    languages: languages,
                    status: FormStatus.Published,
                    code: FORM_CODE,
                    questions: new List<BaseQuestion> {
                NumberQuestion.Create(Guid.NewGuid(), "QA", translatedStringFaker.Generate()),
                SingleSelectQuestion.Create(Guid.NewGuid(), "QB", translatedStringFaker.Generate(),
                    new List<SelectOption>{
                        SelectOption.Create(Guid.NewGuid(), translatedStringFaker.Generate()),
                        SelectOption.Create(Guid.NewGuid(), translatedStringFaker.Generate()),
                        SelectOption.Create(Guid.NewGuid(), translatedStringFaker.Generate()),
                    }.AsReadOnly()),
                MultiSelectQuestion.Create(Guid.NewGuid(), "QC", translatedStringFaker.Generate(),
                    new List<SelectOption>{
                        SelectOption.Create(Guid.NewGuid(), translatedStringFaker.Generate()),
                        SelectOption.Create(Guid.NewGuid(), translatedStringFaker.Generate()),
                        SelectOption.Create(Guid.NewGuid(), translatedStringFaker.Generate()),
                    }.AsReadOnly())
                    }
                ).Generate();
    }

    [Fact]
    public async Task ShouldThrow_WhenSmsMessageIsMalformed()
    {
        var request = ValidRequest;

        request.SmsMessage = "Th!sI$AR3@11yM@lf0rm3dM355@g3";

        await _endpoint.Awaiting(e => e.ExecuteAsync(request, CancellationToken.None)).Should().ThrowAsync<ValidationFailureException>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenFormDoesntExist()
    {
        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetFormSpecification>())
            .ReturnsNull();

        var result = await _endpoint.ExecuteAsync(ValidRequest, CancellationToken.None);

        result
            .Should().BeOfType<Results<Ok<FormSubmissionModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenMonitoringObserverDoesntExist()
    {
        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserver>())
            .ReturnsNull();

        var result = await _endpoint.ExecuteAsync(ValidRequest, CancellationToken.None);

        result
            .Should().BeOfType<Results<Ok<FormSubmissionModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenPollingStationDoesntExist()
    {
        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStation>())
            .ReturnsNull();

        var result = await _endpoint.ExecuteAsync(ValidRequest, CancellationToken.None);

        result
            .Should().BeOfType<Results<Ok<FormSubmissionModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldThrow_WhenNumberAnswersIsInvalid()
    {
        var request = ValidRequest;
        var invalidNumber = "99999999";
        var questionCode = "QA";
        request.SmsMessage = $"XAABCDABCD{questionCode}{invalidNumber}QB1QC3";

        var exception = await _endpoint.Awaiting(e => e.ExecuteAsync(request, CancellationToken.None)).Should().ThrowAsync<ValidationFailureException>();
        exception.Which.Failures.Should().HaveCountGreaterThanOrEqualTo(1)
            .And.Subject.Should().Contain(x => x.ErrorMessage == $"The answer {invalidNumber} for question {questionCode} must have 4 digits or less.");
    }

    [Fact]
    public async Task ShouldThrow_WhenSingleSelectAnswersIsInvalid()
    {
        var request = ValidRequest;
        var invalidSelect = "9";
        var questionCode = "QB";
        request.SmsMessage = $"XAABCDABCDQA32{questionCode}{invalidSelect}QC3";

        var exception = await _endpoint.Awaiting(e => e.ExecuteAsync(request, CancellationToken.None)).Should().ThrowAsync<ValidationFailureException>();
        exception.Which.Failures.Should().HaveCountGreaterThanOrEqualTo(1)
            .And.Subject.Should().Contain(x => x.ErrorMessage == $"The answer {invalidSelect} for question {questionCode} is not a valid option.");
    }

    [Fact]
    public async Task ShouldThrow_WhenMultiSelectAnswersIsInvalid()
    {
        var request = ValidRequest;
        var invalidSelect = "98";
        var questionCode = "QC";
        request.SmsMessage = $"XAABCDABCDQA32QB1{questionCode}{invalidSelect}";

        var exception = await _endpoint.Awaiting(e => e.ExecuteAsync(request, CancellationToken.None)).Should().ThrowAsync<ValidationFailureException>();
        exception.Which.Failures.Should().HaveCountGreaterThanOrEqualTo(2)
            .And.Subject.Should().Contain(x => x.ErrorMessage == $"The answer {invalidSelect[0]} for question {questionCode} is not a valid option.")
            .And.Subject.Should().Contain(x => x.ErrorMessage == $"The answer {invalidSelect[1]} for question {questionCode} is not a valid option.");
    }

    [Fact]
    public async Task ShouldThrow_WhenAnsweringAQuestionThatIsNotPartOfTheForm()
    {
        var request = ValidRequest;
        var newQuestionCode = "QX";
        request.SmsMessage = $"{FORM_CODE}ABCDABCDQA32QB1QC3{newQuestionCode}3";

        var exception = await _endpoint.Awaiting(e => e.ExecuteAsync(request, CancellationToken.None)).Should().ThrowAsync<ValidationFailureException>();
        exception.Which.Failures.Should().HaveCountGreaterThanOrEqualTo(1)
            .And.Subject.Should().Contain(x => x.ErrorMessage == $"Question '{newQuestionCode}' does not exist in form '{FORM_CODE}'.");
    }

    [Fact]
    public async Task ShouldThrow_WhenTheNumberOfAnswersDoesNotMatchNumberOfQuestions()
    {
        var request = ValidRequest;
        request.SmsMessage = $"{FORM_CODE}ABCDABCDQA32QB1";

        var exception = await _endpoint.Awaiting(e => e.ExecuteAsync(request, CancellationToken.None)).Should().ThrowAsync<ValidationFailureException>();
        exception.Which.Failures.Should().HaveCountGreaterThanOrEqualTo(1)
            .And.Subject.Should().Contain(x => x.ErrorMessage == $"The number of questions and answers does not match.");
    }
}
