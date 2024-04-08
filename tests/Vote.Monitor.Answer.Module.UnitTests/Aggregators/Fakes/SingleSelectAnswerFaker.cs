using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators.Fakes;

public sealed class SingleSelectAnswerFaker : Faker<SingleSelectAnswer>
{
    public SingleSelectAnswerFaker(Guid? questionId = null, SelectedOption? selectedOption = null)
    {
        CustomInstantiator(f => SingleSelectAnswer.Create(questionId ?? f.Random.Guid(), selectedOption ?? new SelectedOption(Guid.NewGuid(), "")));
    }
}
