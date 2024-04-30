using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Answer.Module.Aggregators;

public static class AnswerAggregateFactory
{
    public static BaseAnswerAggregate Map(BaseQuestion question, int displayOrder)
    {
        switch (question)
        {
            case TextQuestion textQuestion:
                return new TextAnswerAggregate(textQuestion, displayOrder);
            case NumberQuestion numberQuestion:
                return new NumberAnswerAggregate(numberQuestion, displayOrder);
            case DateQuestion dateQuestion:
                return new DateAnswerAggregate(dateQuestion, displayOrder);
            case RatingQuestion ratingQuestion:
                return new RatingAnswerAggregate(ratingQuestion, displayOrder);
            case SingleSelectQuestion singleSelectQuestion:
                return new SingleSelectAnswerAggregate(singleSelectQuestion, displayOrder);
            case MultiSelectQuestion multiSelectQuestion:
                return new MultiSelectAnswerAggregate(multiSelectQuestion, displayOrder);

            default:
                throw new ArgumentException("Unknown question received", question.Discriminator);
        }

    }
}
