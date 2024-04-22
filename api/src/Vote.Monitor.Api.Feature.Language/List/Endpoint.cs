using Vote.Monitor.Core.Constants;

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
            .GetAll()
            .Select(language => new LanguageModel
            {
                Id = language.Id,
                Code = language.Iso1,
                Name = language.Name,
                NativeName = language.NativeName
            }).ToList();

        return Task.FromResult(languages);
    }
}
