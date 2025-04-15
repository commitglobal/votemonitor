using FluentValidation.Results;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Answers.UnitTests.Aggregators;

public record TestAnswer() : BaseAnswer(Guid.NewGuid())
{
    public override string Discriminator => "TestAnswer";

    public override ValidationResult Validate(BaseQuestion question, int index)
    {
        throw new NotImplementedException();
    }
}
