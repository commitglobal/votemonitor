using Vote.Monitor.Api.Feature.UserPreferences.Helpers;
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

        LanguageDetails? language = default;
        if (req.LanguageIso != null) language = LanguagesList.GetByIso(req.LanguageIso.ToUpperInvariant());
        else language = LanguagesList.GetAll().FirstOrDefault(x => x.Id == req.LanguageId);

        if (language == null)
        {
            return TypedResults.NotFound("Language not found");
        }

        UserPreferencesModel userPreferencesModel = new UserPreferencesModel
        {
            Id = req.Id,
            Preferences = new Dictionary<string, string>()
        };
        userPreferencesModel.Preferences.Add("LanguageId", language.Id.ToString());
        userPreferencesModel.Preferences.Add("LanguageIso", language.Iso1);
        if (req.Preferences != null)
            foreach (var preference in req.Preferences)
            {
                userPreferencesModel.Preferences.Add(preference.Key, preference.Value);
            }

        user.UpdatePreferences(userPreferencesModel.Preferences.toPreferencesObject());
        await repository.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
