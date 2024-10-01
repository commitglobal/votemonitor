using Vote.Monitor.Hangfire.Jobs.Export.CitizenReports.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.CitizenReports;

public class CitizenReportsDataTableGenerator
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;
    private readonly Guid _formId;

    private readonly Dictionary<Guid, AnswerWriter> _answerWriters;
    private readonly List<CitizenReportModel> _citizenReports = [];

    private CitizenReportsDataTableGenerator(List<string> header,
        List<List<object>> dataTable,
        Guid formId,
        List<AnswerWriter> answerWriters)
    {
        _header = header;
        _dataTable = dataTable;
        _formId = formId;
        _answerWriters = answerWriters.ToDictionary(x => x.QuestionId);
    }

    internal static CitizenReportsDataTableGenerator For(List<string> header,
        List<List<object>> dataTable,
        Guid formId,
        List<AnswerWriter> answerWriters)
    {
        return new CitizenReportsDataTableGenerator(header, dataTable, formId, answerWriters);
    }

    public CitizenReportsDataTableGenerator For(params CitizenReportModel[] citizenReports)
    {
        foreach (var citizenReport in citizenReports)
        {
            if (citizenReport.FormId != _formId)
            {
                // ignore submission if it is not for the current form
                continue;
            }

            MapToAnswerData(citizenReport);

            var row = new List<object>
            {
                citizenReport.CitizenReportId.ToString(),
                citizenReport.TimeSubmitted.ToString("s"),
                citizenReport.FollowUpStatus.Value,
                citizenReport.Level1,
                citizenReport.Level2,
                citizenReport.Level3,
                citizenReport.Level4,
                citizenReport.Level5,
            };
            
            _dataTable.Add(row);
        }

        return this;
    }

    private void MapToAnswerData(CitizenReportModel citizenReport)
    {
        _citizenReports.Add(citizenReport);
        var answerNotes = citizenReport
            .Notes
            .GroupBy(x => x.QuestionId)
            .ToDictionary(x => x.Key, y => y.ToList());

        var answerAttachments = citizenReport
            .Attachments
            .GroupBy(x => x.QuestionId)
            .ToDictionary(x => x.Key, y => y.ToList());

        foreach (var answer in citizenReport.Answers)
        {
            var notes = answerNotes.GetValueOrDefault(answer.QuestionId, []);
            var attachments = answerAttachments.GetValueOrDefault(answer.QuestionId, []);
            if (_answerWriters.TryGetValue(answer.QuestionId, out var writer))
            {
                writer.WithSubmission(citizenReport.CitizenReportId, answer, attachments, notes);
            }
        }
    }

    public (List<string> header, List<List<object>> dataTable) Please()
    {
        foreach (var writer in _answerWriters.Values)
        {
            _header.AddRange(writer.Header);
        }

        for (var index = 0; index < _citizenReports.Count; index++)
        {
            var submission = _citizenReports[index];
            foreach (var writer in _answerWriters.Values)
            {
                _dataTable[index].AddRange(writer.Write(submission.CitizenReportId));
            }
        }

        return (_header, _dataTable);
    }

}
