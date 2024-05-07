using Bogus;
using SubmissionsFaker.Clients.Models.Questions;
using SubmissionsFaker.Clients.MonitoringObserver.Models;

namespace SubmissionsFaker.Fakers;

public sealed class MultiSelectAnswerFaker : Faker<MultiSelectAnswerRequest>
{
    public MultiSelectAnswerFaker(Guid questionId, List<SelectOptionRequest> options)
    {
        RuleFor(x => x.QuestionId, questionId);

        RuleFor(x => x.Selection, f =>
        {
            var optionsToPick = f.Random.Number(1, options.Count);

            var selectedOptions = f.PickRandom(options, optionsToPick).Distinct().ToList();
            var selection = new List<SelectedOptionRequest>();

            foreach (var selectedOption in selectedOptions)
            {
                string? text = null;
                if (selectedOption.IsFreeText)
                {
                    text = FakerHub.Lorem.Sentence(100);
                }

                selection.Add(new SelectedOptionRequest() { OptionId = selectedOption.Id, Text = text });
            }

            return selection;
        });
    }
}
