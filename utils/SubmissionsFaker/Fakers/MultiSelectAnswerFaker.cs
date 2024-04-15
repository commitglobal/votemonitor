using Bogus;
using SubmissionsFaker.Clients.MonitoringObserver.Models;
using SubmissionsFaker.Clients.NgoAdmin.Models;

namespace SubmissionsFaker.Fakers;

public sealed class MultiSelectAnswerFaker : Faker<MultiSelectAnswerRequest>
{
    public MultiSelectAnswerFaker(Guid questionId, List<SelectOptionRequest> options)
    {
        RuleFor(x => x.QuestionId, questionId);
        var optionsToPick = FakerHub.Random.Number(0, options.Count);

        var selectedOptions = FakerHub.PickRandom(options, optionsToPick).Distinct().ToList();
        List<SelectedOptionRequest> selection = new List<SelectedOptionRequest>();

        foreach (var selectedOption in selectedOptions)
        {
            string? text = null;
            if (selectedOption.IsFreeText)
            {
                text = FakerHub.Lorem.Sentence(100);
            }

            selection.Add(new SelectedOptionRequest() { OptionId = selectedOption.Id, Text = text });
        }

        RuleFor(x => x.Selection, selection);
    }
}
