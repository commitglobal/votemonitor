using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class NgoAggregateFaker : PrivateFaker<Ngo>
{
    private readonly NgoStatus[] _statuses = [NgoStatus.Activated, NgoStatus.Deactivated];
    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);
    private readonly DateTime _baseModifiedDate = new(2024, 01, 02, 00, 00, 00, DateTimeKind.Utc);

    public NgoAggregateFaker(Guid? ngoId = null, int? index = null, string? name = null, NgoStatus? status = null)
    {
        UsePrivateConstructor();

        RuleFor(fake => fake.Id, fake => ngoId ?? fake.Random.Guid());
        RuleFor(fake => fake.Name, fake => name ?? fake.Company.CompanyName());
        RuleFor(fake => fake.Status, fake => status ?? fake.PickRandom(_statuses));
        RuleFor(fake => fake.CreatedOn, _baseCreationDate.AddHours(index ?? 0));
        RuleFor(fake => fake.LastModifiedOn, _baseModifiedDate.AddHours(index ?? 0));
        RuleFor(fake => fake.CreatedBy, fake => fake.Random.Guid());
        RuleFor(fake => fake.LastModifiedBy, fake => fake.Random.Guid());
    }
}
