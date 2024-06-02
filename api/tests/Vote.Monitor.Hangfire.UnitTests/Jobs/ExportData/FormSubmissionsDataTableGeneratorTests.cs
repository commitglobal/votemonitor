using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions;
using Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions.ReadModels;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData;

public class FormSubmissionsDataTableGeneratorTests
{
    private const string DefaultLanguageCode = "EN";
    private const string OtherLanguageCode = "RO";
    private const string QuestionText = "Question text";

    private const string TextQuestionCode = "TQ";
    private const string NumberQuestionCode = "NQ";
    private const string RatingQuestionCode = "RQ";
    private const string DateQuestionCode = "DQ";
    private const string SingleSelectQuestionCode = "SC";
    private const string MultiSelectQuestionCode = "MC";

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
        [OtherLanguageCode] = $"Translated {QuestionText}",
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
    private static readonly TextQuestion _textQuestion = TextQuestion.Create(_textQuestionId, TextQuestionCode, _questionText);

    private static readonly Guid _numberQuestionId = Guid.NewGuid();
    private static readonly NumberQuestion _numberQuestion = NumberQuestion.Create(_numberQuestionId, NumberQuestionCode, _questionText);

    private static readonly Guid _dateQuestionId = Guid.NewGuid();
    private static readonly DateQuestion _dateQuestion = DateQuestion.Create(_dateQuestionId, DateQuestionCode, _questionText);

    private static readonly Guid _ratingQuestionId = Guid.NewGuid();
    private static readonly RatingQuestion _ratingQuestion = RatingQuestion.Create(_ratingQuestionId, RatingQuestionCode, _questionText, RatingScale.OneTo10);

    private static readonly Guid _singleSelectQuestionId = Guid.NewGuid();
    private static readonly SingleSelectQuestion _singleSelectQuestion = SingleSelectQuestion.Create(_singleSelectQuestionId, SingleSelectQuestionCode, _questionText, _selectOptions);

    private static readonly Guid _multiSelectQuestionId = Guid.NewGuid();
    private static readonly MultiSelectQuestion _multiSelectQuestion = MultiSelectQuestion.Create(_multiSelectQuestionId, MultiSelectQuestionCode, _questionText, _selectOptions);

    private static readonly string[] _submissionColumns =
    [
        "SubmissionId",
        "TimeSubmitted",
        "FollowUpStatus",
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
        var generator = FormSubmissionsDataTable
            .FromForm(Fake.Form(DefaultLanguageCode))
            .WithData();

        // Act
        var result = generator.Please();

        // Assert
        result.Should().NotBeNull();
        result.header.Should().ContainInOrder(_submissionColumns);
        result.header.Should().HaveSameCount(_submissionColumns);
    }

    [Fact]
    public void FormSubmissionsDataTableGenerator_Should_Generates_Correct_DataTable_WhenTextAnswers()
    {
        // Arrange
        var form = Fake.Form(DefaultLanguageCode, _textQuestion);

        // No notes, no attachments
        var submission1 = Fake.Submission(form.Id, TextAnswer.Create(_textQuestionId, "answer 1"), [], []);
        // No notes, 2 attachments
        var submission2 = Fake.Submission(form.Id, TextAnswer.Create(_textQuestionId, "answer 2"), [], FakeAttachmentsFor(_textQuestionId));
        // 2 notes, no attachments
        var submission3 = Fake.Submission(form.Id, TextAnswer.Create(_textQuestionId, "answer 3"), FakeNotesFor(_textQuestionId), []);
        // 2 notes, 2 attachments
        var submission4 = Fake.Submission(form.Id, TextAnswer.Create(_textQuestionId, "answer 4"), FakeNotesFor(_textQuestionId), FakeAttachmentsFor(_textQuestionId));

        List<object[]> expectedData =
        [
            [.. GetDefaultExpectedColumns(submission1), "answer 1", 0, "", "", 0, "", ""],
            [.. GetDefaultExpectedColumns(submission2), "answer 2", 0, "", "", 2, Attachment1Url, Attachment2Url],
            [.. GetDefaultExpectedColumns(submission3), "answer 3", 2, Note1, Note2, 0, "", ""],
            [.. GetDefaultExpectedColumns(submission4), "answer 4", 2, Note1, Note2, 2, Attachment1Url, Attachment2Url],
        ];

        // Act
        var result = FormSubmissionsDataTable
            .FromForm(form)
            .WithData()
            .ForSubmission(submission1)
            .ForSubmission(submission2)
            .ForSubmission(submission3)
            .ForSubmission(submission4)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _submissionColumns,
            "TQ - Question text",
            "Notes",
            "Note 1",
            "Note 2",
            "Attachments",
            "Attachment 1",
            "Attachment 2",
        ];

        result.Should().NotBeNull();
        result.header.Should().HaveSameCount(expectedColumns);
        result.dataTable.Should().HaveCount(4);

        result.dataTable[0].Should().ContainInOrder(expectedData[0]);
        result.dataTable[1].Should().ContainInOrder(expectedData[1]);
        result.dataTable[2].Should().ContainInOrder(expectedData[2]);
        result.dataTable[3].Should().ContainInOrder(expectedData[3]);
    }

    [Fact]
    public void FormSubmissionsDataTableGenerator_Should_Generates_Correct_DataTable_WhenNumberAnswers()
    {
        // Arrange
        var form = Fake.Form(DefaultLanguageCode, _numberQuestion);
        // No notes, no attachments
        var submission1 = Fake.Submission(form.Id, NumberAnswer.Create(_numberQuestionId, 42), [], []);
        // No notes, 2 attachments
        var submission2 = Fake.Submission(form.Id, NumberAnswer.Create(_numberQuestionId, 43), [], FakeAttachmentsFor(_numberQuestionId));
        // 2 notes, no attachments
        var submission3 = Fake.Submission(form.Id, NumberAnswer.Create(_numberQuestionId, 44), FakeNotesFor(_numberQuestionId), []);
        // 2 notes, 2 attachments
        var submission4 = Fake.Submission(form.Id, NumberAnswer.Create(_numberQuestionId, 45), FakeNotesFor(_numberQuestionId), FakeAttachmentsFor(_numberQuestionId));

        List<object[]> expectedData =
        [
            [.. GetDefaultExpectedColumns(submission1), 42, 0, "", "", 0, "", ""],
            [.. GetDefaultExpectedColumns(submission2), 43, 0, "", "", 2, Attachment1Url, Attachment2Url],
            [.. GetDefaultExpectedColumns(submission3), 44, 2, Note1, Note2, 0, "", ""],
            [.. GetDefaultExpectedColumns(submission4), 45, 2, Note1, Note2, 2, Attachment1Url, Attachment2Url],
        ];

        // Act
        var result = FormSubmissionsDataTable
            .FromForm(form)
            .WithData()
            .ForSubmission(submission1)
            .ForSubmission(submission2)
            .ForSubmission(submission3)
            .ForSubmission(submission4)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _submissionColumns,
            "NQ - Question text",
            "Notes",
            "Note 1",
            "Note 2",
            "Attachments",
            "Attachment 1",
            "Attachment 2",
        ];

        result.Should().NotBeNull();
        result.header.Should().ContainInOrder(expectedColumns);
        result.dataTable.Should().HaveCount(4);

        result.dataTable[0].Should().ContainInOrder(expectedData[0]);
        result.dataTable[1].Should().ContainInOrder(expectedData[1]);
        result.dataTable[2].Should().ContainInOrder(expectedData[2]);
        result.dataTable[3].Should().ContainInOrder(expectedData[3]);
    }

    [Fact]
    public void FormSubmissionsDataTableGenerator_Should_Generates_Correct_DataTable_WhenRatingAnswer()
    {
        // Arrange
        var form = Fake.Form(DefaultLanguageCode, _ratingQuestion);
        // No notes, no attachments
        var submission1 = Fake.Submission(form.Id, RatingAnswer.Create(_ratingQuestionId, 4), [], []);
        // No notes, 2 attachments
        var submission2 = Fake.Submission(form.Id, RatingAnswer.Create(_ratingQuestionId, 5), [], FakeAttachmentsFor(_ratingQuestionId));
        // 2 notes, no attachments
        var submission3 = Fake.Submission(form.Id, RatingAnswer.Create(_ratingQuestionId, 9), FakeNotesFor(_ratingQuestionId), []);
        // 2 notes, 2 attachments
        var submission4 = Fake.Submission(form.Id, RatingAnswer.Create(_ratingQuestionId, 10), FakeNotesFor(_ratingQuestionId), FakeAttachmentsFor(_ratingQuestionId));

        List<object[]> expectedData =
        [
            [.. GetDefaultExpectedColumns(submission1), 4, 0, "", "", 0, "", ""],
            [.. GetDefaultExpectedColumns(submission2), 5, 0, "", "", 2, Attachment1Url, Attachment2Url],
            [.. GetDefaultExpectedColumns(submission3), 9, 2, Note1, Note2, 0, "", ""],
            [.. GetDefaultExpectedColumns(submission4), 10, 2, Note1, Note2, 2, Attachment1Url, Attachment2Url],
        ];

        // Act
        var result = FormSubmissionsDataTable
            .FromForm(form)
            .WithData()
            .ForSubmission(submission1)
            .ForSubmission(submission2)
            .ForSubmission(submission3)
            .ForSubmission(submission4)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _submissionColumns,
            "RQ - Question text",
            "Notes",
            "Note 1",
            "Note 2",
            "Attachments",
            "Attachment 1",
            "Attachment 2",
        ];

        result.Should().NotBeNull();
        result.header.Should().ContainInOrder(expectedColumns);
        result.dataTable.Should().HaveCount(4);

        result.dataTable[0].Should().ContainInOrder(expectedData[0]);
        result.dataTable[1].Should().ContainInOrder(expectedData[1]);
        result.dataTable[2].Should().ContainInOrder(expectedData[2]);
        result.dataTable[3].Should().ContainInOrder(expectedData[3]);
    }

    [Fact]
    public void FormSubmissionsDataTableGenerator_Should_Generates_Correct_DataTable_WhenDateAnswer()
    {
        // Arrange
        var form = Fake.Form(DefaultLanguageCode, _dateQuestion);
        var date1 = _utcNow.AddDays(-1);
        var date2 = _utcNow.AddDays(-1);
        var date3 = _utcNow.AddDays(-1);
        var date4 = _utcNow.AddDays(-1);

        // No notes, no attachments
        var submission1 = Fake.Submission(form.Id, DateAnswer.Create(_dateQuestionId, date1), [], []);
        // No notes, 2 attachments
        var submission2 = Fake.Submission(form.Id, DateAnswer.Create(_dateQuestionId, date2), [], FakeAttachmentsFor(_dateQuestionId));
        // 2 notes, no attachments
        var submission3 = Fake.Submission(form.Id, DateAnswer.Create(_dateQuestionId, date3), FakeNotesFor(_dateQuestionId), []);
        // 2 notes, 2 attachments
        var submission4 = Fake.Submission(form.Id, DateAnswer.Create(_dateQuestionId, date4), FakeNotesFor(_dateQuestionId), FakeAttachmentsFor(_dateQuestionId));

        List<object[]> expectedData =
        [
            [.. GetDefaultExpectedColumns(submission1), date1.ToString("s"), 0, "", "", 0, "", ""],
            [.. GetDefaultExpectedColumns(submission2), date2.ToString("s"), 0, "", "", 2, Attachment1Url, Attachment2Url],
            [.. GetDefaultExpectedColumns(submission3), date3.ToString("s"), 2, Note1, Note2, 0, "", ""],
            [.. GetDefaultExpectedColumns(submission4), date4.ToString("s"), 2, Note1, Note2, 2, Attachment1Url, Attachment2Url],
        ];

        // Act
        var result = FormSubmissionsDataTable
            .FromForm(form)
            .WithData()
            .ForSubmission(submission1)
            .ForSubmission(submission2)
            .ForSubmission(submission3)
            .ForSubmission(submission4)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _submissionColumns,
            "DQ - Question text",
            "Notes",
            "Note 1",
            "Note 2",
            "Attachments",
            "Attachment 1",
            "Attachment 2",
        ];

        result.Should().NotBeNull();
        result.header.Should().ContainInOrder(expectedColumns);
        result.dataTable.Should().HaveCount(4);

        result.dataTable[0].Should().ContainInOrder(expectedData[0]);
        result.dataTable[1].Should().ContainInOrder(expectedData[1]);
        result.dataTable[2].Should().ContainInOrder(expectedData[2]);
        result.dataTable[3].Should().ContainInOrder(expectedData[3]);
    }

    [Fact]
    public void FormSubmissionsDataTableGenerator_Should_Generates_Correct_DataTable_WhenSingleSelectAnswer()
    {
        // Arrange
        var form = Fake.Form(DefaultLanguageCode, _singleSelectQuestion);
        // No notes, no attachments
        var submission1 = Fake.Submission(form.Id, SingleSelectAnswer.Create(_singleSelectQuestionId, SelectedOption.Create(_option1Id, null)), [], []);
        // No notes, 2 attachments
        var submission2 = Fake.Submission(form.Id, SingleSelectAnswer.Create(_singleSelectQuestionId, SelectedOption.Create(_option2Id, null)), [], FakeAttachmentsFor(_singleSelectQuestionId));
        // 2 notes, no attachments
        var submission3 = Fake.Submission(form.Id, SingleSelectAnswer.Create(_singleSelectQuestionId, SelectedOption.Create(_option3Id, null)), FakeNotesFor(_singleSelectQuestionId), []);
        // 2 notes, 2 attachments
        var submission4 = Fake.Submission(form.Id, SingleSelectAnswer.Create(_singleSelectQuestionId, SelectedOption.Create(_option4Id, "some free text")), FakeNotesFor(_singleSelectQuestionId), FakeAttachmentsFor(_singleSelectQuestionId));

        List<object[]> expectedData =
        [
            [.. GetDefaultExpectedColumns(submission1), "", true, false, false, false, "",0, "", "", 0, "", ""],
            [.. GetDefaultExpectedColumns(submission2), "", false, true, false, false, "", 0, "", "", 2, Attachment1Url, Attachment2Url],
            [.. GetDefaultExpectedColumns(submission3), "", false, false, true, false, "", 2, Note1, Note2, 0, "", ""],
            [.. GetDefaultExpectedColumns(submission4), "", false, false, false, true, "some free text", 2, Note1, Note2, 2, Attachment1Url, Attachment2Url],
        ];

        // Act
        var result = FormSubmissionsDataTable
            .FromForm(form)
            .WithData()
            .ForSubmission(submission1)
            .ForSubmission(submission2)
            .ForSubmission(submission3)
            .ForSubmission(submission4)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _submissionColumns,
            "SC - Question text",
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

        result.Should().NotBeNull();
        result.header.Should().ContainInOrder(expectedColumns);
        result.dataTable.Should().HaveCount(4);

        result.dataTable[0].Should().ContainInOrder(expectedData[0]);
        result.dataTable[1].Should().ContainInOrder(expectedData[1]);
        result.dataTable[2].Should().ContainInOrder(expectedData[2]);
        result.dataTable[3].Should().ContainInOrder(expectedData[3]);
    }

    [Fact]
    public void FormSubmissionsDataTableGenerator_Should_Generates_Correct_DataTable_WhenMultiSelectAnswer()
    {
        // Arrange
        var form = Fake.Form(DefaultLanguageCode, _multiSelectQuestion);
        // No notes, no attachments
        SelectedOption[] submission1Selection = [
            SelectedOption.Create(_option4Id, "user written text"),
            SelectedOption.Create(_option2Id, ""),
            SelectedOption.Create(_option3Id, ""),
            SelectedOption.Create(_option1Id, ""),
        ];
        var submission1 = Fake.Submission(form.Id, MultiSelectAnswer.Create(_multiSelectQuestionId, submission1Selection), [], []);

        // No notes, 2 attachments
        SelectedOption[] submission2Selection = [SelectedOption.Create(_option4Id, "some written text")];
        var submission2 = Fake.Submission(form.Id, MultiSelectAnswer.Create(_multiSelectQuestionId, submission2Selection), [], FakeAttachmentsFor(_multiSelectQuestionId));

        // 2 notes, no attachments
        SelectedOption[] submission3Selection = [SelectedOption.Create(_option3Id, ""), SelectedOption.Create(_option2Id, "")];
        var submission3 = Fake.Submission(form.Id, MultiSelectAnswer.Create(_multiSelectQuestionId, submission3Selection), FakeNotesFor(_multiSelectQuestionId), []);

        // 2 notes, 2 attachments
        SelectedOption[] submission4Selection = [SelectedOption.Create(_option4Id, "some free text"), SelectedOption.Create(_option1Id, "")];
        var submission4 = Fake.Submission(form.Id, MultiSelectAnswer.Create(_multiSelectQuestionId, submission4Selection), FakeNotesFor(_multiSelectQuestionId), FakeAttachmentsFor(_multiSelectQuestionId));

        List<object[]> expectedData =
        [
            [.. GetDefaultExpectedColumns(submission1), "", true, true, true, true, "user written text", 0, "", "", 0, "", ""],
            [.. GetDefaultExpectedColumns(submission2), "", false, false, false, true, "some written text", 0, "", "", 2, Attachment1Url, Attachment2Url],
            [.. GetDefaultExpectedColumns(submission3), "", false, true, true, false, "", 2, Note1, Note2, 0, "", ""],
            [.. GetDefaultExpectedColumns(submission4), "", true, false, false, true, "some free text", 2, Note1, Note2, 2, Attachment1Url, Attachment2Url],
        ];

        // Act
        var result = FormSubmissionsDataTable
            .FromForm(form)
            .WithData()
            .ForSubmission(submission1)
            .ForSubmission(submission2)
            .ForSubmission(submission3)
            .ForSubmission(submission4)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _submissionColumns,
            "MC - Question text",
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

        result.Should().NotBeNull();
        result.header.Should().ContainInOrder(expectedColumns);
        result.dataTable.Should().HaveCount(4);

        result.dataTable[0].Should().ContainInOrder(expectedData[0]);
        result.dataTable[1].Should().ContainInOrder(expectedData[1]);
        result.dataTable[2].Should().ContainInOrder(expectedData[2]);
        result.dataTable[3].Should().ContainInOrder(expectedData[3]);
    }


    [Fact]
    public void FormSubmissionsDataTableGenerator_Should_Generates_Correct_DataTable_WhenMultipleQuestions()
    {
        // Arrange
        var form = Fake.Form(DefaultLanguageCode, _textQuestion,
            _numberQuestion,
            _ratingQuestion,
            _dateQuestion,
            _singleSelectQuestion,
            _multiSelectQuestion);

        SelectedOption[] selection = [
            SelectedOption.Create(_option4Id, "user written text"),
            SelectedOption.Create(_option2Id, ""),
            SelectedOption.Create(_option3Id, ""),
            SelectedOption.Create(_option1Id, ""),
        ];

        var submission1 = Fake.PartialSubmission(form.Id,
         dateAnswer: (DateAnswer.Create(_dateQuestionId, _utcNow), FakeNotesFor(_dateQuestionId), FakeAttachmentsFor(_dateQuestionId)),
          numberAnswer: (NumberAnswer.Create(_numberQuestionId, 42), FakeNotesFor(_numberQuestionId), FakeAttachmentsFor(_numberQuestionId)),
        ratingAnswer: (RatingAnswer.Create(_ratingQuestionId, 3), FakeNotesFor(_ratingQuestionId), FakeAttachmentsFor(_ratingQuestionId)),
         singleSelectAnswer: (SingleSelectAnswer.Create(_singleSelectQuestionId, SelectedOption.Create(_option4Id, "user written text")), FakeNotesFor(_singleSelectQuestionId), FakeAttachmentsFor(_singleSelectQuestionId)),
         multiSelectAnswer: (MultiSelectAnswer.Create(_multiSelectQuestionId, selection), FakeNotesFor(_multiSelectQuestionId), FakeAttachmentsFor(_multiSelectQuestionId))
            );

        var submission2 = Fake.PartialSubmission(form.Id,
        textAnswer: (TextAnswer.Create(_textQuestionId, "some answer"), FakeNotesFor(_textQuestionId), FakeAttachmentsFor(_textQuestionId)),
       dateAnswer: (DateAnswer.Create(_dateQuestionId, _utcNow), FakeNotesFor(_dateQuestionId), FakeAttachmentsFor(_dateQuestionId)),
       numberAnswer: (NumberAnswer.Create(_numberQuestionId, 42), FakeNotesFor(_numberQuestionId), FakeAttachmentsFor(_numberQuestionId)),
      singleSelectAnswer: (SingleSelectAnswer.Create(_singleSelectQuestionId, SelectedOption.Create(_option4Id, "user written text")), FakeNotesFor(_singleSelectQuestionId), FakeAttachmentsFor(_singleSelectQuestionId)),
       multiSelectAnswer: (MultiSelectAnswer.Create(_multiSelectQuestionId, selection), FakeNotesFor(_multiSelectQuestionId), FakeAttachmentsFor(_multiSelectQuestionId))
        );

        var submission3 = Fake.PartialSubmission(form.Id,
       textAnswer: (TextAnswer.Create(_textQuestionId, "some answer"), FakeNotesFor(_textQuestionId), FakeAttachmentsFor(_textQuestionId)),
      dateAnswer: (DateAnswer.Create(_dateQuestionId, _utcNow), FakeNotesFor(_dateQuestionId), FakeAttachmentsFor(_dateQuestionId)),
      numberAnswer: (NumberAnswer.Create(_numberQuestionId, 42), FakeNotesFor(_numberQuestionId), FakeAttachmentsFor(_numberQuestionId)),
    ratingAnswer: (RatingAnswer.Create(_ratingQuestionId, 3), FakeNotesFor(_ratingQuestionId), FakeAttachmentsFor(_ratingQuestionId)),
        multiSelectAnswer: (MultiSelectAnswer.Create(_multiSelectQuestionId, selection), FakeNotesFor(_multiSelectQuestionId), FakeAttachmentsFor(_multiSelectQuestionId))
        );

        object[] expectedTextAnswerColumns = ["some answer", 2, Note1, Note2, 2, Attachment1Url, Attachment2Url];
        object[] expectedNumberAnswerColumns = [42, 2, Note1, Note2, 2, Attachment1Url, Attachment2Url];
        object[] expectedRatingAnswerColumns = [3, 2, Note1, Note2, 2, Attachment1Url, Attachment2Url];
        object[] expectedDateAnswerColumns = [_utcNow.ToString("s"), 2, Note1, Note2, 2, Attachment1Url, Attachment2Url];

        object[] expectedSingleSelectAnswerColumns =
            ["", false, false, false, true, "user written text", 2, Note1, Note2, 2, Attachment1Url, Attachment2Url];
        object[] expectedMultiSelectAnswerColumns =
            ["", true, true, true, true, "user written text", 2, Note1, Note2, 2, Attachment1Url, Attachment2Url];

        List<object[]> expectedData = [

            [
                .. GetDefaultExpectedColumns(submission1),
                "", "", "", "", "", "", "",
                .. expectedNumberAnswerColumns,
                .. expectedRatingAnswerColumns,
                .. expectedDateAnswerColumns,
                .. expectedSingleSelectAnswerColumns,
                .. expectedMultiSelectAnswerColumns
            ],

            [
                .. GetDefaultExpectedColumns(submission2),
                .. expectedTextAnswerColumns,
                .. expectedNumberAnswerColumns,
                "", "", "", "", "", "", "",
                .. expectedDateAnswerColumns,
                .. expectedSingleSelectAnswerColumns,
                .. expectedMultiSelectAnswerColumns
            ],

            [
                .. GetDefaultExpectedColumns(submission3),
                .. expectedTextAnswerColumns,
                .. expectedNumberAnswerColumns,
                .. expectedRatingAnswerColumns,
                .. expectedDateAnswerColumns,
                "", "", "", "", "", "", "", "", "", "", "", "",
                .. expectedMultiSelectAnswerColumns
            ]
        ];

        // Act
        var result = FormSubmissionsDataTable
            .FromForm(form)
            .WithData()
            .ForSubmission(submission1)
            .ForSubmission(submission2)
            .ForSubmission(submission3)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _submissionColumns,
            // text question columns
            "TQ - Question text",
            "Notes",
            "Note 1",
            "Note 2",
            "Attachments",
            "Attachment 1",
            "Attachment 2",
            // number answer columns
            "NQ - Question text",
            "Notes",
            "Note 1",
            "Note 2",
            "Attachments",
            "Attachment 1",
            "Attachment 2",
            // rating answer columns
            "RQ - Question text",
            "Notes",
            "Note 1",
            "Note 2",
            "Attachments",
            "Attachment 1",
            "Attachment 2",
            // Date question columns
            "DQ - Question text",
            "Notes",
            "Note 1",
            "Note 2",
            "Attachments",
            "Attachment 1",
            "Attachment 2",
            // Single select question columns
            "SC - Question text",
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
            // Multi select question columns
            "MC - Question text",
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

        result.Should().NotBeNull();
        result.header.Should().ContainInOrder(expectedColumns);
        result.dataTable.Should().HaveCount(3);

        result.dataTable[0].Should().ContainInOrder(expectedData[0]);
        result.dataTable[1].Should().ContainInOrder(expectedData[1]);
        result.dataTable[2].Should().ContainInOrder(expectedData[2]);
    }

    [Fact]
    public void FormSubmissionsDataTableGenerator_Should_Generates_Correct_DataTable_WhenMultipleQuestions_AndEmptyResponses()
    {
        // Arrange
        var form = Fake.Form(DefaultLanguageCode, _textQuestion,
            _numberQuestion,
            _ratingQuestion,
            _dateQuestion,
            _singleSelectQuestion,
            _multiSelectQuestion);

        var submission = Fake.Submission(form.Id);

        object[] expectedTextAnswerColumns = ["", "", ""];
        object[] expectedNumberAnswerColumns = ["", "", ""];
        object[] expectedRatingAnswerColumns = ["", "", ""];
        object[] expectedDateAnswerColumns = ["", "", ""];

        object[] expectedSingleSelectAnswerColumns =
            ["", "", "", "", "", "", "", ""];
        object[] expectedMultiSelectAnswerColumns =
            ["", "", "", "", "", "", "", ""];

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
        var result = FormSubmissionsDataTable
            .FromForm(form)
            .WithData()
            .ForSubmission(submission)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _submissionColumns,
            // text question columns
            "TQ - Question text",
            "Notes",
            "Attachments",
            // number answer columns
            "NQ - Question text",
            "Notes",
            "Attachments",
            // rating answer columns
            "RQ - Question text",
            "Notes",
            "Attachments",
            // Date question columns
            "DQ - Question text",
            "Notes",
            "Attachments",
            // Single select question columns
            "SC - Question text",
            Option1Text,
            Option2Text,
            Option3Text,
            Option4Text,
            Option4Text + "-UserInput",
            "Notes",
            "Attachments",
            // Multi select question columns
            "MC - Question text",
            Option1Text,
            Option2Text,
            Option3Text,
            Option4Text,
            Option4Text + "-UserInput",
            "Notes",
            "Attachments",
        ];

        result.Should().NotBeNull();
        result.header.Should().ContainInOrder(expectedColumns);
        result.dataTable.Should().HaveCount(1);

        result.dataTable[0].Should().ContainInOrder(expectedData);
    }

    private object[] GetDefaultExpectedColumns(SubmissionModel submission)
    {
        return
        [
            submission.SubmissionId.ToString(),
            submission.TimeSubmitted.ToString("s"),
            submission.FollowUpStatus.Value,
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

    private SubmissionAttachmentModel[] FakeAttachmentsFor(Guid questionId)
    {
        return
        [
            new SubmissionAttachmentModel { QuestionId = questionId, PresignedUrl = Attachment1Url },
            new SubmissionAttachmentModel { QuestionId = questionId, PresignedUrl = Attachment2Url },
        ];
    }
}
