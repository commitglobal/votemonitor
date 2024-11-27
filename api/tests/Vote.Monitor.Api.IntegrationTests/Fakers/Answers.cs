using Vote.Monitor.Answer.Module.Requests;
using Vote.Monitor.Form.Module.Requests;

namespace Vote.Monitor.Api.IntegrationTests.Fakers;

public static class Answers
{
    public static BaseAnswerRequest GetFakeAnswer(BaseQuestionRequest question)
    {
        switch (question)
        {
            case TextQuestionRequest _:
                return new TextAnswerFaker(question.Id).Generate();

            case NumberQuestionRequest _:
                return new NumberAnswerFaker(question.Id).Generate();

            case DateQuestionRequest _:
                return new DateAnswerFaker(question.Id).Generate();

            case SingleSelectQuestionRequest singleSelectQuestionRequest:
                return new SingleSelectAnswerFaker(question.Id, singleSelectQuestionRequest.Options).Generate();

            case MultiSelectQuestionRequest multiSelectQuestionRequest:
                return new MultiSelectAnswerFaker(question.Id, multiSelectQuestionRequest.Options).Generate();

            case RatingQuestionRequest _:
                return new RatingAnswerFaker(question.Id).Generate();

            default: throw new ApplicationException("Unknown question type received");
        }
    }
}
