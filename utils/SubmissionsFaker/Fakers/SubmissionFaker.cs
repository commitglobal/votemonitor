using Bogus;
using SubmissionsFaker.Clients.MonitoringObserver.Models;
using SubmissionsFaker.Clients.NgoAdmin.Models;

namespace SubmissionsFaker.Fakers;

public sealed class SubmissionFaker : Faker<SubmissionRequest>
{
    public SubmissionFaker(Guid formId, Guid pollingStationId, List<BaseQuestionRequest> questions)
    {
        RuleFor(x => x.PollingStationId, pollingStationId);
        RuleFor(x => x.FormId, formId);
        RuleFor(x => x.SubmissionId, f => f.Random.Guid());
        RuleFor(x => x.Answers, questions.Select(GetFakeAnswer).ToList());
    }

    private BaseAnswerRequest GetFakeAnswer(BaseQuestionRequest question)
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