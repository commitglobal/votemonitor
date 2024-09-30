using Vote.Monitor.Hangfire.Jobs.Export.IssueReports.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.IssueReports;

public class IssueReportsDataTableGenerator
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;
    private readonly Guid _formId;

    private readonly Dictionary<Guid, AnswerWriter> _answerWriters;
    private readonly List<IssueReportModel> _issueReports = [];

    private IssueReportsDataTableGenerator(List<string> header,
        List<List<object>> dataTable,
        Guid formId,
        List<AnswerWriter> answerWriters)
    {
        _header = header;
        _dataTable = dataTable;
        _formId = formId;
        _answerWriters = answerWriters.ToDictionary(x => x.QuestionId);
    }

    internal static IssueReportsDataTableGenerator For(List<string> header,
        List<List<object>> dataTable,
        Guid formId,
        List<AnswerWriter> answerWriters)
    {
        return new IssueReportsDataTableGenerator(header, dataTable, formId, answerWriters);
    }

    public IssueReportsDataTableGenerator For(params IssueReportModel[] issueReports)
    {
        foreach (var issueReport in issueReports)
        {
            if (issueReport.FormId != _formId)
            {
                // ignore submission if it is not for the current form
                continue;
            }

            MapToAnswerData(issueReport);

            var row = new List<object>
            {
                issueReport.IssueReportId.ToString(),
                issueReport.TimeSubmitted.ToString("s"),
                issueReport.FollowUpStatus.Value,
                issueReport.LocationType.ToString(),

                issueReport.LocationDescription ?? "",
                issueReport.Level1 ?? "",
                issueReport.Level2 ?? "",
                issueReport.Level3 ?? "",
                issueReport.Level4 ?? "",
                issueReport.Level5 ?? "",
                issueReport.Number ?? "",

                issueReport.MonitoringObserverId.ToString(),
                issueReport.FirstName,
                issueReport.LastName,
                issueReport.Email,
                issueReport.PhoneNumber
            };

            _dataTable.Add(row);
        }

        return this;
    }

    private void MapToAnswerData(IssueReportModel issueReport)
    {
        _issueReports.Add(issueReport);
        var answerNotes = issueReport
            .Notes
            .GroupBy(x => x.QuestionId)
            .ToDictionary(x => x.Key, y => y.ToList());

        var answerAttachments = issueReport
            .Attachments
            .GroupBy(x => x.QuestionId)
            .ToDictionary(x => x.Key, y => y.ToList());

        foreach (var answer in issueReport.Answers)
        {
            var notes = answerNotes.GetValueOrDefault(answer.QuestionId, []);
            var attachments = answerAttachments.GetValueOrDefault(answer.QuestionId, []);
            if (_answerWriters.TryGetValue(answer.QuestionId, out var writer))
            {
                writer.WithSubmission(issueReport.IssueReportId, answer, attachments, notes);
            }
        }
    }

    public (List<string> header, List<List<object>> dataTable) Please()
    {
        foreach (var writer in _answerWriters.Values)
        {
            _header.AddRange(writer.Header);
        }

        for (var index = 0; index < _issueReports.Count; index++)
        {
            var submission = _issueReports[index];
            foreach (var writer in _answerWriters.Values)
            {
                _dataTable[index].AddRange(writer.Write(submission.IssueReportId));
            }
        }

        return (_header, _dataTable);
    }
}