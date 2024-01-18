using Bogus;
using NSubstitute;

namespace Vote.Monitor.Domain.UnitTests.Specifications;

public class AuditableBaseEntityFaker : Faker<TestEntity>
{
    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);
    private readonly DateTime _baseUpdateDate = new(2024, 01, 02, 00, 00, 00, DateTimeKind.Utc);
    private readonly ITimeService _timeService = Substitute.For<ITimeService>();

    public AuditableBaseEntityFaker(int? index = null)
    {
        _timeService.UtcNow.Returns(_baseCreationDate.AddHours(index ?? 0));

        RuleFor(fake => fake.Id, Guid.NewGuid());
        RuleFor(o => o.LastModifiedOn, _baseUpdateDate.AddHours(index ?? 0));

        CustomInstantiator(faker => new TestEntity(_timeService));
    }
}
