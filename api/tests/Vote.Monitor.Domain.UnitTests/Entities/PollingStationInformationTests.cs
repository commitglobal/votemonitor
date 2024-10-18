using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

namespace Vote.Monitor.Domain.UnitTests.Entities;

public class PollingStationInformationTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private static readonly ElectionRound ElectionRound = new ElectionRoundAggregateFaker().Generate();
    private readonly PollingStation _pollingStation = new PollingStationFaker().Generate();
    private readonly MonitoringObserver _monitoringObserver = new MonitoringObserverFaker().Generate();
    private static readonly string[] Languages = ["RO"];
    private static readonly TextQuestion TextQuestion = new TextQuestionFaker(languageList: Languages).Generate();
    private static readonly List<BaseAnswer> Answers = [new TextAnswerFaker(question: TextQuestion).Generate()];

    private static readonly BaseQuestion[] Questions = [TextQuestion];

    private readonly PollingStationInformationForm _form =
        PollingStationInformationForm.Create(electionRound: ElectionRound, defaultLanguage: "RO", languages: Languages,
            questions: Questions);

    private readonly DateTime _now = DateTime.Now;

    [Fact]
    public void Update_Should_Not_Override_IsCompleted_When_Undefined()
    {
        // Arrange
        var submission = _form.CreatePollingStationInformation(
            _userId,
            pollingStation: _pollingStation,
            monitoringObserver: _monitoringObserver,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            answers: [],
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Some(true));

        // Act
        submission.Update(answers: [],
            numberOfQuestionsAnswered: 0,
            numberOfFlaggedAnswers: 0,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Undefined()
        );

        // Assert
        submission.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void Update_Should_Override_IsCompleted_When_HasValue()
    {
        // Arrange
        var submission = _form.CreatePollingStationInformation(
            _userId,
            pollingStation: _pollingStation,
            monitoringObserver: _monitoringObserver,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            answers: [],
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Some(true));

        // Act
        submission.Update(answers: [],
            numberOfQuestionsAnswered: 0,
            numberOfFlaggedAnswers: 0,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Some(false)
        );

        // Assert
        submission.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public void Update_Should_Not_Override_ArrivalTime_When_Undefined()
    {
        // Arrange
        var submission = _form.CreatePollingStationInformation(
            _userId,
            pollingStation: _pollingStation,
            monitoringObserver: _monitoringObserver,
            arrivalTime: ValueOrUndefined<DateTime?>.Some(_now),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            answers: [],
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Undefined());

        // Act
        submission.Update(answers: [],
            numberOfQuestionsAnswered: 0,
            numberOfFlaggedAnswers: 0,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Undefined()
        );

        // Assert
        submission.ArrivalTime.Should().Be(_now);
    }

    [Fact]
    public void Update_Should_Override_ArrivalTime_When_HasValue()
    {
        // Arrange
        var submission = _form.CreatePollingStationInformation(
            _userId,
            pollingStation: _pollingStation,
            monitoringObserver: _monitoringObserver,
            arrivalTime: ValueOrUndefined<DateTime?>.Some(_now),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            answers: [],
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Some(true));

        // Act
        submission.Update(answers: [],
            numberOfQuestionsAnswered: 0,
            numberOfFlaggedAnswers: 0,
            arrivalTime: ValueOrUndefined<DateTime?>.Some(_now.AddDays(3)),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Some(false)
        );

        // Assert
        submission.ArrivalTime.Should().Be(_now.AddDays(3));
    }

    [Fact]
    public void Update_Should_Not_Override_DepartureTime_When_Undefined()
    {
        // Arrange
        var submission = _form.CreatePollingStationInformation(
            _userId,
            pollingStation: _pollingStation,
            monitoringObserver: _monitoringObserver,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Some(_now),
            answers: [],
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Undefined());

        // Act
        submission.Update(answers: [],
            numberOfQuestionsAnswered: 0,
            numberOfFlaggedAnswers: 0,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Undefined()
        );

        // Assert
        submission.DepartureTime.Should().Be(_now);
    }

    [Fact]
    public void Update_Should_Override_DepartureTime_When_HasValue()
    {
        // Arrange
        var submission = _form.CreatePollingStationInformation(
            _userId,
            pollingStation: _pollingStation,
            monitoringObserver: _monitoringObserver,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Some(_now),
            answers: [],
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Some(true));

        // Act
        submission.Update(answers: [],
            numberOfQuestionsAnswered: 0,
            numberOfFlaggedAnswers: 0,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Some(_now.AddDays(3)),
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Some(false)
        );

        // Assert
        submission.DepartureTime.Should().Be(_now.AddDays(3));
    }

    [Fact]
    public void Update_Should_Not_Override_Breaks_When_Null()
    {
        // Arrange
        var observationBreak = ObservationBreak.Create(_now, _now.AddDays(3));

        var submission = _form.CreatePollingStationInformation(
            _userId,
            pollingStation: _pollingStation,
            monitoringObserver: _monitoringObserver,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            answers: [],
            breaks: [observationBreak],
            isCompleted: ValueOrUndefined<bool>.Undefined());

        // Act
        submission.Update(answers: [],
            numberOfQuestionsAnswered: 0,
            numberOfFlaggedAnswers: 0,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            breaks: null,
            isCompleted: ValueOrUndefined<bool>.Undefined()
        );

        // Assert
        submission.Breaks.Should().HaveCount(1);
        submission.Breaks.Should().BeEquivalentTo([observationBreak]);
    }

    [Fact]
    public void Update_Should_Override_Breaks_When_Empty()
    {
        // Arrange
        var observationBreak = ObservationBreak.Create(_now, _now.AddDays(3));

        var submission = _form.CreatePollingStationInformation(
            _userId,
            pollingStation: _pollingStation,
            monitoringObserver: _monitoringObserver,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            answers: [],
            breaks: [observationBreak],
            isCompleted: ValueOrUndefined<bool>.Undefined());

        // Act
        submission.Update(answers: [],
            numberOfQuestionsAnswered: 0,
            numberOfFlaggedAnswers: 0,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Some(_now.AddDays(3)),
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Some(false)
        );

        // Assert
        submission.Breaks.Should().BeEmpty();
    }

    [Fact]
    public void Update_Should_Override_Breaks_WithNewValue()
    {
        // Arrange
        var observationBreak = ObservationBreak.Create(_now, _now.AddDays(3));

        var submission = _form.CreatePollingStationInformation(
            _userId,
            pollingStation: _pollingStation,
            monitoringObserver: _monitoringObserver,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            answers: [],
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Undefined());

        // Act
        submission.Update(answers: [],
            numberOfQuestionsAnswered: 0,
            numberOfFlaggedAnswers: 0,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Some(_now.AddDays(3)),
            breaks: [observationBreak],
            isCompleted: ValueOrUndefined<bool>.Some(false)
        );

        // Assert
        submission.Breaks.Should().BeEquivalentTo([observationBreak]);
    }

    [Fact]
    public void Update_Should_Not_Override_Answers_When_Null()
    {
        // Arrange
        var submission = _form.CreatePollingStationInformation(
            _userId,
            pollingStation: _pollingStation,
            monitoringObserver: _monitoringObserver,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            answers: Answers,
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Undefined());

        // Act
        submission.Update(answers: null,
            numberOfQuestionsAnswered: 0,
            numberOfFlaggedAnswers: 0,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            breaks: null,
            isCompleted: ValueOrUndefined<bool>.Undefined()
        );

        // Assert
        submission.Answers.Should().HaveCount(1);
        submission.Answers.Should().BeEquivalentTo(Answers);
    }

    [Fact]
    public void Update_Should_Override_Answers_When_Empty()
    {
        // Arrange
        var submission = _form.CreatePollingStationInformation(
            _userId,
            pollingStation: _pollingStation,
            monitoringObserver: _monitoringObserver,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            answers: Answers,
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Undefined());

        // Act
        submission.Update(answers: [],
            numberOfQuestionsAnswered: 0,
            numberOfFlaggedAnswers: 0,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Some(_now.AddDays(3)),
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Some(false)
        );

        // Assert
        submission.Answers.Should().BeEmpty();
    }

    [Fact]
    public void Update_Should_Override_Answers_WithNewValue()
    {
        // Arrange
        var submission = _form.CreatePollingStationInformation(
            _userId,
            pollingStation: _pollingStation,
            monitoringObserver: _monitoringObserver,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Undefined(),
            answers: [],
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Undefined());

        // Act
        submission.Update(answers: Answers,
            numberOfQuestionsAnswered: 0,
            numberOfFlaggedAnswers: 0,
            arrivalTime: ValueOrUndefined<DateTime?>.Undefined(),
            departureTime: ValueOrUndefined<DateTime?>.Some(_now.AddDays(3)),
            breaks: [],
            isCompleted: ValueOrUndefined<bool>.Some(false)
        );

        // Assert
        submission.Answers.Should().BeEquivalentTo(Answers);
    }
}