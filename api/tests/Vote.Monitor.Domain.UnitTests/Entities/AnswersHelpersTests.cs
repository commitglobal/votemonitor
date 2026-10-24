using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

namespace Vote.Monitor.Domain.UnitTests.Entities;

public class AnswersHelpersTests
{
    private static BaseAnswer[] Answers() =>
    [
        new TextAnswerFaker().Generate(),
        new NumberAnswerFaker().Generate(),
        new SingleSelectAnswerFaker().Generate(),
        new MultiSelectAnswerFaker().Generate()
    ];

    [Fact]
    public void HaveSameContent_IgnoresAnswerTimestampsAndOrder()
    {
        var answers = Answers();
        var resent = answers.DeepClone()
            .Reverse()
            .Select(x => x.WithTimestamps(DateTime.UtcNow, DateTime.UtcNow))
            .ToArray();

        AnswersHelpers.HaveSameContent(answers, resent).Should().BeTrue();
    }

    [Fact]
    public void HaveSameContent_DetectsChangedAddedAndRemovedAnswers()
    {
        var answers = Answers();
        var changed = answers.DeepClone();
        changed[1] = NumberAnswer.Create(changed[1].QuestionId, ((NumberAnswer)changed[1]).Value + 1);

        AnswersHelpers.HaveSameContent(answers, changed).Should().BeFalse();
        AnswersHelpers.HaveSameContent(answers, answers.Append(new TextAnswerFaker().Generate())).Should().BeFalse();
        AnswersHelpers.HaveSameContent(answers, answers.Skip(1)).Should().BeFalse();
    }
}
