using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.LocationAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class CitizenReportFaker : PrivateFaker<CitizenReport>
{
    public CitizenReportFaker(Guid? id = null,
        ElectionRound? electionRound = null,
        Location? location = null,
        Form? form = null)
    {
        UsePrivateConstructor();
        electionRound ??= new ElectionRoundAggregateFaker().Generate();
        form ??= new FormAggregateFaker(electionRound).Generate();
        location ??= new LocationAggregateFaker(electionRound).Generate();

        RuleFor(fake => fake.Id, faker => id ?? faker.Random.Guid());
        RuleFor(fake => fake.ElectionRound, electionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound.Id);

        RuleFor(fake => fake.Location, location);
        RuleFor(fake => fake.LocationId, location.Id);

        RuleFor(fake => fake.Form, form);
        RuleFor(fake => fake.FormId, form.Id);
    }
}