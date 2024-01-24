using Vote.Monitor.Domain.Entities.CSOAggregate;

namespace Vote.Monitor.TestUtils.Fakes;

public class NgoAggregateFaker : PrivateFaker<NgoAggregate>
{
    private readonly CSOStatus[] _statuses = [CSOStatus.Activated, CSOStatus.Deactivated];
    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);

    public NgoAggregateFaker(Guid? id = null, int? index = null, string? name = null, CSOStatus? status = null)
    {
        UsePrivateConstructor();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.Name, fake => name ?? fake.Name.FirstName());
        RuleFor(fake => fake.Status, fake => status ?? fake.PickRandom(_statuses));
        RuleFor(o => o.CreatedOn, _baseCreationDate.AddHours(index ?? 0));
    }
}
