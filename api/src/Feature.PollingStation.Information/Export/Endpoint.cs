using System.Globalization;
using Authorization.Policies;
using CsvHelper;
using Dapper;
using Feature.PollingStation.Information.PsiData;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Feature.PollingStation.Information.Export;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory, VoteMonitorContext context) : Endpoint<Request>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/information:export");
        DontAutoTag();
        Options(x => x
            .WithTags("polling-station-information")
            .Produces(200, typeof(string), contentType: "text/csv"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Exports polling station information of an election round to CSV (platform admins)";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var sql = $"""
                   {PsiDataQuery.SelectColumns}
                   {PsiDataQuery.FromWhere}
                   {PsiDataQuery.OrderBy};
                   """;

        List<PsiDataRowModel> rows;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            rows = (await dbConnection.QueryAsync<PsiDataRowModel>(sql, PsiDataQuery.BuildArgs(req, paged: false)))
                .ToList();
        }

        var form = await context.PollingStationInformationForms
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ElectionRoundId == req.ElectionRoundId, ct);

        var questions = form?.Questions ?? [];
        var language = form?.DefaultLanguage ?? string.Empty;

        var rowIds = rows.Select(x => x.Id).ToList();
        var answersById = rowIds.Count == 0
            ? new Dictionary<Guid, IReadOnlyList<BaseAnswer>>()
            : (await context.PollingStationInformation
                    .AsNoTracking()
                    .Where(x => x.ElectionRoundId == req.ElectionRoundId && rowIds.Contains(x.Id))
                    .Select(x => new { x.Id, x.Answers })
                    .ToListAsync(ct))
                .ToDictionary(x => x.Id, x => x.Answers);

        using var memoryStream = new MemoryStream();
        await using (var streamWriter = new StreamWriter(memoryStream, leaveOpen: true))
        await using (var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
        {
            WriteHeader(csv, questions, language);
            foreach (var row in rows)
            {
                answersById.TryGetValue(row.Id, out var answers);
                WriteRow(csv, row, questions, answers ?? [], language);
            }

            await streamWriter.FlushAsync(ct);
        }

        await SendBytesAsync(memoryStream.ToArray(), $"psi-data-{req.ElectionRoundId}.csv", contentType: "text/csv",
            cancellation: ct);
    }

    private static readonly string[] BaseHeader =
    [
        "SubmissionId", "PollingStationId", "Level1", "Level2", "Level3", "Level4", "Level5", "Number", "Address",
        "Ngo", "ArrivalTime", "DepartureTime", "MinutesMonitoring", "QuestionsAnswered", "FlaggedAnswers",
        "IsCompleted", "LastUpdatedAt"
    ];

    private static void WriteHeader(CsvWriter csv, IReadOnlyList<BaseQuestion> questions, string language)
    {
        foreach (var column in BaseHeader)
        {
            csv.WriteField(column);
        }

        foreach (var question in questions)
        {
            csv.WriteField($"{question.Code} - {Translate(question.Text, language)}");
            if (HasFreeTextOption(question))
            {
                csv.WriteField($"{question.Code} - FreeText");
            }
        }

        csv.NextRecord();
    }

    private static void WriteRow(CsvWriter csv, PsiDataRowModel row, IReadOnlyList<BaseQuestion> questions,
        IReadOnlyList<BaseAnswer> answers, string language)
    {
        csv.WriteField(row.Id);
        csv.WriteField(row.PollingStationId);
        csv.WriteField(row.Level1);
        csv.WriteField(row.Level2);
        csv.WriteField(row.Level3);
        csv.WriteField(row.Level4);
        csv.WriteField(row.Level5);
        csv.WriteField(row.Number);
        csv.WriteField(row.Address);
        csv.WriteField(row.NgoName);
        csv.WriteField(row.ArrivalTime?.ToString("s"));
        csv.WriteField(row.DepartureTime?.ToString("s"));
        csv.WriteField(Math.Round(row.MinutesMonitoring, 1));
        csv.WriteField(row.NumberOfQuestionsAnswered);
        csv.WriteField(row.NumberOfFlaggedAnswers);
        csv.WriteField(row.IsCompleted);
        csv.WriteField(row.LastUpdatedAt.ToString("s"));

        var answersByQuestion = answers
            .GroupBy(x => x.QuestionId)
            .ToDictionary(x => x.Key, x => x.First());

        foreach (var question in questions)
        {
            answersByQuestion.TryGetValue(question.Id, out var answer);
            var (value, freeText) = FormatAnswer(question, answer, language);
            csv.WriteField(value);
            if (HasFreeTextOption(question))
            {
                csv.WriteField(freeText);
            }
        }

        csv.NextRecord();
    }

    private static (string Value, string FreeText) FormatAnswer(BaseQuestion question, BaseAnswer? answer,
        string language)
    {
        switch (answer)
        {
            case null:
                return (string.Empty, string.Empty);
            case TextAnswer text:
                return (text.Text ?? string.Empty, string.Empty);
            case NumberAnswer number:
                return (number.Value.ToString(CultureInfo.InvariantCulture), string.Empty);
            case RatingAnswer rating:
                return (rating.Value.ToString(CultureInfo.InvariantCulture), string.Empty);
            case DateAnswer date:
                return (date.Date.ToString("s"), string.Empty);
            case SingleSelectAnswer single:
            {
                var option = (question as SingleSelectQuestion)?.Options
                    .FirstOrDefault(x => x.Id == single.Selection.OptionId);
                return (option is null ? string.Empty : Translate(option.Text, language),
                    single.Selection.Text ?? string.Empty);
            }
            case MultiSelectAnswer multi:
            {
                var selectedIds = multi.Selection.Select(x => x.OptionId).ToHashSet();
                var options = (question as MultiSelectQuestion)?.Options
                    .Where(x => selectedIds.Contains(x.Id))
                    .Select(x => Translate(x.Text, language)) ?? [];
                var freeTexts = multi.Selection.Select(x => x.Text).Where(x => !string.IsNullOrWhiteSpace(x));
                return (string.Join(", ", options), string.Join(", ", freeTexts));
            }
            default:
                return (string.Empty, string.Empty);
        }
    }

    private static bool HasFreeTextOption(BaseQuestion question) => question switch
    {
        SingleSelectQuestion single => single.Options.Any(x => x.IsFreeText),
        MultiSelectQuestion multi => multi.Options.Any(x => x.IsFreeText),
        _ => false
    };

    private static string Translate(IReadOnlyDictionary<string, string>? text, string language)
    {
        if (text is null || text.Count == 0)
        {
            return string.Empty;
        }

        if (!string.IsNullOrEmpty(language) && text.TryGetValue(language, out var value))
        {
            return value;
        }

        return text.Values.First();
    }
}
