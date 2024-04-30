using FluentAssertions;
using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;
using Xunit;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators;

public class AnswerAggregateFactoryTests
{
    [Theory]
    [MemberData(nameof(AggregatorFactoryTestCases))]
    public void Map_Should_Return_CorrectAggregator(BaseQuestion question, Type expectedAggregatorType)
    {
        var aggregator = AnswerAggregateFactory.Map(question, 0);
        aggregator
            .Should()
            .BeOfType(expectedAggregatorType)
            .And
            .Subject.As<BaseAnswerAggregate>()
            .QuestionId.Should().Be(question.Id);
    }

    public static IEnumerable<object[]> AggregatorFactoryTestCases =>
        new List<object[]>
        {
            new object[] { new TextQuestionFaker().Generate(), typeof(TextAnswerAggregate) },
            new object[] { new DateQuestionFaker().Generate(), typeof(DateAnswerAggregate) },
            new object[] { new RatingQuestionFaker().Generate(), typeof(RatingAnswerAggregate) },
            new object[] { new NumberQuestionFaker().Generate(), typeof(NumberAnswerAggregate) },
            new object[] { new SingleSelectQuestionFaker().Generate(), typeof(SingleSelectAnswerAggregate) },
            new object[] { new MultiSelectQuestionFaker().Generate(), typeof(MultiSelectAnswerAggregate) },
        };
}
