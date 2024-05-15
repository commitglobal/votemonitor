using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

public sealed class SingleSelectAnswerFaker : Faker<SingleSelectAnswer>
{
    public SingleSelectAnswerFaker(SingleSelectQuestion? question = null)
    {
        CustomInstantiator(f => SingleSelectAnswer.Create(question?.Id ?? f.Random.Guid(), new SelectedOptionFaker(question?.Options).Generate()));
    }
}
