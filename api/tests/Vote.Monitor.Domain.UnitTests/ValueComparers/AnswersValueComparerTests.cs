using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.ValueComparers;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;
using Vote.Monitor.TestUtils.Utils;

namespace Vote.Monitor.Domain.UnitTests.ValueComparers;

public class AnswersValueComparerTests
{
    private readonly AnswersValueComparer _valueComparer = new AnswersValueComparer();
    [Fact]
    public void ComparingTwoEquivalentAnswersLists_ShouldReturnTrue()
    {
        // Arrange
        var list1 = new BaseAnswer[]
        {
            new TextAnswerFaker().Generate(),
            new NumberAnswerFaker().Generate(),
            new DateAnswerFaker().Generate(),
            new RatingAnswerFaker().Generate(),
            new SingleSelectAnswerFaker().Generate(),
            new MultiSelectAnswerFaker().Generate()
        };
                
        var list2 = list1.DeepClone();

        // Act
        var result = _valueComparer.Equals(list1, list2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingTwoDifferentAnswersLists_ShouldReturnFalse()
    {
        // Arrange
        var textAnswer = new TextAnswerFaker().Generate();
        var numberAnswer = new NumberAnswerFaker().Generate();
        var dateAnswer = new DateAnswerFaker().Generate();
        var ratingAnswer = new RatingAnswerFaker().Generate();
        var singleSelectAnswer = new SingleSelectAnswerFaker().Generate();
        var multiSelectAnswer = new MultiSelectAnswerFaker().Generate();

        var list1 = new BaseAnswer[]
        {
            numberAnswer,
            dateAnswer,
            ratingAnswer,
            singleSelectAnswer,
            multiSelectAnswer,
            textAnswer
        };

        var list2 = new BaseAnswer[]
        {
            textAnswer,
            numberAnswer,
            dateAnswer,
            ratingAnswer,
            singleSelectAnswer,
            multiSelectAnswer
        };

        // Act
        var result = _valueComparer.Equals(list1, list2);


        // Assert
        result.Should().BeFalse();
    }
}
