using Vote.Monitor.Domain.Entities.ElectionRoundFormTemplateAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public class ElectionRoundFormTemplateAggregateFaker : PrivateFaker<ElectionRoundFormTemplate>
{
    public ElectionRoundFormTemplateAggregateFaker(Guid? electionRoundId = null, Guid? formTemplateId = null)
    {
        UsePrivateConstructor();

        RuleFor(fake => fake.Id, fake => Guid.NewGuid());
        RuleFor(fake => fake.ElectionRoundId, electionRoundId);
        RuleFor(fake => fake.FormTemplateId, formTemplateId );
        
        RuleFor(fake => fake.ElectionRound, _ => null!); // Assuming lazy loading, null by default
        RuleFor(fake => fake.FormTemplate, _ => new FormTemplateAggregateFaker().Generate());
    }
}
