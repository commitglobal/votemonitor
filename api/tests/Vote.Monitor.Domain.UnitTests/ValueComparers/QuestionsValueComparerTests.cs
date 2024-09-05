using Vote.Monitor.Domain.ValueComparers;
using Vote.Monitor.TestUtils.Utils;

namespace Vote.Monitor.Domain.UnitTests.ValueComparers;

public class QuestionsValueComparerTests
{
    private readonly QuestionsValueComparer _valueComparer = new();

    [Fact]
    public void ComparingTwoEquivalentQuestionsLists_ShouldReturnTrue()
    {
        // Arrange
        var list1 = new BaseQuestion[]
        {
            new TextQuestionFaker().Generate(),
            new NumberQuestionFaker().Generate(),
            new DateQuestionFaker().Generate(),
            new RatingQuestionFaker().Generate(),
            new SingleSelectQuestionFaker().Generate(),
            new MultiSelectQuestionFaker().Generate()
        };
                
        var list2 = list1.DeepClone();

        // Act
        var result = _valueComparer.Equals(list1, list2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingTwoDifferentQuestionsLists_ShouldReturnFalse()
    {
        // Arrange
        var textQuestion = new TextQuestionFaker().Generate();
        var numberQuestion = new NumberQuestionFaker().Generate();
        var dateQuestion = new DateQuestionFaker().Generate();
        var ratingQuestion = new RatingQuestionFaker().Generate();
        var singleSelectQuestion = new SingleSelectQuestionFaker().Generate();
        var multiSelectQuestion = new MultiSelectQuestionFaker().Generate();

        var list1 = new BaseQuestion[]
        {
            numberQuestion,
            dateQuestion,
            ratingQuestion,
            singleSelectQuestion,
            multiSelectQuestion,
            textQuestion
        };

        var list2 = new BaseQuestion[]
        {
            textQuestion,
            numberQuestion,
            dateQuestion,
            ratingQuestion,
            singleSelectQuestion,
            multiSelectQuestion
        };

        // Act
        var result = _valueComparer.Equals(list1, list2);


        // Assert
        result.Should().BeFalse();
    }
}
