namespace Vote.Monitor.Api.Feature.Language.Get;

public class Endpoint : Endpoint<Request, Results<Ok<LanguageModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/languages/{id}");
    }

    public override Task<Results<Ok<LanguageModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var language = LanguagesList.GetAllIso1().FirstOrDefault(x => x.Id == req.Id);
        if (language is null)
        {
            return Task.FromResult<Results<Ok<LanguageModel>, NotFound>>(TypedResults.NotFound());
        }

        var languageModel = new LanguageModel
        {
            Id = language.Id,
            Name = language.Name,
            Iso1 = language.Iso1,
            Iso3 = language.Iso3
        };

        return Task.FromResult<Results<Ok<LanguageModel>, NotFound>>(TypedResults.Ok(languageModel));
    }
}
