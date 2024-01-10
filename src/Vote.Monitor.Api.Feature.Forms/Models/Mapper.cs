using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate.SelectQuestion;

namespace Vote.Monitor.Api.Feature.Forms.Models;

public static class Mapper
{
    public static List<BaseQuestionModel> ToModels(this List<BaseQuestion> questions)
    {
        var mappedQuestions = new List<BaseQuestionModel>();
        foreach (var question in questions)
        {
            switch (question)
            {
                case SingleResponseQuestion srq:
                    mappedQuestions.Add(new SingleResponseQuestionModel(srq.Id, srq.Headline, srq.Subheader, srq.Options.ToModels()));
                    break;
                case MultiResponseQuestion mrq:
                    mappedQuestions.Add(new MultipleChoiceQuestionModel(mrq.Id, mrq.Headline, mrq.Subheader, mrq.Options.ToModels()));
                    break;
                case OpenQuestion oq:
                    mappedQuestions.Add(new OpenQuestionModel(oq.Id, oq.Headline, oq.Subheader, oq.Placeholder, oq.LongAnswer, oq.OpenQuestionType));
                    break;
                case RatingQuestion rq:
                    mappedQuestions.Add(new RatingQuestionModel(rq.Id, rq.Headline, rq.Subheader, rq.Scale));
                    break;
                default:
                    throw new ArgumentException($"Unknown question type = {question.GetType()}", nameof(question));
            }
        }

        return mappedQuestions;
    }

    private static List<QuestionOptionModel> ToModels(this List<QuestionOption> options) => options.Select(q => new QuestionOptionModel(q.Id, q.Text, q.IsFlagged)).ToList();
}
