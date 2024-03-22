using Vote.Monitor.Core.Security;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;
public sealed class ApplicationUserFaker : PrivateFaker<ApplicationUserAggregate>
{
    private readonly UserStatus[] _statuses = [UserStatus.Active, UserStatus.Deactivated];
    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);

    public ApplicationUserFaker(Guid? id = null,
        int? index = null,
        string? name = null,
        string? login = null,
        string? password = null,
        UserStatus? status = null,
        UserPreferences? preferences = null)
    {
        UsePrivateConstructor();
        RuleFor(fake => fake.Id, faker => id ?? faker.Random.Guid());
        RuleFor(fake => fake.Name, fake => name ?? fake.Name.FirstName());
        RuleFor(fake => fake.Preferences, preferences);
        RuleFor(fake => fake.Login, fake => login ?? fake.Internet.Email());
        RuleFor(fake => fake.Password, fake => password ?? fake.Random.String2(10));
        RuleFor(fake => fake.Status, fake => status ?? fake.PickRandom(_statuses));
        RuleFor(fake => fake.Role, UserRole.Observer);
        RuleFor(fake => fake.Preferences, fake => preferences ?? UserPreferences.Defaults);
        RuleFor(o => o.CreatedOn, _baseCreationDate.AddHours(index ?? 0));
    }
}
