namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class ObserverAggregateFaker : PrivateFaker<ObserverAggregate>
{
    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);

    public ObserverAggregateFaker(Guid? id = null,
        int? index = null,
        ApplicationUser? applicationUser = null)
    {
        UsePrivateConstructor();
        applicationUser ??= new ApplicationUserFaker();

        RuleFor(fake => fake.Id, faker => id ?? faker.Random.Guid());
        RuleFor(fake => fake.ApplicationUser, applicationUser);
        RuleFor(o => o.CreatedOn, _baseCreationDate.AddHours(index ?? 0));
    }
}
