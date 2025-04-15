using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.TestUtils.Fakes;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.PollingStations.UnitTests.Specifications;

public sealed class PollingStationAggregateFaker : PrivateFaker<PollingStationAggregate>
{
    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);

    public PollingStationAggregateFaker(ElectionRound? electionRound = null, Guid? id = null, int? displayOrder = null, string? address = null)
    {
        UsePrivateConstructor();
        electionRound ??= new ElectionRoundAggregateFaker().Generate();

        RuleFor(fake => fake.Id, id ?? Guid.NewGuid());
        RuleFor(fake => fake.ElectionRound, electionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound.Id);
        RuleFor(fake => fake.DisplayOrder, fake => displayOrder ?? fake.Random.Int(0, 1000));
        RuleFor(fake => fake.Address, fake => address ?? fake.Address.FullAddress());
        RuleFor(fake => fake.Tags, fake => fake.Commerce.Categories(4).Select((x, i) => new TagImportModel
        {
            Name = $"Category {i}",
            Value = x
        }).ToTagsObject());
        RuleFor(fake => fake.CreatedOn, _baseCreationDate);
    }

}
