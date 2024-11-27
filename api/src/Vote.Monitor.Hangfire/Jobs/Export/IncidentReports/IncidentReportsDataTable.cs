using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Hangfire.Jobs.Export.IncidentReports;

public class IncidentReportsDataTable
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;
    private readonly Guid _formId;
    private readonly List<AnswerWriter> _answerWriters;

    private IncidentReportsDataTable(Guid formId, string defaultLanguage, IReadOnlyList<BaseQuestion> questions)
    {
        _header = new List<string>();
        _dataTable = new List<List<object>>();
        _formId = formId;
        _answerWriters = questions.Select(question => new AnswerWriter(defaultLanguage, question)).ToList();

        _header.AddRange([
            "IncidentReportId",
            "TimeSubmitted",
            "FollowUpStatus",
            "LocationType",
            "LocationDescription",
            "Level1",
            "Level2",
            "Level3",
            "Level4",
            "Level5",
            "Number",
            "Ngo",
            "MonitoringObserverId",
            "Name",
            "Email",
            "PhoneNumber"
        ]);
    }

    public static IncidentReportsDataTable FromForm(Form form)
    {
        return new IncidentReportsDataTable(form.Id, form.DefaultLanguage, form.Questions);
    }

    public IncidentReportsDataTableGenerator WithData()
    {
        return IncidentReportsDataTableGenerator.For(_header, _dataTable, _formId, _answerWriters);
    }
}
