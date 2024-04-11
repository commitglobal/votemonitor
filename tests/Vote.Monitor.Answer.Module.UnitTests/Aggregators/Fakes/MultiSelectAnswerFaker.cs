using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators.Fakes;

public sealed class MultiSelectAnswerFaker : Faker<MultiSelectAnswer>
{
    public MultiSelectAnswerFaker(Guid? questionId = null, IEnumerable<SelectedOption>? options = null)
    {
        CustomInstantiator(f => MultiSelectAnswer.Create(questionId ?? f.Random.Guid(), options?.ToList() ?? []));
    }
}
