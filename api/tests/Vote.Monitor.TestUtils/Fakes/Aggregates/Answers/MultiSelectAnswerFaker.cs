using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

public sealed class MultiSelectAnswerFaker : Faker<MultiSelectAnswer>
{
    public MultiSelectAnswerFaker()
    {
        CustomInstantiator(f => MultiSelectAnswer.Create(f.Random.Guid(),
            new SelectedOptionFaker().GenerateBetween(1, 4)));
    }

    public MultiSelectAnswerFaker(MultiSelectQuestion question)
    {
        CustomInstantiator(f => MultiSelectAnswer.Create(question?.Id ?? f.Random.Guid(),
            new SelectedOptionFaker(question?.Options).GenerateBetween(1, question?.Options?.Count ?? 4)));
    }

    public MultiSelectAnswerFaker(Guid questionId, IEnumerable<SelectedOption> options)
    {
        CustomInstantiator(f => MultiSelectAnswer.Create(questionId, options.ToList()));
    }
}