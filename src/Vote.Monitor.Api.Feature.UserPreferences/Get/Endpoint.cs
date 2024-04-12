using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.Api.Feature.UserPreferences.Get;

public class Endpoint(IReadRepository<ApplicationUser> repository) : Endpoint<Request, Results<Ok<UserPreferencesModel>, NotFound<string>>>
{
    public override void Configure()
    {
        Get("/api/preferences");
    }

    public override async Task<Results<Ok<UserPreferencesModel>, NotFound<string>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(req.Id, ct);
        if (user is null)
        {
            return TypedResults.NotFound("User not found");
        }

        return TypedResults.Ok(new UserPreferencesModel
        {
            LanguageId = user.Preferences.LanguageId
        });
    }
}
