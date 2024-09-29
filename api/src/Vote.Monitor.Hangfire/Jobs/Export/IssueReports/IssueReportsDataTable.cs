using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Hangfire.Jobs.Export.IssueReports;

public class IssueReportsDataTable
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;
    private readonly Guid _formId;
    private readonly List<AnswerWriter> _answerWriters;

    private IssueReportsDataTable(Guid formId, string defaultLanguage, IReadOnlyList<BaseQuestion> questions)
    {
        _header = new List<string>();
        _dataTable = new List<List<object>>();
        _formId = formId;
        _answerWriters = questions.Select(question => new AnswerWriter(defaultLanguage, question)).ToList();

        _header.AddRange([
            "IssueReportId",
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
            "MonitoringObserverId",
            "FirstName",
            "LastName",
            "Email",
            "PhoneNumber"
        ]);
    }

    public static IssueReportsDataTable FromForm(PollingStationInformationForm psiForm)
    {
        return new IssueReportsDataTable(psiForm.Id, psiForm.DefaultLanguage, psiForm.Questions);
    }

    public static IssueReportsDataTable FromForm(Form form)
    {
        return new IssueReportsDataTable(form.Id, form.DefaultLanguage, form.Questions);
    }

    public IssueReportsDataTableGenerator WithData()
    {
        return IssueReportsDataTableGenerator.For(_header, _dataTable, _formId, _answerWriters);
    }
}