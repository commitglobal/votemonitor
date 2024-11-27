using Vote.Monitor.Hangfire.Jobs.Export.IncidentReports.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.IncidentReports;

public class IncidentReportsDataTableGenerator
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;
    private readonly Guid _formId;

    private readonly Dictionary<Guid, AnswerWriter> _answerWriters;
    private readonly List<IncidentReportModel> _incidentReports = [];

    private IncidentReportsDataTableGenerator(List<string> header,
        List<List<object>> dataTable,
        Guid formId,
        List<AnswerWriter> answerWriters)
    {
        _header = header;
        _dataTable = dataTable;
        _formId = formId;
        _answerWriters = answerWriters.ToDictionary(x => x.QuestionId);
    }

    internal static IncidentReportsDataTableGenerator For(List<string> header,
        List<List<object>> dataTable,
        Guid formId,
        List<AnswerWriter> answerWriters)
    {
        return new IncidentReportsDataTableGenerator(header, dataTable, formId, answerWriters);
    }

    public IncidentReportsDataTableGenerator For(params IncidentReportModel[] incidentReports)
    {
        foreach (var incidentReport in incidentReports)
        {
            if (incidentReport.FormId != _formId)
            {
                // ignore submission if it is not for the current form
                continue;
            }

            MapToAnswerData(incidentReport);

            var row = new List<object>
            {
                incidentReport.IncidentReportId.ToString(),
                incidentReport.TimeSubmitted.ToString("s"),
                incidentReport.FollowUpStatus.Value,
                incidentReport.LocationType.ToString(),

                incidentReport.LocationDescription ?? "",
                incidentReport.Level1 ?? "",
                incidentReport.Level2 ?? "",
                incidentReport.Level3 ?? "",
                incidentReport.Level4 ?? "",
                incidentReport.Level5 ?? "",
                incidentReport.Number ?? "",

                incidentReport.NgoName,
                incidentReport.MonitoringObserverId.ToString(),
                incidentReport.DisplayName,
                incidentReport.Email,
                incidentReport.PhoneNumber
            };

            _dataTable.Add(row);
        }

        return this;
    }

    private void MapToAnswerData(IncidentReportModel incidentReport)
    {
        _incidentReports.Add(incidentReport);
        var answerNotes = incidentReport
            .Notes
            .GroupBy(x => x.QuestionId)
            .ToDictionary(x => x.Key, y => y.ToList());

        var answerAttachments = incidentReport
            .Attachments
            .GroupBy(x => x.QuestionId)
            .ToDictionary(x => x.Key, y => y.ToList());

        foreach (var answer in incidentReport.Answers)
        {
            var notes = answerNotes.GetValueOrDefault(answer.QuestionId, []);
            var attachments = answerAttachments.GetValueOrDefault(answer.QuestionId, []);
            if (_answerWriters.TryGetValue(answer.QuestionId, out var writer))
            {
                writer.WithSubmission(incidentReport.IncidentReportId, answer, attachments, notes);
            }
        }
    }

    public (List<string> header, List<List<object>> dataTable) Please()
    {
        foreach (var writer in _answerWriters.Values)
        {
            _header.AddRange(writer.Header);
        }

        for (var index = 0; index < _incidentReports.Count; index++)
        {
            var submission = _incidentReports[index];
            foreach (var writer in _answerWriters.Values)
            {
                _dataTable[index].AddRange(writer.Write(submission.IncidentReportId));
            }
        }

        return (_header, _dataTable);
    }
}
