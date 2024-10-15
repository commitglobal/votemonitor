using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

public sealed class SingleSelectAnswerFaker : Faker<SingleSelectAnswer>
{
    public SingleSelectAnswerFaker()
    {
        CustomInstantiator(f => SingleSelectAnswer.Create(f.Random.Guid(),
            new SelectedOptionFaker().Generate()));
    }

    public SingleSelectAnswerFaker(SingleSelectQuestion question)
    {
        CustomInstantiator(f => SingleSelectAnswer.Create(question.Id,
            new SelectedOptionFaker(question.Options).Generate()));
    }

    public SingleSelectAnswerFaker(Guid questionId, SelectedOption selectedOption)
    {
        CustomInstantiator(f => SingleSelectAnswer.Create(questionId, selectedOption));
    }
}