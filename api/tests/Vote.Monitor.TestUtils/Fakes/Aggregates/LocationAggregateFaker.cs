using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.LocationAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class LocationAggregateFaker : PrivateFaker<Location>
{
    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);

    public LocationAggregateFaker(ElectionRound? electionRound = null, Guid? id = null, int? displayOrder = null)
    {
        UsePrivateConstructor();
        electionRound ??= new ElectionRoundAggregateFaker().Generate();

        RuleFor(fake => fake.Id, id ?? Guid.NewGuid());
        RuleFor(fake => fake.ElectionRound, electionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound.Id);
        RuleFor(fake => fake.Level1, fake => fake.Address.Country());
        RuleFor(fake => fake.Level2, fake => fake.Address.State());
        RuleFor(fake => fake.Level3, fake => fake.Address.County());
        RuleFor(fake => fake.Level4, fake => fake.Address.City());
        RuleFor(fake => fake.Level5, fake => fake.Address.StreetName());
        RuleFor(fake => fake.DisplayOrder, fake => displayOrder ?? fake.Random.Int(0, 1000));
        RuleFor(fake => fake.Tags, fake => fake.Commerce.Categories(4).Select((x, i) => new TagImportModel
        {
            Name = $"Tag {i}",
            Value = x
        }).ToTagsObject());
        RuleFor(fake => fake.CreatedOn, _baseCreationDate);
    }

}
