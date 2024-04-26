using System.Collections;
using System.Data;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Hangfire.Jobs.ExportData;
using Vote.Monitor.Hangfire.Jobs.ExportData.ReadModels;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData;

public class FormSubmissionsDataTableGeneratorTests
{
    private const string DefaultLanguageCode = "EN";
    private const string OtherLanguageCode = "RO";
    private const string QuestionCode = "Q1";
    private const string QuestionText = "Question 1";
    private const string Option1Text = "Option 1";
    private const string Option2Text = "Option 2";
    private const string Option3Text = "Option 3";
    private const string Option4Text = "Free text option";

    private const string Note1 = "Some Note 1";
    private const string Note2 = "Some Note 2";

    private const string Attachment1Url = "https://example.com/1";
    private const string Attachment2Url = "https://example.com/2";

    private static readonly TranslatedString _questionText = new()
    {
        [DefaultLanguageCode] = QuestionText,
        [OtherLanguageCode] = "Translated Question 1",
    };

    private static readonly Guid _option1Id = Guid.NewGuid();
    private static readonly Guid _option2Id = Guid.NewGuid();
    private static readonly Guid _option3Id = Guid.NewGuid();
    private static readonly Guid _option4Id = Guid.NewGuid();

    private static readonly DateTime _utcNow = DateTime.UtcNow;

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

    private static readonly SelectOption[] _selectOptions =
    [
        SelectOption.Create(_option1Id, _option1Text, false, false),
        SelectOption.Create(_option2Id, _option2Text, false, false),
        SelectOption.Create(_option3Id, _option3Text, false, false),
        SelectOption.Create(_option4Id, _option4Text, true, false)
    ];


    private static readonly Guid _textQuestionId = Guid.NewGuid();
    private static readonly TextQuestion _textQuestion = TextQuestion.Create(_textQuestionId, QuestionCode, _questionText);

    private static readonly Guid _numberQuestionId = Guid.NewGuid();
    private static readonly NumberQuestion _numberQuestion = NumberQuestion.Create(_numberQuestionId, QuestionCode, _questionText);

    private static readonly Guid _dateQuestionId = Guid.NewGuid();
    private static readonly DateQuestion _dateQuestion = DateQuestion.Create(_dateQuestionId, QuestionCode, _questionText);

    private static readonly Guid _ratingQuestionId = Guid.NewGuid();
    private static readonly RatingQuestion _ratingQuestion = RatingQuestion.Create(_ratingQuestionId, QuestionCode, _questionText, RatingScale.OneTo10);

    private static readonly Guid _singleSelectQuestionId = Guid.NewGuid();
    private static readonly SingleSelectQuestion _singleSelectQuestion = SingleSelectQuestion.Create(_singleSelectQuestionId, QuestionCode, _questionText, _selectOptions);

    private static readonly Guid _multiSelectQuestionId = Guid.NewGuid();
    private static readonly MultiSelectQuestion _multiSelectQuestion = MultiSelectQuestion.Create(_multiSelectQuestionId, QuestionCode, _questionText, _selectOptions);

    private static readonly string[] _submissionColumns =
    [
        "SubmissionId",
        "TimeSubmitted",
        "Level1",
        "Level2",
        "Level3",
        "Level4",
        "Level5",
        "Number",
        "MonitoringObserverId",
        "FirstName",
        "LastName",
        "Email",
        "PhoneNumber",
    ];

    [Fact]
    public void FormSubmissionsDataTableGenerator_Should_Generate_DataTable_With_Default_Columns()
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

    [Fact]
    public void FormSubmissionsDataTableGenerator_Should_Generates_Correct_DataTable_WhenTextAnswers()
    {
        // Arrange
        var formName = "SampleForm";

        // No notes, no attachments
        var submission1 = FakeSubmission.For(TextAnswer.Create(_textQuestionId, "answer 1"), [], []);
        // No notes, 2 attachments
        var submission2 = FakeSubmission.For(TextAnswer.Create(_textQuestionId, "answer 2"), [], FakeAttachmentsFor(_textQuestionId));
        // 2 notes, no attachments
        var submission3 = FakeSubmission.For(TextAnswer.Create(_textQuestionId, "answer 3"), FakeNotesFor(_textQuestionId), []);
        // 2 notes, 2 attachments
        var submission4 = FakeSubmission.For(TextAnswer.Create(_textQuestionId, "answer 4"), FakeNotesFor(_textQuestionId), FakeAttachmentsFor(_textQuestionId));

        List<object[]> expectedData =
        [
            [.. GetDefaultExpectedColumns(submission1), "answer 1", "", "", "", "", "", ""],
            [.. GetDefaultExpectedColumns(submission2), "answer 2", "", "", "", "", Attachment1Url, Attachment2Url],
            [.. GetDefaultExpectedColumns(submission3), "answer 3", "", Note1, Note2, "", "", ""],
            [.. GetDefaultExpectedColumns(submission4), "answer 4", "", Note1, Note2, "", Attachment1Url, Attachment2Url],
        ];

        // Act
        var resultDataTable = FormSubmissionsDataTable
            .CreateFor(formName, DefaultLanguageCode)
            .WithHeader()
            .ForQuestion(_textQuestion)
            .WithData()
            .ForSubmission(submission1)
            .ForSubmission(submission2)
            .ForSubmission(submission3)
            .ForSubmission(submission4)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _submissionColumns,
            "Q1 - Question 1",
            "Notes",
            "Note 1",
            "Note 2",
            "Attachments",
            "Attachment 1",
            "Attachment 2",
        ];

        resultDataTable.Should().NotBeNull();
        resultDataTable.Columns.Should().HaveSameCount(expectedColumns);
        resultDataTable.Should().HaveRowCount(4);

        resultDataTable.Rows[0].ItemArray.Should().BeEquivalentTo(expectedData[0]);
        resultDataTable.Rows[1].ItemArray.Should().BeEquivalentTo(expectedData[1]);
        resultDataTable.Rows[2].ItemArray.Should().BeEquivalentTo(expectedData[2]);
        resultDataTable.Rows[3].ItemArray.Should().BeEquivalentTo(expectedData[3]);
    }

    [Fact]
    public void FormSubmissionsDataTableGenerator_Should_Generates_Correct_DataTable_WhenNumberAnswers()
    {
        // Arrange
        var formName = "SampleForm";

        // No notes, no attachments
        var submission1 = FakeSubmission.For(NumberAnswer.Create(_numberQuestionId, 42), [], []);
        // No notes, 2 attachments
        var submission2 = FakeSubmission.For(NumberAnswer.Create(_numberQuestionId, 43), [], FakeAttachmentsFor(_numberQuestionId));
        // 2 notes, no attachments
        var submission3 = FakeSubmission.For(NumberAnswer.Create(_numberQuestionId, 44), FakeNotesFor(_numberQuestionId), []);
        // 2 notes, 2 attachments
        var submission4 = FakeSubmission.For(NumberAnswer.Create(_numberQuestionId, 45), FakeNotesFor(_numberQuestionId), FakeAttachmentsFor(_numberQuestionId));

        List<object[]> expectedData =
        [
            [.. GetDefaultExpectedColumns(submission1), 42, "", "", "", "", "", ""],
            [.. GetDefaultExpectedColumns(submission2), 43, "", "", "", "", Attachment1Url, Attachment2Url],
            [.. GetDefaultExpectedColumns(submission3), 44, "", Note1, Note2, "", "", ""],
            [.. GetDefaultExpectedColumns(submission4), 45, "", Note1, Note2, "", Attachment1Url, Attachment2Url],
        ];

        // Act
        var resultDataTable = FormSubmissionsDataTable
            .CreateFor(formName, DefaultLanguageCode)
            .WithHeader()
            .ForQuestion(_numberQuestion)
            .WithData()
            .ForSubmission(submission1)
            .ForSubmission(submission2)
            .ForSubmission(submission3)
            .ForSubmission(submission4)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _submissionColumns,
            "Q1 - Question 1",
            "Notes",
            "Note 1",
            "Note 2",
            "Attachments",
            "Attachment 1",
            "Attachment 2",
        ];

        resultDataTable.Should().NotBeNull();
        resultDataTable.Columns.Should().HaveSameCount(expectedColumns);
        resultDataTable.Should().HaveRowCount(4);

        resultDataTable.Rows[0].ItemArray.Should().BeEquivalentTo(expectedData[0]);
        resultDataTable.Rows[1].ItemArray.Should().BeEquivalentTo(expectedData[1]);
        resultDataTable.Rows[2].ItemArray.Should().BeEquivalentTo(expectedData[2]);
        resultDataTable.Rows[3].ItemArray.Should().BeEquivalentTo(expectedData[3]);
    }

    [Fact]
    public void FormSubmissionsDataTableGenerator_Should_Generates_Correct_DataTable_WhenRatingAnswer()
    {
        // Arrange
        var formName = "SampleForm";

        // No notes, no attachments
        var submission1 = FakeSubmission.For(RatingAnswer.Create(_ratingQuestionId, 4), [], []);
        // No notes, 2 attachments
        var submission2 = FakeSubmission.For(RatingAnswer.Create(_ratingQuestionId, 5), [], FakeAttachmentsFor(_ratingQuestionId));
        // 2 notes, no attachments
        var submission3 = FakeSubmission.For(RatingAnswer.Create(_ratingQuestionId, 9), FakeNotesFor(_ratingQuestionId), []);
        // 2 notes, 2 attachments
        var submission4 = FakeSubmission.For(RatingAnswer.Create(_ratingQuestionId, 10), FakeNotesFor(_ratingQuestionId), FakeAttachmentsFor(_ratingQuestionId));

        List<object[]> expectedData =
        [
            [.. GetDefaultExpectedColumns(submission1), 4, "", "", "", "", "", ""],
            [.. GetDefaultExpectedColumns(submission2), 5, "", "", "", "", Attachment1Url, Attachment2Url],
            [.. GetDefaultExpectedColumns(submission3), 9, "", Note1, Note2, "", "", ""],
            [.. GetDefaultExpectedColumns(submission4), 10, "", Note1, Note2, "", Attachment1Url, Attachment2Url],
        ];

        // Act
        var resultDataTable = FormSubmissionsDataTable
            .CreateFor(formName, DefaultLanguageCode)
            .WithHeader()
            .ForQuestion(_ratingQuestion)
            .WithData()
            .ForSubmission(submission1)
            .ForSubmission(submission2)
            .ForSubmission(submission3)
            .ForSubmission(submission4)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _submissionColumns,
            "Q1 - Question 1",
            "Notes",
            "Note 1",
            "Note 2",
            "Attachments",
            "Attachment 1",
            "Attachment 2",
        ];

        resultDataTable.Should().NotBeNull();
        resultDataTable.Columns.Should().HaveSameCount(expectedColumns);
        resultDataTable.Should().HaveRowCount(4);

        resultDataTable.Rows[0].ItemArray.Should().BeEquivalentTo(expectedData[0]);
        resultDataTable.Rows[1].ItemArray.Should().BeEquivalentTo(expectedData[1]);
        resultDataTable.Rows[2].ItemArray.Should().BeEquivalentTo(expectedData[2]);
        resultDataTable.Rows[3].ItemArray.Should().BeEquivalentTo(expectedData[3]);
    }

    [Fact]
    public void FormSubmissionsDataTableGenerator_Should_Generates_Correct_DataTable_WhenDateAnswer()
    {
        // Arrange
        var formName = "SampleForm";

        var date1 = _utcNow.AddDays(-1);
        var date2 = _utcNow.AddDays(-1);
        var date3 = _utcNow.AddDays(-1);
        var date4 = _utcNow.AddDays(-1);

        // No notes, no attachments
        var submission1 = FakeSubmission.For(DateAnswer.Create(_dateQuestionId, date1), [], []);
        // No notes, 2 attachments
        var submission2 = FakeSubmission.For(DateAnswer.Create(_dateQuestionId, date2), [], FakeAttachmentsFor(_dateQuestionId));
        // 2 notes, no attachments
        var submission3 = FakeSubmission.For(DateAnswer.Create(_dateQuestionId, date3), FakeNotesFor(_dateQuestionId), []);
        // 2 notes, 2 attachments
        var submission4 = FakeSubmission.For(DateAnswer.Create(_dateQuestionId, date4), FakeNotesFor(_dateQuestionId), FakeAttachmentsFor(_dateQuestionId));

        List<object[]> expectedData =
        [
            [.. GetDefaultExpectedColumns(submission1), date1.ToString("s"), "", "", "", "", "", ""],
            [.. GetDefaultExpectedColumns(submission2), date2.ToString("s"), "", "", "", "", Attachment1Url, Attachment2Url],
            [.. GetDefaultExpectedColumns(submission3), date3.ToString("s"), "", Note1, Note2, "", "", ""],
            [.. GetDefaultExpectedColumns(submission4), date4.ToString("s"), "", Note1, Note2, "", Attachment1Url, Attachment2Url],
        ];

        // Act
        var resultDataTable = FormSubmissionsDataTable
            .CreateFor(formName, DefaultLanguageCode)
            .WithHeader()
            .ForQuestion(_dateQuestion)
            .WithData()
            .ForSubmission(submission1)
            .ForSubmission(submission2)
            .ForSubmission(submission3)
            .ForSubmission(submission4)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _submissionColumns,
            "Q1 - Question 1",
            "Notes",
            "Note 1",
            "Note 2",
            "Attachments",
            "Attachment 1",
            "Attachment 2",
        ];

        resultDataTable.Should().NotBeNull();
        resultDataTable.Columns.Should().HaveSameCount(expectedColumns);
        resultDataTable.Should().HaveRowCount(4);

        resultDataTable.Rows[0].ItemArray.Should().BeEquivalentTo(expectedData[0]);
        resultDataTable.Rows[1].ItemArray.Should().BeEquivalentTo(expectedData[1]);
        resultDataTable.Rows[2].ItemArray.Should().BeEquivalentTo(expectedData[2]);
        resultDataTable.Rows[3].ItemArray.Should().BeEquivalentTo(expectedData[3]);
    }

    [Fact]
    public void FormSubmissionsDataTableGenerator_Should_Generates_Correct_DataTable_WhenSingleSelectAnswer()
    {
        // Arrange
        var formName = "SampleForm";
        var singleSelectQuestion = SingleSelectQuestion.Create(_singleSelectQuestionId, QuestionCode, _questionText, _selectOptions);

        // No notes, no attachments
        var submission1 = FakeSubmission.For(SingleSelectAnswer.Create(_singleSelectQuestionId, SelectedOption.Create(_option1Id, null)), [], []);
        // No notes, 2 attachments
        var submission2 = FakeSubmission.For(SingleSelectAnswer.Create(_singleSelectQuestionId, SelectedOption.Create(_option2Id, null)), [], FakeAttachmentsFor(_singleSelectQuestionId));
        // 2 notes, no attachments
        var submission3 = FakeSubmission.For(SingleSelectAnswer.Create(_singleSelectQuestionId, SelectedOption.Create(_option3Id, null)), FakeNotesFor(_singleSelectQuestionId), []);
        // 2 notes, 2 attachments
        var submission4 = FakeSubmission.For(SingleSelectAnswer.Create(_singleSelectQuestionId, SelectedOption.Create(_option4Id, "some free text")), FakeNotesFor(_singleSelectQuestionId), FakeAttachmentsFor(_singleSelectQuestionId));

        List<object[]> expectedData =
        [
            [.. GetDefaultExpectedColumns(submission1), string.Empty, true, false, false, false, "", "", "", "", "", "", ""],
            [.. GetDefaultExpectedColumns(submission2), string.Empty, false, true, false, false, "", "", "", "", "", Attachment1Url, Attachment2Url],
            [.. GetDefaultExpectedColumns(submission3), string.Empty, false, false, true, false, "", "", Note1, Note2, "", "", ""],
            [.. GetDefaultExpectedColumns(submission4), string.Empty, false, false, false, true, "some free text", "", Note1, Note2, "", Attachment1Url, Attachment2Url],
        ];

        // Act
        var resultDataTable = FormSubmissionsDataTable
            .CreateFor(formName, DefaultLanguageCode)
            .WithHeader()
            .ForQuestion(singleSelectQuestion)
            .WithData()
            .ForSubmission(submission1)
            .ForSubmission(submission2)
            .ForSubmission(submission3)
            .ForSubmission(submission4)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _submissionColumns,
            "Q1 - Question 1",
            Option1Text,
            Option2Text,
            Option3Text,
            Option4Text,
            Option4Text + "-UserInput",
            "Notes",
            "Note 1",
            "Note 2",
            "Attachments",
            "Attachment 1",
            "Attachment 2",
        ];

        resultDataTable.Should().NotBeNull();
        resultDataTable.Columns.Should().HaveSameCount(expectedColumns);
        resultDataTable.Should().HaveRowCount(4);

        resultDataTable.Rows[0].ItemArray.Should().BeEquivalentTo(expectedData[0]);
        resultDataTable.Rows[1].ItemArray.Should().BeEquivalentTo(expectedData[1]);
        resultDataTable.Rows[2].ItemArray.Should().BeEquivalentTo(expectedData[2]);
        resultDataTable.Rows[3].ItemArray.Should().BeEquivalentTo(expectedData[3]);
    }

    [Fact]
    public void FormSubmissionsDataTableGenerator_Should_Generates_Correct_DataTable_WhenMultiSelectAnswer()
    {
        // Arrange
        var formName = "SampleForm";
        var multiSelectQuestion = MultiSelectQuestion.Create(_multiSelectQuestionId, QuestionCode, _questionText, _selectOptions);

        // No notes, no attachments
        SelectedOption[] submission1Selection = [
            SelectedOption.Create(_option4Id, "user written text"),
            SelectedOption.Create(_option2Id, ""),
            SelectedOption.Create(_option3Id, ""),
            SelectedOption.Create(_option1Id, ""),
        ];
        var submission1 = FakeSubmission.For(MultiSelectAnswer.Create(_multiSelectQuestionId, submission1Selection), [], []);

        // No notes, 2 attachments
        SelectedOption[] submission2Selection = [SelectedOption.Create(_option4Id, "some written text")];
        var submission2 = FakeSubmission.For(MultiSelectAnswer.Create(_multiSelectQuestionId, submission2Selection), [], FakeAttachmentsFor(_multiSelectQuestionId));

        // 2 notes, no attachments
        SelectedOption[] submission3Selection = [SelectedOption.Create(_option3Id, ""), SelectedOption.Create(_option2Id, "")];
        var submission3 = FakeSubmission.For(MultiSelectAnswer.Create(_multiSelectQuestionId, submission3Selection), FakeNotesFor(_multiSelectQuestionId), []);

        // 2 notes, 2 attachments
        SelectedOption[] submission4Selection = [SelectedOption.Create(_option4Id, "some free text"), SelectedOption.Create(_option1Id, "")];
        var submission4 = FakeSubmission.For(MultiSelectAnswer.Create(_multiSelectQuestionId, submission4Selection), FakeNotesFor(_multiSelectQuestionId), FakeAttachmentsFor(_multiSelectQuestionId));

        List<object[]> expectedData =
        [
            [.. GetDefaultExpectedColumns(submission1), string.Empty, true, true, true, true, "user written text", "", "", "", "", "", ""],
            [.. GetDefaultExpectedColumns(submission2), string.Empty, false, false, false, true, "some written text", "", "", "", "", Attachment1Url, Attachment2Url],
            [.. GetDefaultExpectedColumns(submission3), string.Empty, false, true, true, false, "", "", Note1, Note2, "", "", ""],
            [.. GetDefaultExpectedColumns(submission4), string.Empty, true, false, false, true, "some free text", "", Note1, Note2, "", Attachment1Url, Attachment2Url],
        ];

        // Act
        var resultDataTable = FormSubmissionsDataTable
            .CreateFor(formName, DefaultLanguageCode)
            .WithHeader()
            .ForQuestion(multiSelectQuestion)
            .WithData()
            .ForSubmission(submission1)
            .ForSubmission(submission2)
            .ForSubmission(submission3)
            .ForSubmission(submission4)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _submissionColumns,
            "Q1 - Question 1",
            Option1Text,
            Option2Text,
            Option3Text,
            Option4Text,
            Option4Text + "-UserInput",
            "Notes",
            "Note 1",
            "Note 2",
            "Attachments",
            "Attachment 1",
            "Attachment 2",
        ];

        resultDataTable.Should().NotBeNull();
        resultDataTable.Columns.Should().HaveSameCount(expectedColumns);
        resultDataTable.Should().HaveRowCount(4);

        resultDataTable.Rows[0].ItemArray.Should().BeEquivalentTo(expectedData[0]);
        resultDataTable.Rows[1].ItemArray.Should().BeEquivalentTo(expectedData[1]);
        resultDataTable.Rows[2].ItemArray.Should().BeEquivalentTo(expectedData[2]);
        resultDataTable.Rows[3].ItemArray.Should().BeEquivalentTo(expectedData[3]);
    }

    [Fact]
    public void FormSubmissionsDataTableGenerator_Should_Generates_Correct_DataTable_WhenMultipleQuestions()
    {
        // Arrange
        var formName = "SampleForm";

        // No notes, no attachments
        SelectedOption[] selection = [
            SelectedOption.Create(_option4Id, "user written text"),
            SelectedOption.Create(_option2Id, ""),
            SelectedOption.Create(_option3Id, ""),
            SelectedOption.Create(_option1Id, ""),
        ];

        var submission = FakeSubmission.For(
            TextAnswer.Create(_textQuestionId, "some answer"), [], [],
            DateAnswer.Create(_dateQuestionId, _utcNow), [], [],
            NumberAnswer.Create(_numberQuestionId, 42), [], [],
            RatingAnswer.Create(_ratingQuestionId, 3), [], [],
            SingleSelectAnswer.Create(_singleSelectQuestionId, SelectedOption.Create(_option4Id, "user written text")), [], [],
            MultiSelectAnswer.Create(_multiSelectQuestionId, selection), [], []
            );

        object[] expectedTextAnswerColumns =
            [string.Empty, true, true, true, true, "user written text", "", "", "", "", "", ""];
        object[] expectedNumberAnswerColumns =
            [string.Empty, true, true, true, true, "user written text", "", "", "", "", "", ""];
        object[] expectedRatingAnswerColumns =
            [string.Empty, true, true, true, true, "user written text", "", "", "", "", "", ""];
        object[] expectedDateAnswerColumns =
            [string.Empty, true, true, true, true, "user written text", "", "", "", "", "", ""];
        object[] expectedSingleSelectAnswerColumns =
            [string.Empty, true, true, true, true, "user written text", "", "", "", "", "", ""];
        object[] expectedMultiSelectAnswerColumns =
            [string.Empty, true, true, true, true, "user written text", "", "", "", "", "", ""];

        object[] expectedData =
        [
            .. GetDefaultExpectedColumns(submission),
            .. expectedTextAnswerColumns,
            .. expectedNumberAnswerColumns,
            .. expectedRatingAnswerColumns,
            .. expectedDateAnswerColumns,
            .. expectedSingleSelectAnswerColumns,
            .. expectedMultiSelectAnswerColumns
        ];

        // Act
        var resultDataTable = FormSubmissionsDataTable
            .CreateFor(formName, DefaultLanguageCode)
            .WithHeader()
            .ForQuestion(_textQuestion)
            .ForQuestion(_numberQuestion)
            .ForQuestion(_ratingQuestion)
            .ForQuestion(_dateQuestion)
            .ForQuestion(_singleSelectQuestion)
            .ForQuestion(_multiSelectQuestion)
            .WithData()
            .ForSubmission(submission)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _submissionColumns,
            "Q1 - Question 1",
            Option1Text,
            Option2Text,
            Option3Text,
            Option4Text,
            Option4Text + "-UserInput",
            "Notes",
            "Note 1",
            "Note 2",
            "Attachments",
            "Attachment 1",
            "Attachment 2",
        ];

        resultDataTable.Should().NotBeNull();
        resultDataTable.Columns.Should().HaveSameCount(expectedColumns);
        resultDataTable.Should().HaveRowCount(1);

        //resultDataTable.Rows[0].ItemArray.Should().BeEquivalentTo(expectedData[0]);
    }

    private object[] GetDefaultExpectedColumns(SubmissionModel submission)
    {
        return
        [
            submission.SubmissionId.ToString(),
            submission.TimeSubmitted.ToString("s"),
            submission.Level1,
            submission.Level2,
            submission.Level3,
            submission.Level4,
            submission.Level5,
            submission.Number,
            submission.MonitoringObserverId.ToString(),
            submission.FirstName,
            submission.LastName,
            submission.Email,
            submission.PhoneNumber,
        ];
    }

    private NoteModel[] FakeNotesFor(Guid questionId)
    {
        return
        [
            new NoteModel { QuestionId = questionId, Text = Note1 },
            new NoteModel { QuestionId = questionId, Text = Note2 },
        ];
    }

    private AttachmentModel[] FakeAttachmentsFor(Guid questionId)
    {
        return
        [
            new AttachmentModel { QuestionId = questionId, PresignedUrl = Attachment1Url },
            new AttachmentModel { QuestionId = questionId, PresignedUrl = Attachment2Url },
        ];
    }
}
