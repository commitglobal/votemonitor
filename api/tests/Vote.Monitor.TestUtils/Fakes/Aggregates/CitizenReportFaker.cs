using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class CitizenReportFaker : PrivateFaker<CitizenReport>
{
    public CitizenReportFaker(Guid? id = null,
        ElectionRound? electionRound = null,
        Form? form = null)
    {
        UsePrivateConstructor();
        electionRound ??= new ElectionRoundAggregateFaker().Generate();
        form ??= new FormAggregateFaker(electionRound).Generate();

        RuleFor(fake => fake.Id, faker => id ?? faker.Random.Guid());
        RuleFor(fake => fake.ElectionRound, electionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound.Id);
        
        RuleFor(fake => fake.Form, form);
        RuleFor(fake => fake.FormId, form.Id);
    }
}
