using Vote.Monitor.Api.Feature.Forms.Update.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate.SelectQuestion;

namespace Vote.Monitor.Api.Feature.Forms.Update;

public static class Mapper
{
    public static List<BaseQuestion> ToEntities(this List<BaseQuestionRequest> questions)
    {
        List<BaseQuestion> mappedQuestions = new List<BaseQuestion>();
        foreach (var question in questions)
        {
            switch (question)
            {
                case SingleResponseQuestionRequest srq:
                    mappedQuestions.Add(new SingleResponseQuestion(srq.Id, srq.Headline, srq.Subheader, srq.Options.ToEntities()));
                    break;
                case MultiResponseQuestionRequest mrq:
                    mappedQuestions.Add(new MultiResponseQuestion(mrq.Id, mrq.Headline, mrq.Subheader, mrq.Options.ToEntities()));
                    break;
                case OpenQuestionRequest oq:
                    mappedQuestions.Add(new OpenQuestion(oq.Id, oq.Headline, oq.Subheader, oq.Placeholder, oq.LongAnswer, oq.OpenQuestionType));
                    break;
                case RatingQuestionRequest rq:
                    mappedQuestions.Add(new RatingQuestion(rq.Id, rq.Headline, rq.Subheader, rq.Scale));
                    break;
                default:
                    throw new ArgumentException($"Unknown question type = {question.GetType()}", nameof(question));
            }
        }

        return mappedQuestions;
    }

    private static List<QuestionOption> ToEntities(this List<QuestionOptionRequest> options) => options.Select(q => new QuestionOption(q.Id, q.Text, q.IsFlagged)).ToList();
}
