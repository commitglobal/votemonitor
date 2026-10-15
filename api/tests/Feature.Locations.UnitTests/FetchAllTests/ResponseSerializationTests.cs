using Feature.Locations.FetchAll;

namespace Feature.Locations.UnitTests.FetchAllTests;

public class ResponseSerializationTests
{
    [Fact]
    public void Response_ShouldRoundTrip_WhenReadBackFromDistributedCache()
    {
        // Arrange
        var root = new LocationNode(0, Guid.Empty, "Root", 0, -1, 0);
        var parent = new LocationNode(1, Guid.NewGuid(), "Level 1", 1, root.Id, 3);
        var leaf = new LocationNode(2, Guid.NewGuid(), "Level 2", 2, parent.Id, 7);
        root.AddChild(parent);
        parent.AddChild(leaf);

        var response = new Response
        {
            ElectionRoundId = Guid.NewGuid(),
            Version = "42",
            Nodes = root.Traverse().Skip(1).ToList()
        };
        response.MarkCached(TimeSpan.FromMinutes(30));

        // Act
        var result = JsonSerializer.Deserialize<Response>(JsonSerializer.Serialize(response));

        // Assert
        // Children are not serialized: the endpoint returns the nodes flattened
        result.Should().BeEquivalentTo(response, options => options.Excluding(x => x.Path.EndsWith("Children")));
        result!.Nodes[0].LocationId.Should().BeNull();
        result.Nodes[1].LocationId.Should().Be(leaf.LocationId);
    }
}
