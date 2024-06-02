using Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions;

public class FormSubmissionsDataTableGenerator
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;
    private readonly Guid _formId;

    private readonly Dictionary<Guid, AnswerWriter> _answerWriters;
    private readonly List<SubmissionModel> _submissions = [];

    private FormSubmissionsDataTableGenerator(List<string> header,
        List<List<object>> dataTable,
        Guid formId,
        List<AnswerWriter> answerWriters)
    {
        _header = header;
        _dataTable = dataTable;
        _formId = formId;
        _answerWriters = answerWriters.ToDictionary(x => x.QuestionId);
    }

    internal static FormSubmissionsDataTableGenerator For(List<string> header,
        List<List<object>> dataTable,
        Guid formId,
        List<AnswerWriter> answerWriters)
    {
        return new FormSubmissionsDataTableGenerator(header, dataTable, formId, answerWriters);
    }

    public FormSubmissionsDataTableGenerator ForSubmission(SubmissionModel submission)
    {
        if (submission.FormId != _formId)
        {
            // ignore submission if it is not for the current form
            return this;
        }

        MapToAnswerData(submission);

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
        _submissions.Add(submission);
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
            if (_answerWriters.TryGetValue(answer.QuestionId, out var writer))
            {
                writer.WithSubmission(submission.SubmissionId, answer, attachments, notes);
            }
        }
    }

    public (List<string> header, List<List<object>> dataTable) Please()
    {
        foreach (var writer in _answerWriters.Values)
        {
            _header.AddRange(writer.Header);
        }

        for (var index = 0; index < _submissions.Count; index++)
        {
            var submission = _submissions[index];
            foreach (var writer in _answerWriters.Values)
            {
                _dataTable[index].AddRange(writer.Write(submission.SubmissionId));
            }
        }

        return (_header, _dataTable);
    }

}
