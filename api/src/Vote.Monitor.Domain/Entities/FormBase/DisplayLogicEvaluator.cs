using System.Globalization;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormBase;

/// <summary>
/// Which of a form's questions the observer was shown, the way the mobile app decides it: a question with display
/// logic is shown only when its parent's answer meets the rule, judged against that answer alone (one level).
/// </summary>
public static class DisplayLogicEvaluator
{
    /// <summary>Whether a question is shown, given the record's answers by question id.</summary>
    public static bool IsShown(BaseQuestion question, IReadOnlyDictionary<Guid, BaseAnswer> answersByQuestionId)
    {
        var logic = question.DisplayLogic;
        if (logic is null)
        {
            return true;
        }

        if (!answersByQuestionId.TryGetValue(logic.ParentQuestionId, out var parent) || parent is null)
        {
            return false;
        }

        // Stored rules can carry a null condition; such a rule, or one naming neither a value nor options, never shows
        // its question.
        var condition = logic.Condition?.Value;
        var optionIds = logic.OptionIds ?? [];
        if (condition is null || (string.IsNullOrEmpty(logic.Value) && optionIds.Count == 0))
        {
            return false;
        }

        return parent switch
        {
            SingleSelectAnswer single => MatchesSelect(
                single.Selection is null || single.Selection.OptionId == Guid.Empty ? [] : [single.Selection.OptionId],
                condition, logic.Value, optionIds),
            MultiSelectAnswer multi => MatchesSelect(
                (multi.Selection ?? []).Select(x => x.OptionId).ToList(), condition, logic.Value, optionIds),
            NumberAnswer number => CompareNumber(number.Value, logic.Value, condition),
            RatingAnswer rating => CompareNumber(rating.Value, logic.Value, condition),
            TextAnswer text => CompareText(text.Text, logic.Value ?? string.Empty, condition),
            _ => false
        };
    }

    /// <summary>Whether an answer says nothing: blank text, no option picked, no number, rating, date or time.</summary>
    public static bool IsEmpty(BaseAnswer? answer) =>
        answer switch
        {
            null => true,
            TextAnswer text => string.IsNullOrWhiteSpace(text.Text),
            SingleSelectAnswer single => single.Selection is null || single.Selection.OptionId == Guid.Empty,
            MultiSelectAnswer multi => multi.Selection is null || multi.Selection.Count == 0,
            // Numbers, ratings, dates and times are value types: present whenever the answer is (0 is an answer).
            _ => false
        };

    /// <summary>
    /// The questions the observer was shown that they answered, and those they left unanswered; a hidden question
    /// counts for neither, even with an answer left over from before its parent's answer changed.
    /// </summary>
    public static AnswerCounts Count(IEnumerable<BaseQuestion> questions, IEnumerable<BaseAnswer>? answers)
    {
        var answersByQuestionId = new Dictionary<Guid, BaseAnswer>();
        foreach (var answer in answers ?? [])
        {
            answersByQuestionId.TryAdd(answer.QuestionId, answer);
        }

        var answered = 0;
        var unanswered = 0;
        foreach (var question in questions)
        {
            if (!IsShown(question, answersByQuestionId))
            {
                continue;
            }

            answersByQuestionId.TryGetValue(question.Id, out var answer);
            if (IsEmpty(answer))
            {
                unanswered++;
            }
            else
            {
                answered++;
            }
        }

        return new AnswerCounts(answered, unanswered);
    }

    private static bool MatchesSelect(IReadOnlyCollection<Guid> picked, string condition, string? value,
        IReadOnlyList<string> optionIds)
    {
        var targets = optionIds.Select(ParseGuid).ToList();

        if (condition == DisplayLogicCondition.AnyOf.Value)
        {
            return targets.Any(x => x is not null && picked.Contains(x.Value));
        }

        if (condition == DisplayLogicCondition.All.Value)
        {
            return targets.Count > 0 && targets.All(x => x is not null && picked.Contains(x.Value));
        }

        // Older rules (Includes, Equals…) name a single option in value.
        return ParseGuid(value) is { } option && picked.Contains(option);
    }

    private static bool CompareNumber(int answer, string? value, string condition)
    {
        // Compared as numbers ("10" > "9"); a value that isn't a number never meets the rule.
        if (string.IsNullOrWhiteSpace(value) ||
            !decimal.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var target))
        {
            return false;
        }

        return Compare(((decimal)answer).CompareTo(target), condition);
    }

    private static bool CompareText(string? answer, string value, string condition) =>
        Compare(string.CompareOrdinal(answer ?? string.Empty, value), condition);

    private static bool Compare(int comparison, string condition) =>
        condition switch
        {
            "Equals" => comparison == 0,
            "NotEquals" => comparison != 0,
            "GreaterThan" => comparison > 0,
            "GreaterEqual" => comparison >= 0,
            "LessThan" => comparison < 0,
            "LessEqual" => comparison <= 0,
            _ => false
        };

    private static Guid? ParseGuid(string? value) => Guid.TryParse(value, out var id) ? id : null;
}

/// <summary>The questions the observer was shown that they answered, and those they left unanswered.</summary>
public readonly record struct AnswerCounts(int Answered, int Unanswered);
