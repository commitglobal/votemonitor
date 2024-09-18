using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Hangfire.Jobs.Export.CitizenReports;

public class CitizenReportsDataTable
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;
    private readonly Guid _formId;
    private readonly List<AnswerWriter> _answerWriters;

    private CitizenReportsDataTable(Guid formId, string defaultLanguage, IReadOnlyList<BaseQuestion> questions)
    {
        _header = new List<string>();
        _dataTable = new List<List<object>>();
        _formId = formId;
        _answerWriters = questions.Select(question => new AnswerWriter(defaultLanguage, question)).ToList();

        _header.AddRange([
            "CitizenReportId",
            "TimeSubmitted",
            "FollowUpStatus",
            "Level1",
            "Level2",
            "Level3",
            "Level4",
            "Level5"
        ]);
    }

    public static CitizenReportsDataTable FromForm(PollingStationInformationForm psiForm)
    {
        return new CitizenReportsDataTable(psiForm.Id, psiForm.DefaultLanguage, psiForm.Questions);
    }

    public static CitizenReportsDataTable FromForm(Form form)
    {
        return new CitizenReportsDataTable(form.Id, form.DefaultLanguage, form.Questions);
    }

    public CitizenReportsDataTableGenerator WithData()
    {
        return CitizenReportsDataTableGenerator.For(_header, _dataTable, _formId, _answerWriters);
    }
}
