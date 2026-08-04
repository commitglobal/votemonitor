using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions;

public class FormSubmissionsSimplifiedDataTable
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;
    private readonly Guid _formId;
    private readonly List<SimplifiedAnswerWriter> _answerWriters;

    private FormSubmissionsSimplifiedDataTable(Guid formId, string defaultLanguage, IReadOnlyList<BaseQuestion> questions)
    {
        _header = new List<string>();
        _dataTable = new List<List<object>>();
        _formId = formId;
        _answerWriters = questions.Select(question => new SimplifiedAnswerWriter(defaultLanguage, question)).ToList();

        _header.AddRange([
            "SubmissionId",
            "SubmissionNumber",
            "TimeSubmitted",
            "FollowUpStatus",
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

    public static FormSubmissionsSimplifiedDataTable FromForm(PollingStationInformationForm psiForm)
    {
        return new FormSubmissionsSimplifiedDataTable(psiForm.Id, psiForm.DefaultLanguage, psiForm.Questions);
    }

    public static FormSubmissionsSimplifiedDataTable FromForm(Form form)
    {
        return new FormSubmissionsSimplifiedDataTable(form.Id, form.DefaultLanguage, form.Questions);
    }

    public FormSubmissionsSimplifiedDataTableGenerator WithData()
    {
        return FormSubmissionsSimplifiedDataTableGenerator.For(_header, _dataTable, _formId, _answerWriters);
    }
}
