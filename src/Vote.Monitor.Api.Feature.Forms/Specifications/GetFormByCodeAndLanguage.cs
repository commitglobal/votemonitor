namespace Vote.Monitor.Api.Feature.Forms.Specifications;

public class GetFormByCodeAndLanguage : Specification<FormAggregate>, ISingleResultSpecification<FormAggregate>
{
    public GetFormByCodeAndLanguage(string formCode, Guid languageId)
    {
        Query.Where(x => x.Code == formCode
                         && x.LanguageId == languageId);
    }
}
