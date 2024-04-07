using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators.Fakes;

public sealed class MultiSelectAnswerFaker : Faker<MultiSelectAnswer>
{
    public MultiSelectAnswerFaker(Guid? questionId = null, IEnumerable<SelectedOption>? options = null)
    {
        CustomInstantiator(f => MultiSelectAnswer.Create(questionId ?? f.Random.Guid(), options?.ToList() ?? []));
    }
}
