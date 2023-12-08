namespace Vote.Monitor.Api.Feature.Language.List;

public class Endpoint : EndpointWithoutRequest<List<LanguageModel>>
{

    public override void Configure()
    {
        Get("/api/languages");
    }

    public override Task<List<LanguageModel>> ExecuteAsync(CancellationToken ct)
    {
        var languages = LanguagesList
            .GetAllIso1()
            .Select(language => new LanguageModel
            {
                Id = language.Id,
                Name = language.Name,
                Iso1 = language.Iso1,
                Iso3 = language.Iso3
            }).ToList();

        return Task.FromResult(languages);
    }
}
