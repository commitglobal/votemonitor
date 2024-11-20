using Vote.Monitor.Api.IntegrationTests.Models;
using Vote.Monitor.Form.Module.Requests;

namespace Vote.Monitor.Api.IntegrationTests.Fakers;

public sealed class SubmissionFaker : Faker<FormSubmissionRequest>
{
    public SubmissionFaker(Guid formId, Guid pollingStationId, List<BaseQuestionRequest> questions)
    {
        Rules((f, x) =>
        {
            x.PollingStationId = pollingStationId;
            x.FormId = formId;
            x.Answers = f.PickRandom(questions, f.Random.Int(0, questions.Count))
                .Select(Answers.GetFakeAnswer).ToList();
        });
    }
}
