namespace Vote.Monitor.TestUtils.Fakes.Aggregates;
public sealed class ApplicationUserFaker : PrivateFaker<ApplicationUser>
{
    private readonly UserStatus[] _statuses = [UserStatus.Active, UserStatus.Deactivated];
    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);

    public ApplicationUserFaker(Guid? id = null,
        string? name = null,
        UserStatus? status = null,
        UserPreferences? preferences = null)
    {
        UsePrivateConstructor();
        RuleFor(fake => fake.Id, faker => id ?? faker.Random.Guid());
        RuleFor(fake => fake.FirstName, fake => name ?? fake.Name.FirstName());
        RuleFor(fake => fake.LastName, fake => name ?? fake.Name.LastName());
        RuleFor(fake => fake.Preferences, preferences);
        RuleFor(fake => fake.Status, fake => status ?? fake.PickRandom(_statuses));
        RuleFor(fake => fake.Preferences, fake => preferences ?? UserPreferences.Defaults);
    }
}
