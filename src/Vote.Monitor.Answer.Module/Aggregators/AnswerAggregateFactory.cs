using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Answer.Module.Aggregators;

public static class AnswerAggregateFactory
{
    public static BaseAnswerAggregate Map(BaseQuestion question)
    {
        switch (question)
        {
            case TextQuestion textQuestion:
                return new TextAnswerAggregate(textQuestion);
            case NumberQuestion numberQuestion:
                return new NumberAnswerAggregate(numberQuestion);
            case DateQuestion dateQuestion:
                return new DateAnswerAggregate(dateQuestion);
            case RatingQuestion ratingQuestion:
                return new RatingAnswerAggregate(ratingQuestion);
            case SingleSelectQuestion singleSelectQuestion:
                return new SingleSelectAnswerAggregate(singleSelectQuestion);
            case MultiSelectQuestion multiSelectQuestion:
                return new MultiSelectAnswerAggregate(multiSelectQuestion);

            default:
                throw new ArgumentException("Unknown question received", question.Discriminator);
        }

    }
}
