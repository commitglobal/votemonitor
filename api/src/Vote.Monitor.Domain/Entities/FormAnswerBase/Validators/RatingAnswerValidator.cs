using FluentValidation;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;

public class RatingAnswerValidator : AbstractValidator<RatingAnswer>
{
    public RatingAnswerValidator(RatingQuestion question)
    {
        RuleFor(x => x.QuestionId).NotEmpty();

        RuleFor(x => x.Value)
            .NotEmpty()
            .LessThanOrEqualTo(3)
            .When(_ => question.Scale == RatingScale.OneTo3)
            .LessThanOrEqualTo(4)
            .When(_ => question.Scale == RatingScale.OneTo4)
            .LessThanOrEqualTo(5)
            .When(_ => question.Scale == RatingScale.OneTo5)
            .LessThanOrEqualTo(6)
            .When(_ => question.Scale == RatingScale.OneTo6)
            .LessThanOrEqualTo(7)
            .When(_ => question.Scale == RatingScale.OneTo7)
            .LessThanOrEqualTo(8)
            .When(_ => question.Scale == RatingScale.OneTo8)
            .LessThanOrEqualTo(9)
            .When(_ => question.Scale == RatingScale.OneTo9)
            .LessThanOrEqualTo(10)
            .When(_ => question.Scale == RatingScale.OneTo10);
    }
}
