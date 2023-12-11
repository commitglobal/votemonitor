using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Api.Feature.Forms.Specifications;

public class ListFormsSpecification : Specification<FormAggregate>
{
    public ListFormsSpecification(string? formCodeFilter, string? languageFilter, FormStatus? status, int pageSize, int page)
    {
        Query
            .Search(x => x.Code, "%" + formCodeFilter + "%", !string.IsNullOrEmpty(formCodeFilter))
            //.Where(x => x.LanguageCode == languageFilter, !string.IsNullOrEmpty(languageFilter))
            .Where(x => x.Status == status, status != null)
            .Skip(PaginationHelper.CalculateSkip(pageSize, page))
            .Take(PaginationHelper.CalculateTake(pageSize));
    }
}
