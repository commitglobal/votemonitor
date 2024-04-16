using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

public sealed class SelectedOptionFaker : Faker<SelectedOption>
{
    public SelectedOptionFaker(IReadOnlyList<SelectOption>? options = null)
    {
        Guid optionId = FakerHub.Random.Guid();
        if (options != null && options.Any())
        {
            var optionIds = options?.Select(x => x.Id)?.ToList();
            optionId = FakerHub.PickRandom(optionIds);
        }

        CustomInstantiator(f => SelectedOption.Create(optionId, f.Lorem.Text()));
    }
}
