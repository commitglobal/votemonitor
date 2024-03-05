using Vote.Monitor.Domain.Entities.LanguageAggregate;

namespace Vote.Monitor.Api.Feature.FormTemplate.Specifications;

public class LanguagesByIsoCode : Specification<Language>
{
    public LanguagesByIsoCode(List<string> languagesIso)
    {
        Query
            .Where(x => languagesIso.Contains(x.Iso1));
        
    }
}
