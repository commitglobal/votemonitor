using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators.Fakes;

public sealed class TextAnswerFaker : Faker<TextAnswer>
{
    public TextAnswerFaker(Guid? questionId = null, string? text = null)
    {
        CustomInstantiator(f => TextAnswer.Create(questionId ?? f.Random.Guid(), text ?? f.Lorem.Paragraph()));
    }
}
