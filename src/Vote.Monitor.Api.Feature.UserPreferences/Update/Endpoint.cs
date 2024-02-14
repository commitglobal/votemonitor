using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.Api.Feature.UserPreferences.Update;

public class Endpoint(IRepository<ApplicationUser> repository) : Endpoint<Request, Results<NoContent, NotFound<string>>>
{

    public override void Configure()
    {
        Post("/api/preferences");
    }

    public override async Task<Results<NoContent, NotFound<string>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(req.Id, ct);

        if (user is null)
        {
            return TypedResults.NotFound("User not found");
        }

        var language = LanguagesList.Get(req.LanguageId)!;

        user.Preferences.Update(language.Id);

        await repository.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
