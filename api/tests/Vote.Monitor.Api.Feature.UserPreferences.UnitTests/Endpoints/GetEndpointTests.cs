namespace Vote.Monitor.Api.Feature.UserPreferences.UnitTests.Endpoints;
public class GetEndpointTests
{
    [Fact]
    public async Task ShouldCallServiceWhenValidationPasses()
    {
        //arrange
        var repository = Substitute.For<IReadRepository<ApplicationUser>>();
        var endpoint = Factory.Create<Get.Endpoint>(repository);
        var appUser = new ApplicationUserFaker().Generate();
        repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(appUser);

        var request = new Get.Request { Id = Guid.NewGuid() };

        //act 

        var response = await endpoint.ExecuteAsync(request, CancellationToken.None);


        //assert


        response.Should().BeOfType<Results<Ok<UserPreferencesModel>, NotFound<string>>>()
            .Which.Result.Should().BeOfType<Ok<UserPreferencesModel>>();

    }

    [Fact]
    public async Task ShouldReturnUserNotFoundWhenUserIdDoesNotExist()
    {
        //arrange
        var repository = Substitute.For<IReadRepository<ApplicationUser>>();
        var endpoint = Factory.Create<Get.Endpoint>(repository);
        ApplicationUser appUser = null;
        repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(appUser);

        var request = new Get.Request { Id = Guid.NewGuid() };

        //act 
        var response = await endpoint.ExecuteAsync(request, CancellationToken.None);

        //assert
        response.Should().BeOfType<Results<Ok<UserPreferencesModel>, NotFound<string>>>()
            .Which.Result.Should().BeOfType<NotFound<string>>();
        response.Should().BeOfType<Results<Ok<UserPreferencesModel>, NotFound<string>>>()
            .Which
            .Result.Should().BeOfType<NotFound<string>>()
            .Which.Value.Should().Be("User not found");

    }

}
