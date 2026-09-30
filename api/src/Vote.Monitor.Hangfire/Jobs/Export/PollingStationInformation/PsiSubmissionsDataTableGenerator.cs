using Vote.Monitor.Hangfire.Jobs.Export.PollingStationInformation.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.PollingStationInformation;

public class PsiSubmissionsDataTableGenerator
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;
    private readonly Dictionary<Guid, PsiAnswerWriter> _answerWriters;
    private readonly List<PsiSubmissionModel> _submissions = [];

    private PsiSubmissionsDataTableGenerator(
        List<string> header,
        List<List<object>> dataTable,
        List<PsiAnswerWriter> answerWriters)
    {
        _header = header;
        _dataTable = dataTable;
        _answerWriters = answerWriters.ToDictionary(x => x.QuestionId);
    }

    internal static PsiSubmissionsDataTableGenerator For(
        List<string> header,
        List<List<object>> dataTable,
        List<PsiAnswerWriter> answerWriters) =>
        new(header, dataTable, answerWriters);

    public PsiSubmissionsDataTableGenerator ForSubmissions(IEnumerable<PsiSubmissionModel> submissions)
    {
        foreach (var submission in submissions)
        {
            ForSubmission(submission);
        }

        return this;
    }

    public PsiSubmissionsDataTableGenerator ForSubmission(PsiSubmissionModel submission)
    {
        foreach (var answer in submission.Answers)
        {
            if (_answerWriters.TryGetValue(answer.QuestionId, out var writer))
            {
                writer.WithSubmission(submission.SubmissionId, answer);
            }
        }

        _submissions.Add(submission);
        _dataTable.Add(
        [
            submission.SubmissionId.ToString(),
            submission.PollingStationId.ToString(),
            submission.Level1,
            submission.Level2,
            submission.Level3,
            submission.Level4,
            submission.Level5,
            submission.Number,
            submission.Address,
            submission.NgoName,
            submission.ArrivalTime?.ToString("s") ?? string.Empty,
            submission.DepartureTime?.ToString("s") ?? string.Empty,
            Math.Round(submission.MinutesMonitoring, 1),
            submission.NumberOfQuestionsAnswered,
            submission.NumberOfFlaggedAnswers,
            submission.IsCompleted,
            submission.LastUpdatedAt.ToString("s")
        ]);

        return this;
    }

    public (List<string> header, List<List<object>> dataTable) Please()
    {
        foreach (var writer in _answerWriters.Values)
        {
            _header.AddRange(writer.Header);
        }

        for (var index = 0; index < _submissions.Count; index++)
        {
            var submission = _submissions[index];
            foreach (var writer in _answerWriters.Values)
            {
                _dataTable[index].AddRange(writer.Write(submission.SubmissionId));
            }
        }

        return (_header, _dataTable);
    }
}
