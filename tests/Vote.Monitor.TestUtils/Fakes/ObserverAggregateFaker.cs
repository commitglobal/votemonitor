using System;

namespace Vote.Monitor.TestUtils.Fakes;

public class ObserverAggregateFaker : PrivateFaker<ObserverAggregate>
{
    private readonly UserStatus[] _statuses = [UserStatus.Active, UserStatus.Deactivated];
    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);

    public ObserverAggregateFaker(Guid? id = null,
        int? index = null,
        string? name = null,
        string? login = null,
        string? password = null,
        UserStatus? status = null)
    {
        UsePrivateConstructor();

        RuleFor(fake => fake.Id, faker => id ?? faker.Random.Guid());
        RuleFor(fake => fake.Name, fake => name ?? fake.Name.FirstName());
        RuleFor(fake => fake.PhoneNumber, fake => name ?? fake.Phone.PhoneNumber());
        RuleFor(fake => fake.Login, fake => login ?? fake.Internet.Email());
        RuleFor(fake => fake.Password, fake => password ?? fake.Random.String2(10));
        RuleFor(fake => fake.Status, fake => status ?? fake.PickRandom(_statuses));
        RuleFor(fake => fake.Role, UserRole.Observer);
        RuleFor(o => o.CreatedOn, _baseCreationDate.AddHours(index ?? 0));
    }
}
