using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Hangfire.Jobs.Export.IncidentReports;

public class IncidentReportsSimplifiedDataTable
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;
    private readonly Guid _formId;
    private readonly List<SimplifiedAnswerWriter> _answerWriters;

    private IncidentReportsSimplifiedDataTable(Guid formId, string defaultLanguage, IReadOnlyList<BaseQuestion> questions)
    {
        _header = new List<string>();
        _dataTable = new List<List<object>>();
        _formId = formId;
        _answerWriters = questions.Select(question => new SimplifiedAnswerWriter(defaultLanguage, question)).ToList();

        _header.AddRange([
            "IncidentReportId",
            "TimeSubmitted",
            "FollowUpStatus",
            "Labels",
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
            "PhoneNumber",
            "Tags"
        ]);
    }

    public static IncidentReportsSimplifiedDataTable FromForm(Form form)
    {
        return new IncidentReportsSimplifiedDataTable(form.Id, form.DefaultLanguage, form.Questions);
    }

    public IncidentReportsSimplifiedDataTableGenerator WithData()
    {
        return IncidentReportsSimplifiedDataTableGenerator.For(_header, _dataTable, _formId, _answerWriters);
    }
}
