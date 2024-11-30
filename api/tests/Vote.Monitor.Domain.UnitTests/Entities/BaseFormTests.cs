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
    private readonly Guid _userId = Guid.NewGuid();
    private static readonly ElectionRound ElectionRound = new ElectionRoundAggregateFaker().Generate();
    private readonly PollingStation _pollingStation = new PollingStationFaker().Generate();
    private readonly MonitoringObserver _monitoringObserver = new MonitoringObserverFaker().Generate();
    private static readonly string[] Languages = ["RO"];
    private static readonly TextQuestion TextQuestion = new TextQuestionFaker(Languages).Generate();
    private static readonly List<BaseAnswer> Answers = [new TextAnswerFaker(TextQuestion).Generate()];

    private static readonly BaseQuestion[] Questions = [TextQuestion];

    private readonly PollingStationInformationForm _form =
        PollingStationInformationForm.Create(ElectionRound, "RO", Languages, Questions);

    [Theory]
    [MemberData(nameof(TestCases))]
    public void CreatePollingStationInformation_ShouldUpdateTimeOfStays_Correctly(
        ValueOrUndefined<DateTime?> arrivalTime,
        ValueOrUndefined<DateTime?> departureTime,
        List<BaseAnswer>? answers,
        List<ObservationBreak> breaks)
    {
        // Act
        var submission = _form.CreatePollingStationInformation(_userId,
            _pollingStation,
            _monitoringObserver,
            arrivalTime,
            departureTime,
            answers,
            breaks,
            ValueOrUndefined<bool>.Some(true));

        // Assert
        submission.ArrivalTime.Should().Be(arrivalTime.Value);
        submission.DepartureTime.Should().Be(departureTime.Value);
        submission.Breaks.Should().BeEquivalentTo(breaks);
    }
    
    public static IEnumerable<object[]> TestCases =>
        new List<object[]>
        {
            new object[]
            {
                ValueOrUndefined<DateTime?>.Some(null), ValueOrUndefined<DateTime?>.Some(null),
                null as List<BaseAnswer>, new List<ObservationBreak>()
            },
            new object[]
            {
                ValueOrUndefined<DateTime?>.Some(DateTime.Now), ValueOrUndefined<DateTime?>.Some(null),
                null as List<BaseAnswer>, new List<ObservationBreak>()
            },
            new object[]
            {
                ValueOrUndefined<DateTime?>.Some(DateTime.Now),
                ValueOrUndefined<DateTime?>.Some(DateTime.Now.AddDays(3)), null as List<BaseAnswer>,
                new List<ObservationBreak>()
            },

            new object[]
            {
                ValueOrUndefined<DateTime?>.Some(null), ValueOrUndefined<DateTime?>.Some(null), new List<BaseAnswer>(),
                new List<ObservationBreak>()
            },
            new object[]
            {
                ValueOrUndefined<DateTime?>.Some(DateTime.Now), ValueOrUndefined<DateTime?>.Some(null),
                new List<BaseAnswer>(), new List<ObservationBreak>()
            },
            new object[]
            {
                ValueOrUndefined<DateTime?>.Some(DateTime.Now),
                ValueOrUndefined<DateTime?>.Some(DateTime.Now.AddDays(3)), new List<BaseAnswer>(),
                new List<ObservationBreak>()
            },

            new object[]
            {
                ValueOrUndefined<DateTime?>.Some(null), ValueOrUndefined<DateTime?>.Some(null), Answers,
                new List<ObservationBreak>()
            },
            new object[]
            {
                ValueOrUndefined<DateTime?>.Some(DateTime.Now), ValueOrUndefined<DateTime?>.Some(null), Answers,
                new List<ObservationBreak>()
            },
            new object[]
            {
                ValueOrUndefined<DateTime?>.Some(DateTime.Now),
                ValueOrUndefined<DateTime?>.Some(DateTime.Now.AddDays(3)), Answers, new List<ObservationBreak>()
            }
        };
}