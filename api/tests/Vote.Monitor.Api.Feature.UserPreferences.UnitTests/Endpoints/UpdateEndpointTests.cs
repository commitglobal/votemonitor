using Vote.Monitor.Api.Feature.UserPreferences.Update;

namespace Vote.Monitor.Api.Feature.UserPreferences.UnitTests.Endpoints;

public class UpdateEndpointTests
{
    [Fact]
    public async Task ShouldCallServiceWhenValidationPasses()
    {
        //arrange
        var repository = Substitute.For<IRepository<ApplicationUser>>();
        var endpoint = Factory.Create<Endpoint>(repository);
        var appUser = new ApplicationUserFaker().Generate();
        repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(appUser);

        var request = new Request { Id = Guid.NewGuid(), LanguageCode = "EN" };

        //act 

        var response = await endpoint.ExecuteAsync(request, default);

        //assert
        await repository.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
        response.Should().BeOfType<Results<NoContent, NotFound<string>>>()
            .Which.Result.Should().BeOfType<NoContent>();

    }

    [Fact]
    public async Task ShouldReturnUserNotFoundWhenUserIdDoesBotExist()
    {
        //arrange
        var repository = Substitute.For<IRepository<ApplicationUser>>();
        var endpoint = Factory.Create<Endpoint>(repository);
        ApplicationUser appUser = null;
        repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(appUser);

        var request = new Request { Id = Guid.NewGuid(), LanguageCode = "UNKNOWN" };

        //act 
        var response = await endpoint.ExecuteAsync(request, default);

        //assert

        await repository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        response.Should().BeOfType<Results<NoContent, NotFound<string>>>()
            .Which.Result.Should().BeOfType<NotFound<string>>();
        response.Should().BeOfType<Results<NoContent, NotFound<string>>>()
            .Which
            .Result.Should().BeOfType<NotFound<string>>()
            .Which.Value.Should().Be("User not found");

    }
}
