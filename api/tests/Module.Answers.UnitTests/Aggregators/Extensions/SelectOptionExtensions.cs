using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Answers.UnitTests.Aggregators.Extensions;

public static class SelectOptionExtensions
{
    public static SelectedOption Select(this SelectOption option) => SelectedOption.Create(option.Id, string.Empty);
}
