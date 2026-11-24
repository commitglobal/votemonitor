using System.Text.Json;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Vote.Monitor.Domain.UnitTests.Entities;

/// <summary>Which questions an observer was shown, and the answered / unanswered counts over them.</summary>
public class DisplayLogicEvaluatorTests
{
    private static readonly Guid Yes = Guid.NewGuid();
    private static readonly Guid No = Guid.NewGuid();
    private static readonly Guid Maybe = Guid.NewGuid();

    private static readonly SingleSelectQuestion A = SingleSelectQuestion.Create(Guid.NewGuid(), "A", new(),
    [
        SelectOption.Create(Yes, new()), SelectOption.Create(No, new()), SelectOption.Create(Maybe, new())
    ]);

    private static readonly MultiSelectQuestion M = MultiSelectQuestion.Create(Guid.NewGuid(), "M", new(),
    [
        SelectOption.Create(Yes, new()), SelectOption.Create(No, new()), SelectOption.Create(Maybe, new())
    ]);

    private static readonly NumberQuestion N = NumberQuestion.Create(Guid.NewGuid(), "N", new());
    private static readonly TextQuestion T = TextQuestion.Create(Guid.NewGuid(), "T", new());
    private static readonly TextQuestion C = TextQuestion.Create(Guid.NewGuid(), "C", new());

    private static TextQuestion ShownWhen(Guid parentId, DisplayLogicCondition condition, string? value = null,
        params string[] optionIds) =>
        TextQuestion.Create(Guid.NewGuid(), "B", new(),
            displayLogic: DisplayLogic.Create(parentId, condition, value, optionIds.Length == 0 ? null : optionIds));

    private static bool IsShown(BaseQuestion question, params BaseAnswer[] answers) =>
        DisplayLogicEvaluator.IsShown(question, answers.ToDictionary(x => x.QuestionId));

    private static SingleSelectAnswer Pick(Guid option) =>
        SingleSelectAnswer.Create(A.Id, SelectedOption.Create(option, ""));

    private static MultiSelectAnswer PickMany(params Guid[] options) =>
        MultiSelectAnswer.Create(M.Id, options.Select(x => SelectedOption.Create(x, "")).ToList());

    [Fact]
    public void TheContractExample_HiddenQuestionCountsForNeither()
    {
        var b = ShownWhen(A.Id, DisplayLogicCondition.AnyOf, null, Yes.ToString());
        BaseQuestion[] questions = [A, b, C];

        // A "No" and C: B is hidden, so everything shown is answered.
        DisplayLogicEvaluator.Count(questions, [Pick(No), TextAnswer.Create(C.Id, "fine")])
            .Should().Be(new AnswerCounts(2, 0));
        // A "Yes" and C: B is shown and unanswered.
        DisplayLogicEvaluator.Count(questions, [Pick(Yes), TextAnswer.Create(C.Id, "fine")])
            .Should().Be(new AnswerCounts(2, 1));
        // An answer to B left over from before A changed to "No" counts for neither.
        DisplayLogicEvaluator.Count(questions,
                [Pick(No), TextAnswer.Create(b.Id, "left over"), TextAnswer.Create(C.Id, "fine")])
            .Should().Be(new AnswerCounts(2, 0));
        // Nothing answered: B is hidden (no parent answer), A and C are unanswered.
        DisplayLogicEvaluator.Count(questions, []).Should().Be(new AnswerCounts(0, 2));
    }

    [Fact]
    public void SelectRules()
    {
        IsShown(ShownWhen(A.Id, DisplayLogicCondition.AnyOf, null, Yes.ToString(), No.ToString()), Pick(No))
            .Should().BeTrue();
        IsShown(ShownWhen(A.Id, DisplayLogicCondition.AnyOf, null, Yes.ToString()), Pick(Maybe)).Should().BeFalse();

        var all = ShownWhen(M.Id, DisplayLogicCondition.All, null, Yes.ToString(), No.ToString());
        IsShown(all, PickMany(Yes, No, Maybe)).Should().BeTrue();
        IsShown(all, PickMany(Yes)).Should().BeFalse();
        IsShown(ShownWhen(M.Id, DisplayLogicCondition.All, "x"), PickMany(Yes)).Should().BeFalse();

        // Older rules name the option in value (case doesn't matter).
        IsShown(ShownWhen(A.Id, DisplayLogicCondition.Includes, Yes.ToString().ToUpperInvariant()), Pick(Yes))
            .Should().BeTrue();
        IsShown(ShownWhen(A.Id, DisplayLogicCondition.Includes, Yes.ToString()), Pick(No)).Should().BeFalse();
        IsShown(ShownWhen(M.Id, DisplayLogicCondition.ValueEquals, Yes.ToString()), PickMany(No, Yes))
            .Should().BeTrue();
    }

    [Theory]
    [InlineData("Equals", "10", 10, true)]
    [InlineData("Equals", "10.0", 10, true)]
    [InlineData("NotEquals", "10", 10, false)]
    [InlineData("GreaterThan", "9", 10, true)]
    [InlineData("GreaterEqual", "10", 10, true)]
    [InlineData("LessThan", "9", 10, false)]
    [InlineData("LessEqual", "10", 10, true)]
    [InlineData("Equals", "0", 0, true)]
    [InlineData("Equals", "ten", 10, false)]
    [InlineData("NotEquals", "ten", 10, false)]
    [InlineData("AnyOf", "10", 10, false)]
    public void NumberRules(string condition, string value, int answer, bool shown)
    {
        var question = ShownWhen(N.Id, DisplayLogicCondition.FromValue(condition), value);

        IsShown(question, NumberAnswer.Create(N.Id, answer)).Should().Be(shown);
        IsShown(question, RatingAnswer.Create(N.Id, answer)).Should().Be(shown);
    }

    [Theory]
    [InlineData("Equals", "abc", "abc", true)]
    [InlineData("Equals", "abc", "ABC", false)]
    [InlineData("NotEquals", "abc", "abd", true)]
    [InlineData("GreaterThan", "abc", "abd", true)]
    [InlineData("LessThan", "abc", "abd", false)]
    public void TextRules(string condition, string value, string answer, bool shown) =>
        IsShown(ShownWhen(T.Id, DisplayLogicCondition.FromValue(condition), value), TextAnswer.Create(T.Id, answer))
            .Should().Be(shown);

    [Fact]
    public void NoParentAnswer_OtherParentTypes_AndIncompleteRules_NeverShow()
    {
        var d = DateQuestion.Create(Guid.NewGuid(), "D", new());

        IsShown(ShownWhen(A.Id, DisplayLogicCondition.AnyOf, null, Yes.ToString())).Should().BeFalse();
        IsShown(ShownWhen(d.Id, DisplayLogicCondition.ValueEquals, "2024-01-01"),
            DateAnswer.Create(d.Id, new DateTime(2024, 1, 1))).Should().BeFalse();
        IsShown(ShownWhen(A.Id, DisplayLogicCondition.AnyOf), Pick(Yes)).Should().BeFalse();
        IsShown(ShownWhen(N.Id, DisplayLogicCondition.ValueEquals, ""), NumberAnswer.Create(N.Id, 0)).Should().BeFalse();
    }

    [Fact]
    public void AStoredRuleWithoutCondition_NeverShows()
    {
        var json = $$"""
                     {
                       "$questionType": "textQuestion",
                       "Id": "{{Guid.NewGuid()}}",
                       "Code": "B",
                       "Text": { "EN": "B" },
                       "DisplayLogic": { "ParentQuestionId": "{{A.Id}}", "Condition": null, "Value": "{{Yes}}", "OptionIds": null }
                     }
                     """;
        var question = JsonSerializer.Deserialize<BaseQuestion>(json)!;

        IsShown(question, Pick(Yes)).Should().BeFalse();
    }

    [Fact]
    public void EmptyAnswersCountAsUnanswered_ZeroIsAnAnswer()
    {
        BaseQuestion[] questions = [A, M, N, T];

        DisplayLogicEvaluator.Count(questions,
            [
                SingleSelectAnswer.Create(A.Id, SelectedOption.Create(Guid.Empty, "")),
                MultiSelectAnswer.Create(M.Id, []),
                NumberAnswer.Create(N.Id, 0),
                TextAnswer.Create(T.Id, "   ")
            ])
            .Should().Be(new AnswerCounts(1, 3));
    }
}
