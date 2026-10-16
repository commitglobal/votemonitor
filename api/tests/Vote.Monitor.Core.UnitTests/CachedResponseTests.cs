using System.Text.Json;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Core.UnitTests;

public class CachedResponseTests
{
    private sealed class TestResponse : CachedResponse
    {
        public string Value { get; set; } = "";
    }

    [Fact]
    public void ShouldHaveNoCacheInfo_WhenNotCached()
    {
        var response = new TestResponse();

        response.CachedAt.Should().BeNull();
        response.ExpiresAt.Should().BeNull();
        response.ValidForMinutes.Should().BeNull();
    }

    [Fact]
    public void MarkCached_ShouldSetCachedAtAndExpiresAt()
    {
        var response = new TestResponse();

        response.MarkCached(TimeSpan.FromMinutes(5));

        response.CachedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        response.ExpiresAt.Should().Be(response.CachedAt!.Value.AddMinutes(5));
        response.ValidForMinutes.Should().Be(5);
    }

    [Fact]
    public void ValidForMinutes_ShouldRoundUpRemainingMinutes()
    {
        var response = new TestResponse { ExpiresAt = DateTime.UtcNow.AddMinutes(2.5) };

        response.ValidForMinutes.Should().Be(3);
    }

    [Fact]
    public void ValidForMinutes_ShouldBeZero_WhenEntryExpired()
    {
        var response = new TestResponse
        {
            CachedAt = DateTime.UtcNow.AddMinutes(-40),
            ExpiresAt = DateTime.UtcNow.AddMinutes(-10)
        };

        response.ValidForMinutes.Should().Be(0);
    }

    [Fact]
    public async Task Track_ShouldMarkResponsesCreatedByFactory()
    {
        var factory = CachedResponse.Track(_ => Task.FromResult(new TestResponse { Value = "fresh" }),
            TimeSpan.FromMinutes(10));

        var response = await factory(CancellationToken.None);

        response.Value.Should().Be("fresh");
        response.CachedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        response.ExpiresAt.Should().Be(response.CachedAt!.Value.AddMinutes(10));
    }

    [Fact]
    public async Task Track_ShouldPassThroughNull()
    {
        var factory = CachedResponse.Track(_ => Task.FromResult<TestResponse?>(null), TimeSpan.FromMinutes(10));

        var response = await factory(CancellationToken.None);

        response.Should().BeNull();
    }

    [Fact]
    public void ShouldSerializeCacheInfo()
    {
        var response = new TestResponse();
        response.MarkCached(TimeSpan.FromMinutes(15));

        var json = JsonDocument.Parse(JsonSerializer.Serialize(response)).RootElement;

        json.GetProperty(nameof(CachedResponse.CachedAt)).GetDateTime().Should().Be(response.CachedAt!.Value);
        json.GetProperty(nameof(CachedResponse.ExpiresAt)).GetDateTime().Should().Be(response.ExpiresAt!.Value);
        json.GetProperty(nameof(CachedResponse.ValidForMinutes)).GetInt32().Should().Be(15);
    }
}
