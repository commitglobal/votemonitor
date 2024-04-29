using Bogus;
using SubmissionsFaker.Clients.Models.Questions;
using SubmissionsFaker.Clients.MonitoringObserver.Models;

namespace SubmissionsFaker.Fakers;

public sealed class SingleSelectAnswerFaker : Faker<SingleSelectAnswerRequest>
{
    public SingleSelectAnswerFaker(Guid questionId, List<SelectOptionRequest> options)
    {
        RuleFor(x => x.QuestionId, questionId);
        var selectedOption = FakerHub.PickRandom(options);

        string? text = null;
        if (selectedOption.IsFreeText)
        {
            text = FakerHub.Lorem.Sentence(100);
        }

        var selection = new SelectedOptionRequest() { OptionId = selectedOption.Id, Text = text };

        RuleFor(x => x.Selection, f => selection);
    }
}
