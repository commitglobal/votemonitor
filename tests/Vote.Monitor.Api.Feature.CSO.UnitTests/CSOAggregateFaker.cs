namespace Vote.Monitor.Api.Feature.CSO.UnitTests;

public class CSOAggregateFaker : PrivateFaker<CSOAggregate>
{
    private readonly CSOStatus[] _statuses = [CSOStatus.Activated, CSOStatus.Deactivated];
    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);
    private readonly DateTime _baseModifiedDate = new(2024, 01, 02, 00, 00, 00, DateTimeKind.Utc);

    public CSOAggregateFaker(int? index = null, string? name = null, CSOStatus? status = null)
    {
        UsePrivateConstructor();

        RuleFor(fake => fake.Id, fake => fake.Random.Guid());
        RuleFor(fake => fake.Name, fake => name ?? fake.Name.FirstName());
        RuleFor(fake => fake.Status, fake => status ?? fake.PickRandom(_statuses));
        RuleFor(fake => fake.CreatedOn, _baseCreationDate.AddHours(index ?? 0));
        RuleFor(fake => fake.LastModifiedOn, _baseModifiedDate.AddHours(index ?? 0));
        RuleFor(fake => fake.CreatedBy, fake => fake.Random.Guid());
        RuleFor(fake => fake.LastModifiedBy, fake => fake.Random.Guid());
    }
}
