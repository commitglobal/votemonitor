using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Hangfire.Jobs.Export.PollingStationInformation;

public class PsiSubmissionsDataTable
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;
    private readonly List<PsiAnswerWriter> _answerWriters;

    private PsiSubmissionsDataTable(string defaultLanguage, IReadOnlyList<BaseQuestion> questions)
    {
        _header =
        [
            "SubmissionId",
            "PollingStationId",
            "Level1",
            "Level2",
            "Level3",
            "Level4",
            "Level5",
            "Number",
            "Address",
            "Ngo",
            "ArrivalTime",
            "DepartureTime",
            "MinutesMonitoring",
            "QuestionsAnswered",
            "FlaggedAnswers",
            "IsCompleted",
            "LastUpdatedAt"
        ];
        _dataTable = [];
        _answerWriters = questions.Select(q => new PsiAnswerWriter(defaultLanguage, q)).ToList();
    }

    public static PsiSubmissionsDataTable FromForm(PollingStationInformationForm form) =>
        new(form.DefaultLanguage, form.Questions);

    public static PsiSubmissionsDataTable Empty() =>
        new(string.Empty, []);

    public PsiSubmissionsDataTableGenerator WithData() =>
        PsiSubmissionsDataTableGenerator.For(_header, _dataTable, _answerWriters);
}
