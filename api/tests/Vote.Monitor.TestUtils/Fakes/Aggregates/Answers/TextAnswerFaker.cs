using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

public sealed class TextAnswerFaker : Faker<TextAnswer>
{
    public TextAnswerFaker()
    {
        CustomInstantiator(f => TextAnswer.Create(f.Random.Guid(), f.Lorem.Text()));
    }

    public TextAnswerFaker(TextQuestion question)
    {
        CustomInstantiator(f => TextAnswer.Create(question.Id, f.Lorem.Text()));
    }

    public TextAnswerFaker(Guid questionId)
    {
        CustomInstantiator(f => TextAnswer.Create(questionId, f.Lorem.Text()));
    }
}