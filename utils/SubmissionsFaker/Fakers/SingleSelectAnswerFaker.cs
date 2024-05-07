using Bogus;
using SubmissionsFaker.Clients.Models.Questions;
using SubmissionsFaker.Clients.MonitoringObserver.Models;

namespace SubmissionsFaker.Fakers;

public sealed class SingleSelectAnswerFaker : Faker<SingleSelectAnswerRequest>
{
    public SingleSelectAnswerFaker(Guid questionId, List<SelectOptionRequest> options)
    {
        RuleFor(x => x.QuestionId, questionId);
        RuleFor(x => x.Selection, f =>
        {
            var selectedOption = f.PickRandom(options);

            string? text = null;
            if (selectedOption.IsFreeText)
            {
                text = f.Lorem.Sentence(100);
            }

            var selection = new SelectedOptionRequest() { OptionId = selectedOption.Id, Text = text };
            return selection;
        });
    }
}
