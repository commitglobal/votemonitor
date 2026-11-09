using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.Entities.AnswerHistoryAggregate;

/// <summary>
/// One answer of a record (form submission or incident report) its observer changed after sending it: the question,
/// what it said before and what it says now (null when the answer was added or cleared), and when. Written once per
/// changed question each time the record's answers change.
/// </summary>
public class AnswerChange : IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public ReportRecordType RecordType { get; private set; }
    public Guid RecordId { get; private set; }
    public Guid QuestionId { get; private set; }
    /// <summary>The answer before, as JSON (its <c>$answerType</c> picks the type back); null when it was added.</summary>
    public string? PreviousAnswerJson { get; private set; }

    /// <summary>The answer after, as JSON; null when it was cleared.</summary>
    public string? CurrentAnswerJson { get; private set; }

    public DateTime ChangedAt { get; private set; }

    public BaseAnswer? PreviousAnswer => Read(PreviousAnswerJson);
    public BaseAnswer? CurrentAnswer => Read(CurrentAnswerJson);

    private static BaseAnswer? Read(string? json) => json is null ? null : JsonSerializer.Deserialize<BaseAnswer>(json);
    private static string? Write(BaseAnswer? answer) => answer is null ? null : JsonSerializer.Serialize(answer);

    /// <summary>
    /// The answers that differ between two versions of a record's answers, one change per question (answer
    /// timestamps don't count).
    /// </summary>
    public static IEnumerable<AnswerChange> Between(Guid electionRoundId,
        ReportRecordType recordType,
        Guid recordId,
        IEnumerable<BaseAnswer> before,
        IEnumerable<BaseAnswer> after,
        DateTime changedAt)
    {
        var previous = before.GroupBy(x => x.QuestionId).ToDictionary(x => x.Key, x => x.Last());
        var current = after.GroupBy(x => x.QuestionId).ToDictionary(x => x.Key, x => x.Last());

        foreach (var questionId in previous.Keys.Union(current.Keys))
        {
            var was = previous.GetValueOrDefault(questionId);
            var now = current.GetValueOrDefault(questionId);
            if (was is not null && now is not null && FormBase.AnswersHelpers.HaveSameContent([was], [now]))
            {
                continue;
            }

            yield return new AnswerChange
            {
                Id = Guid.NewGuid(),
                ElectionRoundId = electionRoundId,
                RecordType = recordType,
                RecordId = recordId,
                QuestionId = questionId,
                PreviousAnswerJson = Write(was),
                CurrentAnswerJson = Write(now),
                ChangedAt = changedAt
            };
        }
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private AnswerChange()
    {
    }
#pragma warning restore CS8618
}
