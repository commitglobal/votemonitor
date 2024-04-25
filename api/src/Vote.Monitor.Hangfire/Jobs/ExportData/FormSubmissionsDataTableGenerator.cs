using System.Data;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Hangfire.Jobs.ExportData.ReadModels;
using static Vote.Monitor.Hangfire.Jobs.ExportData.FormSubmissionsDataTableGenerator;

namespace Vote.Monitor.Hangfire.Jobs.ExportData;

public class FormSubmissionsDataTable
{
    private readonly DataTable _dataTable;
    private readonly string _defaultLanguage;
    private FormSubmissionsDataTable(string formName, string defaultLanguage)
    {
        _dataTable = new DataTable(formName);
        _defaultLanguage = defaultLanguage;
    }

    public static FormSubmissionsDataTable CreateFor(string formName, string defaultLanguage)
    {
        return new FormSubmissionsDataTable(formName, defaultLanguage);
    }

    public FormSubmissionsDataTableHeaderGenerator WithHeader()
    {
        return FormSubmissionsDataTableHeaderGenerator.New(_dataTable, _defaultLanguage);
    }
}

public class FormSubmissionsDataTableHeaderGenerator
{
    private readonly DataTable _dataTable;
    private readonly string _defaultLanguage;

    private readonly SortedDictionary<int, Guid> _questionIndexMap = new();
    private readonly Dictionary<Guid, BaseQuestion> _questions = new();

    internal static FormSubmissionsDataTableHeaderGenerator New(DataTable dataTable, string defaultLanguage) => new(dataTable, defaultLanguage);

    private FormSubmissionsDataTableHeaderGenerator(DataTable dataTable, string defaultLanguage)
    {
        _dataTable = dataTable;
        _defaultLanguage = defaultLanguage;

        _dataTable.Columns.Add("SubmissionId", typeof(string));
        _dataTable.Columns.Add("TimeSubmitted", typeof(string));
        _dataTable.Columns.Add("Level1", typeof(string));
        _dataTable.Columns.Add("Level2", typeof(string));
        _dataTable.Columns.Add("Level3", typeof(string));
        _dataTable.Columns.Add("Level4", typeof(string));
        _dataTable.Columns.Add("Level5", typeof(string));
        _dataTable.Columns.Add("Number", typeof(string));
        _dataTable.Columns.Add("MonitoringObserverId", typeof(string));
        _dataTable.Columns.Add("FirstName", typeof(string));
        _dataTable.Columns.Add("LastName", typeof(string));
        _dataTable.Columns.Add("Email", typeof(string));
        _dataTable.Columns.Add("PhoneNumber", typeof(string));
        // TODO: add polling stations tags!
        // TODO: add observers tags?!
    }

    public FormSubmissionsDataTableHeaderGenerator WithQuestion(BaseQuestion question)
    {
        _questionIndexMap.Add(_questionIndexMap.Count, question.Id);
        _questions.Add(question.Id, question);

        return this;
    }

    public FormSubmissionsDataTableGenerator WithData()
    {
        return For(_dataTable, _defaultLanguage, _questions, _questionIndexMap);
    }
}

public class FormSubmissionsDataTableGenerator
{
    private readonly DataTable _dataTable;
    private readonly string _defaultLanguage;

    private readonly Dictionary<Guid, BaseQuestion> _questions;
    private readonly SortedDictionary<int, Guid> _questionIndexMap;
    private readonly List<SubmissionModel> _submissions;

    private FormSubmissionsDataTableGenerator(DataTable dataTable,
        string defaultLanguage,
        Dictionary<Guid, BaseQuestion> questions,
        SortedDictionary<int, Guid> questionIndexMap)
    {
        _dataTable = dataTable;
        _defaultLanguage = defaultLanguage;
        _questions = questions;
        _questionIndexMap = questionIndexMap;
        _submissions = new List<SubmissionModel>();
    }

    internal static FormSubmissionsDataTableGenerator For(DataTable dataTable,
        string defaultLanguage,
        Dictionary<Guid, BaseQuestion> questions,
        SortedDictionary<int, Guid> questionIndexMap)
    {
        return new FormSubmissionsDataTableGenerator(dataTable, defaultLanguage, questions, questionIndexMap);
    }

    public FormSubmissionsDataTableGenerator WithSubmission(SubmissionModel submission)
    {
        _submissions.Add(submission);

        var row = _dataTable.NewRow();

        row[0] = submission.SubmissionId.ToString();
        row[1] = submission.TimeSubmitted.ToString("O");
        row[2] = submission.Level1;
        row[3] = submission.Level2;
        row[4] = submission.Level3;
        row[5] = submission.Level4;
        row[6] = submission.Level5;
        row[7] = submission.Number;
        row[8] = submission.MonitoringObserverId.ToString();
        row[9] = submission.FirstName;
        row[10] = submission.LastName;
        row[11] = submission.Email;
        row[12] = submission.PhoneNumber;

        _dataTable.Rows.Add(row);
        return this;
    }

    public DataTable Please()
    {
        List<AnswerData> answersData = new List<AnswerData>();

        foreach (var submission in _submissions)
        {
            var answerNotes = submission
                .Notes
                .GroupBy(x => x.QuestionId)
                .ToDictionary(x => x.Key, y => y.ToList());

            var answerAttachments = submission
                .Attachments
                .GroupBy(x => x.QuestionId)
                .ToDictionary(x => x.Key, y => y.ToList());

            foreach (var answer in submission.Answers)
            {
                var notes = answerNotes.GetValueOrDefault(answer.QuestionId, []);
                var attachments = answerAttachments.GetValueOrDefault(answer.QuestionId, []);
                AnswerData answerData;
                switch (answer)
                {
                    case DateAnswer dateAnswer:
                        var dateQuestion = _questions[dateAnswer.QuestionId] as DateQuestion;
                        answerData = AnswerData.For(submission.SubmissionId, dateQuestion!, _defaultLanguage, dateAnswer, notes, attachments);
                        break;
                    case MultiSelectAnswer multiSelectAnswer:
                        var multiSelectQuestion = _questions[multiSelectAnswer.QuestionId] as MultiSelectQuestion;
                        answerData = AnswerData.For(submission.SubmissionId, multiSelectQuestion!, _defaultLanguage, multiSelectAnswer, notes, attachments);
                        break;
                    case NumberAnswer numberAnswer:
                        var numberQuestion = _questions[numberAnswer.QuestionId] as NumberQuestion;
                        answerData = AnswerData.For(submission.SubmissionId, numberQuestion!, _defaultLanguage, numberAnswer, notes, attachments);
                        break;
                    case RatingAnswer ratingAnswer:
                        var ratingQuestion = _questions[ratingAnswer.QuestionId] as RatingQuestion;
                        answerData = AnswerData.For(submission.SubmissionId, ratingQuestion!, _defaultLanguage, ratingAnswer, notes, attachments);
                        break;
                    case SingleSelectAnswer singleSelectAnswer:
                        var singleSelectQuestion = _questions[singleSelectAnswer.QuestionId] as SingleSelectQuestion;
                        answerData = AnswerData.For(submission.SubmissionId, singleSelectQuestion!, _defaultLanguage, singleSelectAnswer, notes, attachments);
                        break;
                    case TextAnswer textAnswer:
                        var textQuestion = _questions[textAnswer.QuestionId] as TextQuestion;
                        answerData = AnswerData.For(submission.SubmissionId, textQuestion!, _defaultLanguage, textAnswer, notes, attachments);
                        break;
                    default:
                        throw new ArgumentException($"Unknown answer type received {answer.Discriminator} in {submission.SubmissionId}", nameof(answer));
                }

                answersData.Add(answerData);
            }
        }

        var questionAnswers = answersData
            .GroupBy(x => x.QuestionId)
            .ToDictionary(x => x.Key, y => y.ToList());

        List<ValuesColumnHeader> longestValuesHeaders = [];
        List<string> longestNotesColumnHeaders = [];
        List<string> longestAttachmentUrlsColumnHeaders = [];

        // get the longest column headers
        foreach (var (_, questionId) in _questionIndexMap)
        {
            longestValuesHeaders = questionAnswers[questionId]
                .MaxBy(x => x.ValuesColumnHeaders.Count)
                ?.ValuesColumnHeaders ?? [];

            longestNotesColumnHeaders = questionAnswers[questionId]
                .MaxBy(x => x.NotesColumnHeaders.Count)
                ?.NotesColumnHeaders ?? [];

            longestAttachmentUrlsColumnHeaders = questionAnswers[questionId]
                .MaxBy(x => x.AttachmentUrlsColumnHeaders.Count)
                ?.AttachmentUrlsColumnHeaders ?? [];
        }

        // Add longest columns to data table
        foreach (var valuesHeader in longestValuesHeaders)
        {
            _dataTable.Columns.Add(valuesHeader.ColumnName, valuesHeader.ColumnType);
        }

        foreach (var noteColumnHeader in longestNotesColumnHeaders)
        {
            _dataTable.Columns.Add(noteColumnHeader, typeof(string));
        }

        foreach (var attachmentUrlsColumnHeader in longestAttachmentUrlsColumnHeaders)
        {
            _dataTable.Columns.Add(attachmentUrlsColumnHeader, typeof(string));
        }

        var submissionDataMap = answersData
            .GroupBy(x => x.SubmissionId)
            .ToDictionary(x => x.Key, y => y.ToList());

        for (int i = 0; i < _submissions.Count; i++)
        {
            var submissionId = _submissions[i].SubmissionId;
            var row = _dataTable.Rows[i];
            var submissionData = submissionDataMap[submissionId];
            var questionDataMap = submissionData.ToDictionary(x => x.QuestionId);
            foreach (var (_, questionId) in _questionIndexMap)
            {
                var questionsData = questionDataMap[questionId];

                var values = PadListToTheRight(questionsData.Values, longestValuesHeaders.Count, string.Empty);
                var notes = PadListToTheRight(questionsData.Notes, longestNotesColumnHeaders.Count, string.Empty);
                var attachmentsUrls = PadListToTheRight(questionsData.AttachmentUrls, longestAttachmentUrlsColumnHeaders.Count, string.Empty);

                var startIndex = row.ItemArray.Length;
                for (int j = 0; j < values.Count; j++)
                {
                    row[startIndex + j] = values[j];
                }

                startIndex = row.ItemArray.Length;
                for (int j = 0; j < notes.Count; j++)
                {
                    row[startIndex + j] = notes[j];
                }   
                
                startIndex = row.ItemArray.Length;
                for (int j = 0; j < attachmentsUrls.Count; j++)
                {
                    row[startIndex + j] = attachmentsUrls[j];
                }
            }

        }

        return _dataTable;
    }

    private List<T> PadListToTheRight<T>(List<T> list, int desiredLength, T padValue)
    {
        if (list.Count < desiredLength)
        {
            for (int i = 0; i < desiredLength - list.Count; i++)
            {
                list.Add(padValue);
            }
        }

        return list;
    }

    internal class AnswerData
    {
        public Guid SubmissionId { get; }
        public Guid QuestionId { get; }
        public List<object> Values { get; } = [];
        public List<string> Notes { get; } = [];
        public List<string> AttachmentUrls { get; } = [];
        public List<ValuesColumnHeader> ValuesColumnHeaders { get; } = [];
        public List<string> NotesColumnHeaders { get; } = [];
        public List<string> AttachmentUrlsColumnHeaders { get; } = [];

        private AnswerData(Guid submissionId, BaseQuestion question)
        {
            SubmissionId = submissionId;
            QuestionId = question.Id;
            Values = [];
            Notes = [];
            AttachmentUrls = [];
            ValuesColumnHeaders = [];
            NotesColumnHeaders = [];
            AttachmentUrlsColumnHeaders = [];
        }

        private void WithNotes(List<string> notes)
        {
            NotesColumnHeaders.Add("Notes");
            Notes.Add(string.Empty);

            for (int i = 0; i < notes.Count; i++)
            {
                NotesColumnHeaders.Add($"Note {i + 1}");
                Notes.Add(notes[i]);
            }
        }

        private void WithAttachments(List<AttachmentModel> attachments)
        {
            AttachmentUrlsColumnHeaders.Add("Attachments");
            AttachmentUrls.Add(string.Empty);

            for (int i = 0; i < attachments.Count; i++)
            {
                AttachmentUrlsColumnHeaders.Add($"Attachment {i + 1}");
                AttachmentUrls.Add(attachments[i].PresignedUrl);
            }
        }

        private void WithValue(string columnHeader, string value)
        {
            ValuesColumnHeaders.Add(new(columnHeader, typeof(string)));
            Values.Add(value);
        }

        private void WithValue(string columnHeader, int value)
        {
            ValuesColumnHeaders.Add(new(columnHeader, typeof(int)));
            Values.Add(value);
        }

        private void WithValue(string columnHeader, bool value)
        {
            ValuesColumnHeaders.Add(new(columnHeader, typeof(bool)));
            Values.Add(value);
        }

        private void WithValue(string columnHeader, DateTime date)
        {
            ValuesColumnHeaders.Add(new(columnHeader, typeof(string)));
            Values.Add(date.ToString("s"));
        }

        public static AnswerData For(Guid submissionId,
            DateQuestion dateQuestion,
            string defaultLanguage,
            DateAnswer dateAnswer,
            List<NoteModel> notes,
            List<AttachmentModel> attachments)
        {
            var answerData = new AnswerData(submissionId, dateQuestion);
            var columnHeader = dateQuestion.Code + dateQuestion.Text[defaultLanguage];

            answerData.WithValue(columnHeader, dateAnswer.Date);
            answerData.WithNotes(notes.Select(x => x.Text).ToList());
            answerData.WithAttachments(attachments);

            return answerData;
        }

        public static AnswerData For(Guid submissionId,
            TextQuestion textQuestion,
            string defaultLanguage,
            TextAnswer textAnswer,
            List<NoteModel> notes,
            List<AttachmentModel> attachments)
        {
            var answerData = new AnswerData(submissionId, textQuestion);

            var columnHeader = textQuestion.Code + textQuestion.Text[defaultLanguage];

            answerData.WithValue(columnHeader, textAnswer.Text);
            answerData.WithNotes(notes.Select(x => x.Text).ToList());
            answerData.WithAttachments(attachments);

            return answerData;
        }

        public static AnswerData For(Guid submissionId,
            RatingQuestion ratingQuestion,
            string defaultLanguage,
            RatingAnswer ratingAnswer,
            List<NoteModel> notes,
            List<AttachmentModel> attachments)
        {
            var answerData = new AnswerData(submissionId, ratingQuestion);
            var columnHeader = ratingQuestion.Code + ratingQuestion.Text[defaultLanguage];

            answerData.WithValue(columnHeader, ratingAnswer.Value);
            answerData.WithNotes(notes.Select(x => x.Text).ToList());
            answerData.WithAttachments(attachments);

            return answerData;
        }

        public static AnswerData For(Guid submissionId,
            NumberQuestion numberQuestion,
            string defaultLanguage,
            NumberAnswer numberAnswer,
            List<NoteModel> notes,
            List<AttachmentModel> attachments)
        {
            var answerData = new AnswerData(submissionId, numberQuestion);
            var columnHeader = numberQuestion.Code + numberQuestion.Text[defaultLanguage];

            answerData.WithValue(columnHeader, numberAnswer.Value);
            answerData.WithNotes(notes.Select(x => x.Text).ToList());
            answerData.WithAttachments(attachments);

            return answerData;
        }

        public static AnswerData For(Guid submissionId,
            SingleSelectQuestion singleSelectQuestion,
            string defaultLanguage,
            SingleSelectAnswer singleSelectAnswer,
            List<NoteModel> notes,
            List<AttachmentModel> attachments)
        {
            var answerData = new AnswerData(submissionId, singleSelectQuestion);
            var columnHeader = singleSelectQuestion.Code + singleSelectQuestion.Text[defaultLanguage];

            answerData.WithValue(columnHeader, string.Empty);

            foreach (var option in singleSelectQuestion.Options)
            {
                var selectedOption = singleSelectAnswer.Selection;
                answerData.WithValue(option.Text[defaultLanguage], selectedOption.OptionId == option.Id);

                if (option.IsFreeText)
                {
                    answerData.WithValue(option.Text[defaultLanguage] + "-UserInput", selectedOption?.Text ?? string.Empty);
                }
            }

            answerData.WithNotes(notes.Select(x => x.Text).ToList());
            answerData.WithAttachments(attachments);

            return answerData;
        }

        public static AnswerData For(Guid submissionId,
            MultiSelectQuestion multiSelectQuestion,
            string defaultLanguage,
            MultiSelectAnswer multiSelectAnswer,
            List<NoteModel> notes,
            List<AttachmentModel> attachments)
        {
            var answerData = new AnswerData(submissionId, multiSelectQuestion);
            var columnHeader = multiSelectQuestion.Code + multiSelectQuestion.Text[defaultLanguage];

            answerData.WithValue(columnHeader, string.Empty);

            foreach (var option in multiSelectQuestion.Options)
            {
                var selectedOption = multiSelectAnswer.Selection.FirstOrDefault(x => x.OptionId == option.Id);
                answerData.WithValue(option.Text[defaultLanguage], selectedOption != null);

                if (option.IsFreeText)
                {
                    answerData.WithValue(option.Text[defaultLanguage] + "-UserInput", selectedOption?.Text ?? string.Empty);
                }
            }

            answerData.WithNotes(notes.Select(x => x.Text).ToList());
            answerData.WithAttachments(attachments);

            return answerData;
        }
    }

    internal record ValuesColumnHeader(string ColumnName, Type ColumnType);
}
