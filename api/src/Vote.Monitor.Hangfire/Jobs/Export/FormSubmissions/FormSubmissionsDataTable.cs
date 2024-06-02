using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions;

public class FormSubmissionsDataTable
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;
    private readonly Guid _formId;
    private readonly string _defaultLanguage;
    private readonly List<AnswerWriter> _answerWriters;

    private FormSubmissionsDataTable(Guid formId, string defaultLanguage, IReadOnlyList<BaseQuestion> questions)
    {
        _header = new List<string>();
        _dataTable = new List<List<object>>();
        _formId = formId;
        _defaultLanguage = defaultLanguage;
        _answerWriters = questions.Select(x => new AnswerWriter(defaultLanguage, x)).ToList();

        _header.AddRange([
            "SubmissionId",
            "TimeSubmitted",
            "FollowUpStatus",
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

    public static FormSubmissionsDataTable FromForm(PollingStationInformationForm psiForm)
    {
        return new FormSubmissionsDataTable(psiForm.Id, psiForm.DefaultLanguage, psiForm.Questions);
    }

    public static FormSubmissionsDataTable FromForm(Form form)
    {
        return new FormSubmissionsDataTable(form.Id, form.DefaultLanguage, form.Questions);
    }

    public FormSubmissionsDataTableGenerator WithData()
    {
        return FormSubmissionsDataTableGenerator.For(_header, _dataTable, _formId, _answerWriters);
    }
}
