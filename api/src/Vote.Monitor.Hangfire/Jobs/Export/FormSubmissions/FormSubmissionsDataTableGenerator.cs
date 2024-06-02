using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions;

public class FormSubmissionsDataTableGenerator
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;
    private readonly Guid _formId;
    private readonly string _defaultLanguage;

    private readonly Dictionary<Guid, BaseQuestion> _questionsMap;
    private readonly IReadOnlyList<BaseQuestion> _questions;
    private readonly List<SubmissionModel> _submissions = [];
    private readonly List<AnswerData> _answersData = [];

    private FormSubmissionsDataTableGenerator(List<string> header,
        List<List<object>> dataTable,
        Guid formId,
        string defaultLanguage,
        Dictionary<Guid, BaseQuestion> questionsMap,
        IReadOnlyList<BaseQuestion> questions)
    {
        _header = header;
        _dataTable = dataTable;
        _formId = formId;
        _defaultLanguage = defaultLanguage;
        _questionsMap = questionsMap;
        _questions = questions;
    }

    internal static FormSubmissionsDataTableGenerator For(List<string> header,
        List<List<object>> dataTable,
        Guid formId,
        string defaultLanguage,
        Dictionary<Guid, BaseQuestion> questionsMap,
        IReadOnlyList<BaseQuestion> questions)
    {
        return new FormSubmissionsDataTableGenerator(header, dataTable, formId, defaultLanguage, questionsMap, questions);
    }

    public FormSubmissionsDataTableGenerator ForSubmission(SubmissionModel submission)
    {
        if (submission.FormId != _formId)
        {
            // ignore submission if it is not for the current form
            return this;
        }

        MapToAnswerData(submission);

        _submissions.Add(submission);

        var row = new List<object>
        {

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
           submission.PhoneNumber
        };

        _dataTable.Add(row);
        return this;
    }

    public FormSubmissionsDataTableGenerator ForSubmissions(List<SubmissionModel> submissions)
    {
        foreach (var submission in submissions)
        {
            ForSubmission(submission);
        }

        return this;
    }

    private void MapToAnswerData(SubmissionModel submission)
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
                    var dateQuestion = _questionsMap[dateAnswer.QuestionId] as DateQuestion;
                    answerData = AnswerData.For(submission.SubmissionId, dateQuestion!, _defaultLanguage, dateAnswer, notes, attachments);
                    break;
                case MultiSelectAnswer multiSelectAnswer:
                    var multiSelectQuestion = _questionsMap[multiSelectAnswer.QuestionId] as MultiSelectQuestion;
                    answerData = AnswerData.For(submission.SubmissionId, multiSelectQuestion!, _defaultLanguage, multiSelectAnswer, notes, attachments);
                    break;
                case NumberAnswer numberAnswer:
                    var numberQuestion = _questionsMap[numberAnswer.QuestionId] as NumberQuestion;
                    answerData = AnswerData.For(submission.SubmissionId, numberQuestion!, _defaultLanguage, numberAnswer, notes, attachments);
                    break;
                case RatingAnswer ratingAnswer:
                    var ratingQuestion = _questionsMap[ratingAnswer.QuestionId] as RatingQuestion;
                    answerData = AnswerData.For(submission.SubmissionId, ratingQuestion!, _defaultLanguage, ratingAnswer, notes, attachments);
                    break;
                case SingleSelectAnswer singleSelectAnswer:
                    var singleSelectQuestion = _questionsMap[singleSelectAnswer.QuestionId] as SingleSelectQuestion;
                    answerData = AnswerData.For(submission.SubmissionId, singleSelectQuestion!, _defaultLanguage, singleSelectAnswer, notes, attachments);
                    break;
                case TextAnswer textAnswer:
                    var textQuestion = _questionsMap[textAnswer.QuestionId] as TextQuestion;
                    answerData = AnswerData.For(submission.SubmissionId, textQuestion!, _defaultLanguage, textAnswer, notes, attachments);
                    break;
                default:
                    throw new ArgumentException($"Unknown answer type received {answer.Discriminator} in {submission.SubmissionId}", nameof(answer));
            }

            _answersData.Add(answerData);
        }
    }

    public (List<string> header, List<List<object>> dataTable) Please()
    {
        var questionAnswers = _answersData
            .GroupBy(x => x.QuestionId)
            .ToDictionary(x => x.Key, y => y.ToList());

        Dictionary<Guid, List<string>> longestValuesHeadersMap = new();
        Dictionary<Guid, List<string>> longestNotesColumnHeadersMap = new();
        Dictionary<Guid, List<string>> longestAttachmentUrlsColumnHeadersMap = new();

        // get the longest column headers
        foreach (var question in _questions)
        {
            var answers = questionAnswers.GetValueOrDefault(question.Id, []);

            var longestValuesColumnHeader = answers
                .MaxBy(x => x.ValuesColumnHeaders.Count)
                ?.ValuesColumnHeaders ?? [];

            var longestNotesHeader = answers
                .MaxBy(x => x.NotesColumnHeaders.Count)
                ?.NotesColumnHeaders ?? [];

            var longestAttachmentUrlsColumnHeader = answers
                .MaxBy(x => x.AttachmentUrlsColumnHeaders.Count)
                ?.AttachmentUrlsColumnHeaders ?? [];

            _header.AddRange(longestValuesColumnHeader);
            _header.AddRange(longestNotesHeader);
            _header.AddRange(longestAttachmentUrlsColumnHeader);

            longestValuesHeadersMap.Add(question.Id, longestValuesColumnHeader);
            longestNotesColumnHeadersMap.Add(question.Id, longestNotesHeader);
            longestAttachmentUrlsColumnHeadersMap.Add(question.Id, longestAttachmentUrlsColumnHeader);
        }

        var submissionDataMap = _answersData
            .GroupBy(x => x.SubmissionId)
            .ToDictionary(x => x.Key, y => y.ToList());

        for (int i = 0; i < _submissions.Count; i++)
        {
            var submissionId = _submissions[i].SubmissionId;
            var row = _dataTable[i];
            var submissionData = submissionDataMap.GetValueOrDefault(submissionId, []);
            var questionDataMap = submissionData.ToDictionary(x => x.QuestionId);

            foreach (var question in _questions)
            {
                var questionsData = questionDataMap.GetValueOrDefault(question.Id, AnswerData.Empty());

                var values = PadListToTheRight(questionsData.Values, longestValuesHeadersMap[question.Id].Count, string.Empty);
                var notes = PadListToTheRight(questionsData.Notes, longestNotesColumnHeadersMap[question.Id].Count, string.Empty);
                var attachmentsUrls = PadListToTheRight(questionsData.AttachmentUrls, longestAttachmentUrlsColumnHeadersMap[question.Id].Count, string.Empty);

                row.AddRange(values);
                row.AddRange(notes);
                row.AddRange(attachmentsUrls);
            }

        }

        return (_header, _dataTable);
    }

    private List<T> PadListToTheRight<T>(List<T> list, int desiredLength, T padValue)
    {
        if (list.Count < desiredLength)
        {
            var numberOfMissingElements = desiredLength - list.Count;
            for (int i = 0; i < numberOfMissingElements; i++)
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
        public List<string> ValuesColumnHeaders { get; } = [];
        public List<string> NotesColumnHeaders { get; } = [];
        public List<string> AttachmentUrlsColumnHeaders { get; } = [];

        private AnswerData(Guid submissionId, Guid questionId)
        {
            SubmissionId = submissionId;
            QuestionId = questionId;
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

        private void WithAttachments(List<SubmissionAttachmentModel> attachments)
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
            ValuesColumnHeaders.Add(columnHeader);
            Values.Add(value);
        }

        private void WithValue(string columnHeader, int value)
        {
            ValuesColumnHeaders.Add(columnHeader);
            Values.Add(value);
        }

        private void WithValue(string columnHeader, bool value)
        {
            ValuesColumnHeaders.Add(columnHeader);
            Values.Add(value);
        }

        private void WithValue(string columnHeader, DateTime date)
        {
            ValuesColumnHeaders.Add(columnHeader);
            Values.Add(date.ToString("s"));
        }

        public static AnswerData For(Guid submissionId,
            DateQuestion dateQuestion,
            string defaultLanguage,
            DateAnswer dateAnswer,
            List<NoteModel> notes,
            List<SubmissionAttachmentModel> attachments)
        {
            var answerData = new AnswerData(submissionId, dateQuestion.Id);
            var columnHeader = dateQuestion.Code + " - " + dateQuestion.Text[defaultLanguage];

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
            List<SubmissionAttachmentModel> attachments)
        {
            var answerData = new AnswerData(submissionId, textQuestion.Id);

            var columnHeader = textQuestion.Code + " - " + textQuestion.Text[defaultLanguage];

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
            List<SubmissionAttachmentModel> attachments)
        {
            var answerData = new AnswerData(submissionId, ratingQuestion.Id);
            var columnHeader = ratingQuestion.Code + " - " + ratingQuestion.Text[defaultLanguage];

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
            List<SubmissionAttachmentModel> attachments)
        {
            var answerData = new AnswerData(submissionId, numberQuestion.Id);
            var columnHeader = numberQuestion.Code + " - " + numberQuestion.Text[defaultLanguage];

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
            List<SubmissionAttachmentModel> attachments)
        {
            var answerData = new AnswerData(submissionId, singleSelectQuestion.Id);
            var columnHeader = singleSelectQuestion.Code + " - " + singleSelectQuestion.Text[defaultLanguage];

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
            List<SubmissionAttachmentModel> attachments)
        {
            var answerData = new AnswerData(submissionId, multiSelectQuestion.Id);
            var columnHeader = multiSelectQuestion.Code + " - " + multiSelectQuestion.Text[defaultLanguage];

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

        public static AnswerData Empty()
        {
            return new AnswerData(Guid.Empty, Guid.Empty);
        }
    }
}
