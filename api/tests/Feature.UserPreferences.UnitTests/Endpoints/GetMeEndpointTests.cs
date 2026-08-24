using Feature.UserPreferences;

namespace Feature.UserPreferences.UnitTests.Endpoints;

public class GetMeEndpointTests
{
    [Fact]
    public async Task ShouldReturnUserWhenUserExists()
    {
        //arrange
        var repository = Substitute.For<IReadRepository<ApplicationUser>>();
        var endpoint = Factory.Create<GetMe.Endpoint>(repository);
        var appUser = ApplicationUser.CreateObserver("Jane", "Doe", "jane@example.com", "555-0100", "Password1!");
        repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(appUser);

        var request = new GetMe.Request { Id = appUser.Id };

        //act
        var response = await endpoint.ExecuteAsync(request, CancellationToken.None);

        //assert
        var result = response.Should().BeOfType<Results<Ok<UserModel>, NotFound<string>>>()
            .Which.Result.Should().BeOfType<Ok<UserModel>>()
            .Which.Value;

        result.Should().NotBeNull();
        result!.Id.Should().Be(appUser.Id);
        result.Email.Should().Be(appUser.Email);
        result.FirstName.Should().Be(appUser.FirstName);
        result.LastName.Should().Be(appUser.LastName);
        result.DisplayName.Should().Be($"{appUser.FirstName} {appUser.LastName}");
        result.PhoneNumber.Should().Be(appUser.PhoneNumber);
        result.Role.Should().Be(appUser.Role);
        result.Status.Should().Be(appUser.Status);
        result.Preferences.LanguageCode.Should().Be(appUser.Preferences.LanguageCode);
    }

    [Fact]
    public async Task ShouldReturnUserNotFoundWhenUserIdDoesNotExist()
    {
        //arrange
        var repository = Substitute.For<IReadRepository<ApplicationUser>>();
        var endpoint = Factory.Create<GetMe.Endpoint>(repository);
        ApplicationUser appUser = null;
        repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(appUser);

        var request = new GetMe.Request { Id = Guid.NewGuid() };

        //act
        var response = await endpoint.ExecuteAsync(request, CancellationToken.None);

        //assert
        response.Should().BeOfType<Results<Ok<UserModel>, NotFound<string>>>()
            .Which.Result.Should().BeOfType<NotFound<string>>();
        response.Should().BeOfType<Results<Ok<UserModel>, NotFound<string>>>()
            .Which
            .Result.Should().BeOfType<NotFound<string>>()
            .Which.Value.Should().Be("User not found");
    }
}
