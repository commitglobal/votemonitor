using Bogus;

namespace Vote.Monitor.Domain.UnitTests.Specifications;

public sealed class AuditableBaseEntityFaker : Faker<TestEntity>
{
    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);
    private readonly DateTime _baseUpdateDate = new(2024, 01, 02, 00, 00, 00, DateTimeKind.Utc);

    public AuditableBaseEntityFaker(int? index = null)
    {
        RuleFor(fake => fake.Id, Guid.NewGuid());
        RuleFor(o => o.LastModifiedOn, _baseUpdateDate.AddHours(index ?? 0));

        CustomInstantiator(faker => new TestEntity());
    }
}
