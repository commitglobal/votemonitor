using AutoBogus.NSubstitute;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Api.Feature.Observer.UnitTests.Specifications;

public class ObserverAggregateFaker : AutoFaker<ObserverAggregate>
{
    private readonly UserStatus[] _statuses = [UserStatus.Active, UserStatus.Deactivated];
    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);
    private readonly ITimeService _timeService = Substitute.For<ITimeService>();

    public ObserverAggregateFaker(int? index = null, string? name = null, string? login = null, string? password = null, UserStatus? status = null)
    {
        _timeService.UtcNow.Returns(_baseCreationDate.AddHours(index ?? 0));

        Configure(builder => builder
            .WithBinder(new NSubstituteBinder())
            .WithOverride<ITimeService>(ctx => _timeService));

        RuleFor(fake => fake.Id, Guid.NewGuid);
        RuleFor(fake => fake.Name, fake => name ?? fake.Name.FirstName());
        RuleFor(fake => fake.PhoneNumber, fake => name ?? fake.Phone.PhoneNumber());
        RuleFor(fake => fake.Login, fake => login ?? fake.Internet.Email());
        RuleFor(fake => fake.Password, fake => password ?? fake.Random.String2(10));
        RuleFor(fake => fake.Status, fake => status ?? fake.PickRandom(_statuses));
        RuleFor(fake => fake.Role, UserRole.Observer);

        Ignore(o => o.CreatedBy);
        Ignore(o => o.CreatedOn);
        Ignore(o => o.LastModifiedOn);
        Ignore(o => o.LastModifiedBy);
    }
}
