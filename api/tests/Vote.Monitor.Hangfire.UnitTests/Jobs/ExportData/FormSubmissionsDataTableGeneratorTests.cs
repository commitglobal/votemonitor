using Bogus;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Hangfire.Jobs.ExportData;
using Vote.Monitor.Hangfire.Jobs.ExportData.ReadModels;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData;

public class FormSubmissionsDataTableHeaderGeneratorTests
{
    private const string DefaultLanguageCode = "EN";
    private const string OtherLanguageCode = "RO";
    private const string QuestionCode = "Q1";
    private const string QuestionText = "Question 1";
    private const string Option1Text = "Option 1";
    private const string Option2Text = "Option 2";
    private const string Option3Text = "Option 3";
    private const string Option4Text = "Free text option";

    private static readonly Guid _textQuestionId = Guid.NewGuid();
    private static readonly Guid _numberQuestionId = Guid.NewGuid();
    private static readonly Guid _dateQuestionId = Guid.NewGuid();
    private static readonly Guid _ratingQuestionId = Guid.NewGuid();
    private static readonly Guid _singleSelectQuestionId = Guid.NewGuid();
    private static readonly Guid _multiSelectQuestionId = Guid.NewGuid();

    private static readonly Guid _option1Id = Guid.NewGuid();
    private static readonly Guid _option2Id = Guid.NewGuid();
    private static readonly Guid _option3Id = Guid.NewGuid();
    private static readonly Guid _option4Id = Guid.NewGuid();
    private static readonly DateTime _utcNow = DateTime.UtcNow;

    private static readonly TranslatedString _questionText = new()
    {
        [DefaultLanguageCode] = QuestionText,
        [OtherLanguageCode] = "Translated Question 1",
    };

    private static readonly TranslatedString _option1Text = new()
    {
        [DefaultLanguageCode] = Option1Text,
        [OtherLanguageCode] = "Translated Question 1",
    };

    private static readonly TranslatedString _option2Text = new()
    {
        [DefaultLanguageCode] = Option2Text,
        [OtherLanguageCode] = "Translated some question text",
    };

    private static readonly TranslatedString _option3Text = new()
    {
        [DefaultLanguageCode] = Option3Text,
        [OtherLanguageCode] = "Translated some question text",
    };

    private static readonly TranslatedString _option4Text = new()
    {
        [DefaultLanguageCode] = Option4Text,
        [OtherLanguageCode] = "Translated some question text",
    };

    private static readonly SelectOption[] _selectOptions = new[]
    {
        SelectOption.Create(_option1Id, _option1Text, false, false),
        SelectOption.Create(_option2Id, _option2Text, false, false),
        SelectOption.Create(_option3Id, _option3Text, false, false),
        SelectOption.Create(_option4Id, _option4Text, true, false)
    };

    [Fact]
    public void NewFormSubmissionsDataTableGenerator_Should_Generate_DataTable_With_Default_Columns()
    {
        // Arrange
        var formName = "SampleForm";

        // Act
        var generator = FormSubmissionsDataTable
            .CreateFor(formName, DefaultLanguageCode)
            .WithHeader()
            .WithData();

        var resultDataTable = generator.Please();

        // Assert
        var expectedColumns = new[] { "SubmissionId","TimeSubmitted", "Level1", "Level2", "Level3", "Level4", "Level5",
            "Number", "MonitoringObserverId", "FirstName", "LastName", "Email", "PhoneNumber" };
        resultDataTable.Should().NotBeNull();
        resultDataTable.Should().HaveColumns(expectedColumns);
        resultDataTable.Columns.Should().HaveSameCount(expectedColumns);
    }

    public static IEnumerable<object[]> InputQuestionsTestData =>
        new List<object[]>
        {
            new object[] { TextQuestion.Create(_textQuestionId, QuestionCode, _questionText) },
            new object[] { NumberQuestion.Create(_numberQuestionId, QuestionCode, _questionText) },
            new object[] { DateQuestion.Create(_dateQuestionId, QuestionCode, _questionText) },
            new object[] { RatingQuestion.Create(_ratingQuestionId, QuestionCode, _questionText, RatingScale.OneTo10) }
        };

    public static IEnumerable<object[]> SelectQuestionsTestData =>
        new List<object[]>
        {
            new object[] { SingleSelectQuestion.Create(Guid.NewGuid(), QuestionCode, _questionText, _selectOptions) },
            new object[] { MultiSelectQuestion.Create(Guid.NewGuid(), QuestionCode, _questionText, _selectOptions) },
        };
    public static IEnumerable<object[]> InputQuestionSubmissionsTestData =>
        new List<object[]>
            {
                new object[]
                {
                    TextQuestion.Create(_textQuestionId, QuestionCode, _questionText),
                    FakeSubmission.For(TextAnswer.Create(_textQuestionId, "some text")),
                    "some text"
                },
                new object[]
                {
                    NumberQuestion.Create(_numberQuestionId, QuestionCode, _questionText),
                    FakeSubmission.For(NumberAnswer.Create(_numberQuestionId, 42)),
                    "42"
                },
                new object[]
                {
                    DateQuestion.Create(_dateQuestionId, QuestionCode, _questionText),
                    FakeSubmission.For(DateAnswer.Create(_dateQuestionId, _utcNow)),
                    _utcNow.ToString("O")
                },
                new object[]
                {
                    RatingQuestion.Create(_ratingQuestionId, QuestionCode, _questionText, RatingScale.OneTo10),
                    FakeSubmission.For(RatingAnswer.Create(_ratingQuestionId, 4)),
                    "4"
                }
        };
}

public sealed class FakeSubmission : Faker<SubmissionModel>
{
    private FakeSubmission(BaseAnswer answer)
    {
        RuleFor(x => x.SubmissionId, f => f.Random.Guid());
        RuleFor(x => x.TimeSubmitted, f => f.Date.Recent(1, DateTime.UtcNow));
        RuleFor(x => x.Level1, f => f.Lorem.Word());
        RuleFor(x => x.Level2, f => f.Lorem.Word());
        RuleFor(x => x.Level3, f => f.Lorem.Word());
        RuleFor(x => x.Level4, f => f.Lorem.Word());
        RuleFor(x => x.Level5, f => f.Lorem.Word());
        RuleFor(x => x.Number, f => f.Lorem.Word());
        RuleFor(x => x.FirstName, f => f.Name.FirstName());
        RuleFor(x => x.LastName, f => f.Name.LastName());
        RuleFor(x => x.Email, f => f.Internet.Email());
        RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber());
        RuleFor(x => x.MonitoringObserverId, f => f.Random.Guid());
        RuleFor(x => x.Answers, [answer]);
        RuleFor(x => x.Notes, []);
        RuleFor(x => x.Attachments, []);
    }

    public static SubmissionModel For(BaseAnswer answer)
    {
        return new FakeSubmission(answer).Generate();
    }
}
