using FluentAssertions;
using Vote.Monitor.Answer.Module.Mappers;
using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using FluentValidation.Results;
using Vote.Monitor.Answer.Module.Requests;
using Xunit;

namespace Vote.Monitor.Answer.Module.UnitTests;

public class AnswerMapperTests
{
    [Fact]
    public void ToModel_ShouldReturnTextAnswerModel_WhenGivenTextAnswer()
    {
        // Arrange
        var textAnswer = TextAnswer.Create(Guid.NewGuid(), "some text");

        // Act
        var result = AnswerMapper.ToModel(textAnswer);

        // Assert
        result.Should().BeOfType<TextAnswerModel>();
        result.Should().BeEquivalentTo(textAnswer);
    }

    [Fact]
    public void ToModel_ShouldReturnNumberAnswerModel_WhenGivenNumberAnswer()
    {
        // Arrange
        var numberAnswer = NumberAnswer.Create(Guid.NewGuid(), 1234);

        // Act
        var result = AnswerMapper.ToModel(numberAnswer);

        // Assert
        result.Should().BeOfType<NumberAnswerModel>();
        result.Should().BeEquivalentTo(numberAnswer);
    }

    [Fact]
    public void ToModel_ShouldReturnNumberAnswerModel_WhenGivenDateAnswer()
    {
        // Arrange
        var dateAnswer = DateAnswer.Create(Guid.NewGuid(), DateTime.UtcNow);

        // Act
        var result = AnswerMapper.ToModel(dateAnswer);

        // Assert
        result.Should().BeOfType<DateAnswerModel>();
        result.Should().BeEquivalentTo(dateAnswer);
    }

    [Fact]
    public void ToModel_ShouldReturnNumberAnswerModel_WhenGivenSingleSelectAnswer()
    {
        // Arrange
        var singleSelectAnswer = SingleSelectAnswer.Create(Guid.NewGuid(), SelectedOption.Create(Guid.NewGuid(), "some text"));

        // Act
        var result = AnswerMapper.ToModel(singleSelectAnswer);

        // Assert
        result.Should().BeOfType<SingleSelectAnswerModel>();
        result.Should().BeEquivalentTo(singleSelectAnswer);
    }

    [Fact]
    public void ToModel_ShouldReturnNumberAnswerModel_WhenGivenMultiSelectAnswer()
    {
        // Arrange
        var selectedOptions = new[] {
            SelectedOption.Create(Guid.NewGuid(), "some extra text"),
            SelectedOption.Create(Guid.NewGuid(), null)
        };

        var multiSelectAnswer = MultiSelectAnswer.Create(Guid.NewGuid(), selectedOptions);

        // Act
        var result = AnswerMapper.ToModel(multiSelectAnswer);

        // Assert
        result.Should().BeOfType<MultiSelectAnswerModel>();
        result.Should().BeEquivalentTo(multiSelectAnswer);
    }

    [Fact]
    public void ToModel_ShouldReturnNumberAnswerModel_WhenGivenRatingAnswer()
    {
        // Arrange
        var ratingAnswer = RatingAnswer.Create(Guid.NewGuid(), 5);

        // Act
        var result = AnswerMapper.ToModel(ratingAnswer);

        // Assert
        result.Should().BeOfType<RatingAnswerModel>();
        result.Should().BeEquivalentTo(ratingAnswer);
    }

    internal class UnknownAnswer : BaseAnswer
    {
        public UnknownAnswer(Guid questionId) : base(questionId)
        {
        }

        public override ValidationResult Validate(BaseQuestion question, int index)
        {
            return new ValidationResult();
        }

    }

    [Fact]
    public void ToModel_ShouldThrowApplicationException_WhenGivenUnknownQuestionType()
    {
        // Arrange
        var unknownAnswer = new UnknownAnswer(Guid.NewGuid());

        // Act
        Action act = () => AnswerMapper.ToModel(unknownAnswer);

        // Assert
        act.Should().Throw<ApplicationException>().WithMessage("Unknown question type");
    }


    [Fact]
    public void ToEntity_ShouldReturnTextAnswer_WhenGivenTextAnswerModel()
    {
        // Arrange
        var textAnswerRequest = new TextAnswerRequest { QuestionId = Guid.NewGuid(), Text = "some text" };

        // Act
        var result = AnswerMapper.ToEntity(textAnswerRequest);

        // Assert
        result.Should().BeOfType<TextAnswer>();
        result.Should().BeEquivalentTo(textAnswerRequest);
    }

    [Fact]
    public void ToEntity_ShouldReturnNumberAnswer_WhenGivenNumberAnswerModel()
    {
        // Arrange
        var numberAnswerRequest = new NumberAnswerRequest { QuestionId = Guid.NewGuid(), Value = 1234 };

        // Act
        var result = AnswerMapper.ToEntity(numberAnswerRequest);

        // Assert
        result.Should().BeOfType<NumberAnswer>();
        result.Should().BeEquivalentTo(numberAnswerRequest);
    }

    [Fact]
    public void ToEntity_ShouldReturnNumberAnswer_WhenGivenDateAnswerModel()
    {
        // Arrange
        var dateAnswerRequest = new DateAnswerRequest { QuestionId = Guid.NewGuid(), Date = DateTime.UtcNow };
        // Act
        var result = AnswerMapper.ToEntity(dateAnswerRequest);

        // Assert
        result.Should().BeOfType<DateAnswer>();
        result.Should().BeEquivalentTo(dateAnswerRequest);
    }

    [Fact]
    public void ToEntity_ShouldReturnNumberAnswer_WhenGivenSingleSelectAnswerModel()
    {
        // Arrange
        var singleSelectAnswerRequest = new SingleSelectAnswerRequest
        {
            QuestionId = Guid.NewGuid(),
            Selection = new SelectedOptionRequest { Text = "some text", OptionId = new Guid() }
        };

        // Act
        var result = AnswerMapper.ToEntity(singleSelectAnswerRequest);

        // Assert
        result.Should().BeOfType<SingleSelectAnswer>();
        result.Should().BeEquivalentTo(singleSelectAnswerRequest);
    }

    [Fact]
    public void ToEntity_ShouldReturnNumberAnswer_WhenGivenMultiSelectAnswerModel()
    {
        // Arrange
        var selectedOptions = new[] {
         new SelectedOptionRequest{ OptionId = Guid.NewGuid(),Text = "some extra text"},
         new SelectedOptionRequest{ OptionId = Guid.NewGuid(),Text = null}
     };

        var multiSelectAnswerRequest = new MultiSelectAnswerRequest
        {
            QuestionId = Guid.NewGuid(),
            Selection = selectedOptions.ToList()
        };

        // Act
        var result = AnswerMapper.ToEntity(multiSelectAnswerRequest);

        // Assert
        result.Should().BeOfType<MultiSelectAnswer>();
        result.Should().BeEquivalentTo(multiSelectAnswerRequest);
    }

    [Fact]
    public void ToEntity_ShouldReturnNumberAnswer_WhenGivenRatingAnswerModel()
    {
        // Arrange
        var ratingAnswerRequest = new RatingAnswerRequest { QuestionId = Guid.NewGuid(), Value = 4 };

        // Act
        var result = AnswerMapper.ToEntity(ratingAnswerRequest);

        // Assert
        result.Should().BeOfType<RatingAnswer>();
        result.Should().BeEquivalentTo(ratingAnswerRequest);
    }

    internal class UnknownAnswerRequest : BaseAnswerRequest;

    [Fact]
    public void ToEntity_ShouldThrowApplicationException_WhenGivenUnknownQuestionType()
    {
        // Arrange
        var unknownAnswer = new UnknownAnswerRequest();

        // Act
        Action act = () => AnswerMapper.ToEntity(unknownAnswer);

        // Assert
        act.Should().Throw<ApplicationException>().WithMessage("Unknown question type");
    }
}

