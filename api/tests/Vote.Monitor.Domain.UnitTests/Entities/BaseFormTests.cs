using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

namespace Vote.Monitor.Domain.UnitTests.Entities;

public class BaseFormTests
{
    private static readonly ElectionRound ElectionRound = new ElectionRoundAggregateFaker().Generate();
    private readonly PollingStation _pollingStation = new PollingStationFaker().Generate();
    private readonly MonitoringObserver _monitoringObserver = new MonitoringObserverFaker().Generate();
    private static readonly string[] Languages = ["RO"];
    private static readonly TextQuestion TextQuestion = new TextQuestionFaker(Languages).Generate();
    private readonly List<BaseAnswer> _answers = [new TextAnswerFaker(TextQuestion).Generate()];

    private static readonly BaseQuestion[] Questions = [TextQuestion];

    private readonly PollingStationInformationForm _form =
        PollingStationInformationForm.Create(ElectionRound, "RO", Languages, Questions);

    [Fact]
    public void CreatePollingStationInformation_1()
    {
        // Act
        var submission = _form.CreatePollingStationInformation(_pollingStation,
            _monitoringObserver,
            null,
            null,
            _answers,
            []);

        // Assert
        submission.ArrivalTime.Should().BeNull();
        submission.DepartureTime.Should().BeNull();
        submission.Answers.Should().BeEquivalentTo(_answers);
        submission.Breaks.Should().BeEmpty();
    }

    [Fact]
    public void CreatePollingStationInformation_2()
    {
        // Arrange
        var arrivalTime = DateTime.Now;
        // Act
        var submission = _form.CreatePollingStationInformation(_pollingStation,
            _monitoringObserver,
            arrivalTime,
            null,
            _answers,
            []);

        // Assert
        submission.ArrivalTime.Should().Be(arrivalTime);
        submission.DepartureTime.Should().BeNull();
        submission.Answers.Should().BeEquivalentTo(_answers);
        submission.Breaks.Should().BeEmpty();
    }

    [Fact]
    public void CreatePollingStationInformation_3()
    {
        // Arrange
        var arrivalTime = DateTime.Now;
        var departureTime = DateTime.Now.AddDays(3);

        // Act
        var submission = _form.CreatePollingStationInformation(_pollingStation,
            _monitoringObserver,
            arrivalTime,
            departureTime,
            _answers,
            []);

        // Assert
        submission.ArrivalTime.Should().Be(arrivalTime);
        submission.DepartureTime.Should().Be(departureTime);
        submission.Answers.Should().BeEquivalentTo(_answers);
        submission.Breaks.Should().BeEmpty();
    }

    [Fact]
    public void CreatePollingStationInformation_4()
    {
        // Arrange
        var arrivalTime = DateTime.Now;
        var departureTime = DateTime.Now.AddDays(3);
        List<ObservationBreak> observationBreaks =
        [
            ObservationBreak.Create(start: DateTime.Now, null),
            ObservationBreak.Create(start: DateTime.Now, DateTime.Now.AddDays(3)),
        ];

        // Act
        var submission = _form.CreatePollingStationInformation(_pollingStation,
            _monitoringObserver,
            arrivalTime,
            departureTime,
            _answers,
            observationBreaks);

        // Assert
        submission.ArrivalTime.Should().Be(arrivalTime);
        submission.DepartureTime.Should().Be(departureTime);
        submission.Answers.Should().BeEquivalentTo(_answers);
        submission.Breaks.Should().BeEquivalentTo(observationBreaks);
    }

    [Fact]
    public void CreatePollingStationInformation_FillIn_1()
    {
        // Arrange
        var arrivalTime = DateTime.Now;

        var submission = _form.CreatePollingStationInformation(_pollingStation,
            _monitoringObserver,
            null,
            null,
            [],
            []);
        
        // Act
        var filledInSubmission = _form.FillIn(submission, _answers, arrivalTime, null, []);

        // Assert
        filledInSubmission.ArrivalTime.Should().Be(arrivalTime);
        filledInSubmission.DepartureTime.Should().BeNull();
        filledInSubmission.Answers.Should().BeEquivalentTo(_answers);
        filledInSubmission.Breaks.Should().BeEmpty();
    }

    [Fact]
    public void CreatePollingStationInformation_FillIn_2()
    {
        // Arrange
        var arrivalTime = DateTime.Now;
        var departureTime = DateTime.Now;

        var submission = _form.CreatePollingStationInformation(_pollingStation,
            _monitoringObserver,
            arrivalTime,
            null,
            [],
            []);
        // Act

        var filledInSubmission = _form.FillIn(submission, _answers, arrivalTime, departureTime, []);

        // Assert
        filledInSubmission.ArrivalTime.Should().Be(arrivalTime);
        filledInSubmission.DepartureTime.Should().Be(departureTime);
        filledInSubmission.Answers.Should().BeEquivalentTo(_answers);
        filledInSubmission.Breaks.Should().BeEmpty();
    }
    
    [Fact]
    public void CreatePollingStationInformation_FillIn_3()
    {
        // Arrange
        var arrivalTime = DateTime.Now;
        var departureTime = DateTime.Now.AddDays(3);

        var submission = _form.CreatePollingStationInformation(_pollingStation,
            _monitoringObserver,
            arrivalTime,
            null,
            [],
            []);
        // Act

        var filledInSubmission = _form.FillIn(submission, _answers, arrivalTime, departureTime, []);

        // Assert
        filledInSubmission.ArrivalTime.Should().Be(arrivalTime);
        filledInSubmission.DepartureTime.Should().Be(departureTime);
        filledInSubmission.Answers.Should().BeEquivalentTo(_answers);
        filledInSubmission.Breaks.Should().BeEmpty();
    }

    [Fact]
    public void CreatePollingStationInformation_FillIn_4()
    {
        // Arrange
        var arrivalTime = DateTime.Now;
        var departureTime = DateTime.Now.AddDays(3);
        
        List<ObservationBreak> observationBreaks =
        [
            ObservationBreak.Create(start: DateTime.Now, null),
            ObservationBreak.Create(start: DateTime.Now, DateTime.Now.AddDays(3)),
        ];

        var submission = _form.CreatePollingStationInformation(_pollingStation,
            _monitoringObserver,
            arrivalTime,
            null,
            [],
            []);
        // Act

        var filledInSubmission = _form.FillIn(submission, _answers, arrivalTime, departureTime, observationBreaks);

        // Assert
        filledInSubmission.ArrivalTime.Should().Be(arrivalTime);
        filledInSubmission.DepartureTime.Should().Be(departureTime);
        filledInSubmission.Answers.Should().BeEquivalentTo(_answers);
        filledInSubmission.Breaks.Should().BeEquivalentTo(observationBreaks);
    }
    
    [Fact]
    public void CreatePollingStationInformation_FillIn_NullAnswers_1()
    {
        // Arrange
        var arrivalTime = DateTime.Now;

        var submission = _form.CreatePollingStationInformation(_pollingStation,
            _monitoringObserver,
            null,
            null,
            null,
            []);
        // Act

        var filledInSubmission = _form.FillIn(submission, null, arrivalTime, null, []);

        // Assert
        filledInSubmission.ArrivalTime.Should().Be(arrivalTime);
        filledInSubmission.DepartureTime.Should().BeNull();
        filledInSubmission.Answers.Should().BeEmpty();
        filledInSubmission.Breaks.Should().BeEmpty();
    }

    [Fact]
    public void CreatePollingStationInformation_FillIn_NullAnswers_2()
    {
        // Arrange
        var arrivalTime = DateTime.Now;

        var submission = _form.CreatePollingStationInformation(_pollingStation,
            _monitoringObserver,
            arrivalTime,
            null,
            [],
            []);
        // Act

        var filledInSubmission = _form.FillIn(submission, null, arrivalTime, null, []);

        // Assert
        filledInSubmission.ArrivalTime.Should().Be(arrivalTime);
        filledInSubmission.DepartureTime.Should().BeNull();
        filledInSubmission.Answers.Should().BeEmpty();
        filledInSubmission.Breaks.Should().BeEmpty();
    }
    
    [Fact]
    public void CreatePollingStationInformation_FillIn_NullAnswers_3()
    {
        // Arrange
        var arrivalTime = DateTime.Now;
        var departureTime = DateTime.Now.AddDays(3);

        var submission = _form.CreatePollingStationInformation(_pollingStation,
            _monitoringObserver,
            arrivalTime,
            null,
            [],
            []);
        // Act

        var filledInSubmission = _form.FillIn(submission, null, arrivalTime, departureTime, []);

        // Assert
        filledInSubmission.ArrivalTime.Should().Be(arrivalTime);
        filledInSubmission.DepartureTime.Should().Be(departureTime);
        filledInSubmission.Answers.Should().BeEmpty();
        filledInSubmission.Breaks.Should().BeEmpty();
    }

    [Fact]
    public void CreatePollingStationInformation_FillIn_NullAnswers_4()
    {
        // Arrange
        var arrivalTime = DateTime.Now;
        var departureTime = DateTime.Now.AddDays(3);
        List<ObservationBreak> observationBreaks =
        [
            ObservationBreak.Create(start: DateTime.Now, null),
            ObservationBreak.Create(start: DateTime.Now, DateTime.Now.AddDays(3)),
        ];

        var submission = _form.CreatePollingStationInformation(_pollingStation,
            _monitoringObserver,
            arrivalTime,
            null,
            [],
            []);
        // Act

        var filledInSubmission = _form.FillIn(submission, null, arrivalTime, departureTime, observationBreaks);

        // Assert
        filledInSubmission.ArrivalTime.Should().Be(arrivalTime);
        filledInSubmission.DepartureTime.Should().Be(departureTime);
        filledInSubmission.Answers.Should().BeEmpty();
        filledInSubmission.Breaks.Should().BeEquivalentTo(observationBreaks);
    }
}