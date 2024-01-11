namespace Vote.Monitor.Api.Feature.Observer.UnitTests.Specifications;

public class ObserverAggregateFaker : AutoFaker<ObserverAggregate>
{
    private readonly UserStatus[] _statuses = { UserStatus.Active, UserStatus.Deactivated };
    public ObserverAggregateFaker(int? index = null, string? name = null, string? login = null, string? password = null, UserStatus? status = null)
    {
        RuleFor(fake => fake.Id, fake => index.HasValue ? GenerateConsecutiveGuid(index.Value) : Guid.NewGuid());
        RuleFor(fake => fake.Name, fake => name ?? fake.Name.FirstName());
        RuleFor(fake => fake.Login, fake => login ?? fake.Internet.Email());
        RuleFor(fake => fake.Password, fake => password ?? fake.Random.String2(10));
        RuleFor(fake => fake.Status, fake => status ?? fake.PickRandom(_statuses));
    }

    private static Guid GenerateConsecutiveGuid(int index)
    {
        return Guid.Parse($"00000000-0000-0000-0000-{index.ToString("X").PadLeft(12, '0')}");
    }
}
