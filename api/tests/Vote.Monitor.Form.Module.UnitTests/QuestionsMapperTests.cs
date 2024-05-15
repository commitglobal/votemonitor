using FluentAssertions;
using Vote.Monitor.Core.Constants;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Form.Module.Mappers;
using Vote.Monitor.Form.Module.Models;
using Vote.Monitor.Form.Module.Requests;

namespace Vote.Monitor.Form.Module.UnitTests;

public class QuestionsMapperTests
{
    private readonly string _code = "Q1";

    private readonly TranslatedString _text = new()
    {
        [LanguagesList.EN.Iso1] = "english question text",
        [LanguagesList.RO.Iso1] = "romanian question text"
    };

    private readonly TranslatedString _helptext = new()
    {
        [LanguagesList.EN.Iso1] = "english question helptext",
        [LanguagesList.RO.Iso1] = "romanian question helptext"
    };

    private readonly TranslatedString _placeholder = new()
    {
        [LanguagesList.EN.Iso1] = "english question helptext",
        [LanguagesList.RO.Iso1] = "romanian question helptext"
    };

    private readonly TranslatedString _option1Text = new()
    {
        [LanguagesList.EN.Iso1] = "english text for option1",
        [LanguagesList.RO.Iso1] = "romanian text for option1"
    };

    private readonly TranslatedString _option2Text = new()
    {
        [LanguagesList.EN.Iso1] = "english text for option2",
        [LanguagesList.RO.Iso1] = "romanian text for option2"
    };

    private readonly TranslatedString _option3Text = new()
    {
        [LanguagesList.EN.Iso1] = "english text for option3",
        [LanguagesList.RO.Iso1] = "romanian text for option3"
    };

    private readonly TranslatedString _option4Text = new()
    {
        [LanguagesList.EN.Iso1] = "english text for option4",
        [LanguagesList.RO.Iso1] = "romanian text for option4"
    };

    private readonly List<SelectOption> _options;
    private readonly List<SelectOptionRequest> _requestOptions;

    public QuestionsMapperTests()
    {
        _options = [
            new(Guid.NewGuid(), _option1Text, false, false),
            new(Guid.NewGuid(), _option2Text, false, true),
            new(Guid.NewGuid(), _option3Text, true, false),
            new(Guid.NewGuid(), _option4Text, true, true),
        ];

        _requestOptions = [
            new()
            {
                Id = Guid.NewGuid(),
                Text = _option1Text,
                IsFlagged = false,
                IsFreeText = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                Text = _option2Text,
                IsFlagged = true,
                IsFreeText = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                Text = _option3Text,
                IsFlagged = false,
                IsFreeText = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Text = _option4Text,
                IsFlagged = true,
                IsFreeText = true
            },
        ];
    }


    [Fact]
    public void ToModel_ShouldReturnTextQuestionModel_WhenGivenTextQuestion()
    {
        // Arrange
        var textQuestion = TextQuestion.Create(Guid.NewGuid(), _code, _text, _helptext, _placeholder);

        // Act
        var result = QuestionsMapper.ToModel(textQuestion);

        // Assert
        result.Should().BeOfType<TextQuestionModel>();
        result.Should().BeEquivalentTo(textQuestion);
    }

    [Fact]
    public void ToModel_ShouldReturnNumberQuestionModel_WhenGivenNumberQuestion()
    {
        // Arrange
        var numberQuestion = NumberQuestion.Create(Guid.NewGuid(), _code, _text, _helptext, _placeholder);

        // Act
        var result = QuestionsMapper.ToModel(numberQuestion);

        // Assert
        result.Should().BeOfType<NumberQuestionModel>();
        result.Should().BeEquivalentTo(numberQuestion);
    }

    [Fact]
    public void ToModel_ShouldReturnNumberQuestionModel_WhenGivenDateQuestion()
    {
        // Arrange
        var dateQuestion = DateQuestion.Create(Guid.NewGuid(), _code, _text, _helptext);

        // Act
        var result = QuestionsMapper.ToModel(dateQuestion);

        // Assert
        result.Should().BeOfType<DateQuestionModel>();
        result.Should().BeEquivalentTo(dateQuestion);
    }

    [Fact]
    public void ToModel_ShouldReturnNumberQuestionModel_WhenGivenSingleSelectQuestion()
    {
        // Arrange
        var singleSelectQuestion = SingleSelectQuestion.Create(Guid.NewGuid(), _code, _text, _options, _helptext);

        // Act
        var result = QuestionsMapper.ToModel(singleSelectQuestion);

        // Assert
        result.Should().BeOfType<SingleSelectQuestionModel>();
        result.Should().BeEquivalentTo(singleSelectQuestion);
    }

    [Fact]
    public void ToModel_ShouldReturnNumberQuestionModel_WhenGivenMultiSelectQuestion()
    {
        // Arrange
        var multiSelectQuestion = MultiSelectQuestion.Create(Guid.NewGuid(), _code, _text, _options, _helptext);

        // Act
        var result = QuestionsMapper.ToModel(multiSelectQuestion);

        // Assert
        result.Should().BeOfType<MultiSelectQuestionModel>();
        result.Should().BeEquivalentTo(multiSelectQuestion);
    }

    [Fact]
    public void ToModel_ShouldReturnNumberQuestionModel_WhenGivenRatingQuestion()
    {
        // Arrange
        var ratingQuestion = RatingQuestion.Create(Guid.NewGuid(), _code, _text, RatingScale.OneTo10, _helptext);

        // Act
        var result = QuestionsMapper.ToModel(ratingQuestion);

        // Assert
        result.Should().BeOfType<RatingQuestionModel>();
        result.Should().BeEquivalentTo(ratingQuestion);
    }

    internal record UnknownQuestion : BaseQuestion
    {
        public UnknownQuestion(Guid id, string code, TranslatedString text, TranslatedString? helptext) : base(id, code, text, helptext, null)
        {
        }

        protected override void AddTranslationsInternal(string languageCode)
        {
        }

        protected override void RemoveTranslationInternal(string languageCode)
        {
        }
    }

    [Fact]
    public void ToModel_ShouldThrowApplicationException_WhenGivenUnknownQuestionType()
    {
        // Arrange
        var unknownQuestion = new UnknownQuestion(Guid.NewGuid(), "Unknown", _text, _helptext);

        // Act
        Action act = () => QuestionsMapper.ToModel(unknownQuestion);

        // Assert
        act.Should().Throw<ApplicationException>().WithMessage("Unknown question type received");
    }


    [Fact]
    public void ToEntity_ShouldReturnTextQuestion_WhenGivenTextQuestionModel()
    {
        // Arrange
        var textQuestionRequest = new TextQuestionRequest
        {
            Id = Guid.NewGuid(),
            Code = _code,
            Text = _text,
            Helptext = _helptext,
            InputPlaceholder = _placeholder
        };

        // Act
        var result = QuestionsMapper.ToEntity(textQuestionRequest);

        // Assert
        result.Should().BeOfType<TextQuestion>();
        result.Should().BeEquivalentTo(textQuestionRequest, opt => opt.ExcludingMissingMembers());
    }

    [Fact]
    public void ToEntity_ShouldReturnNumberQuestion_WhenGivenNumberQuestionModel()
    {
        // Arrange
        var numberQuestionRequest = new NumberQuestionRequest
        {
            Id = Guid.NewGuid(),
            Code = _code,
            Text = _text,
            Helptext = _helptext,
            InputPlaceholder = _placeholder
        };

        // Act
        var result = QuestionsMapper.ToEntity(numberQuestionRequest);

        // Assert
        result.Should().BeOfType<NumberQuestion>();
        result.Should().BeEquivalentTo(numberQuestionRequest, opt => opt.ExcludingMissingMembers());
    }

    [Fact]
    public void ToEntity_ShouldReturnNumberQuestion_WhenGivenDateQuestionModel()
    {
        // Arrange
        var dateQuestionRequest = new DateQuestionRequest
        {
            Id = Guid.NewGuid(),
            Code = _code,
            Text = _text,
            Helptext = _helptext,
        };
        // Act
        var result = QuestionsMapper.ToEntity(dateQuestionRequest);

        // Assert
        result.Should().BeOfType<DateQuestion>();
        result.Should().BeEquivalentTo(dateQuestionRequest, opt => opt.ExcludingMissingMembers());
    }

    [Fact]
    public void ToEntity_ShouldReturnNumberQuestion_WhenGivenSingleSelectQuestionModel()
    {
        // Arrange
        var singleSelectQuestionRequest = new SingleSelectQuestionRequest
        {
            Id = Guid.NewGuid(),
            Code = _code,
            Text = _text,
            Helptext = _helptext,
            Options = _requestOptions
        };

        // Act
        var result = QuestionsMapper.ToEntity(singleSelectQuestionRequest);

        // Assert
        result.Should().BeOfType<SingleSelectQuestion>();
        result.Should().BeEquivalentTo(singleSelectQuestionRequest, opt => opt.ExcludingMissingMembers());
    }

    [Fact]
    public void ToEntity_ShouldReturnNumberQuestion_WhenGivenMultiSelectQuestionModel()
    {
        // Arrange
        var multiSelectQuestionRequest = new MultiSelectQuestionRequest
        {
            Id = Guid.NewGuid(),
            Code = _code,
            Text = _text,
            Helptext = _helptext,
            Options = _requestOptions
        };

        // Act
        var result = QuestionsMapper.ToEntity(multiSelectQuestionRequest);

        // Assert
        result.Should().BeOfType<MultiSelectQuestion>();
        result.Should().BeEquivalentTo(multiSelectQuestionRequest, opt => opt.ExcludingMissingMembers());
    }

    [Fact]
    public void ToEntity_ShouldReturnNumberQuestion_WhenGivenRatingQuestionModel()
    {
        // Arrange
        var ratingQuestionRequest = new RatingQuestionRequest
        {
            Id = Guid.NewGuid(),
            Code = _code,
            Text = _text,
            Helptext = _helptext,
            Scale = RatingScale.OneTo10
        };

        // Act
        var result = QuestionsMapper.ToEntity(ratingQuestionRequest);

        // Assert
        result.Should().BeOfType<RatingQuestion>();
        result.Should().BeEquivalentTo(ratingQuestionRequest, opt => opt.ExcludingMissingMembers());
    }

    internal class UnknownQuestionRequest : BaseQuestionRequest;

    [Fact]
    public void ToEntity_ShouldThrowApplicationException_WhenGivenUnknownQuestionType()
    {
        // Arrange
        var unknownQuestion = new UnknownQuestionRequest();

        // Act
        Action act = () => QuestionsMapper.ToEntity(unknownQuestion);

        // Assert
        act.Should().Throw<ApplicationException>().WithMessage("Unknown question type received");
    }
}
