using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Api.Feature.Forms.Specifications;

public class ListFormsSpecification : Specification<FormAggregate>
{
    public ListFormsSpecification(string? formCodeFilter, Guid? languageId, FormStatus? status, int pageSize, int page)
    {
        Query
            .Search(x => x.Code, "%" + formCodeFilter + "%", !string.IsNullOrEmpty(formCodeFilter))
            .Where(x => x.LanguageId == languageId, languageId != null)
            .Where(x => x.Status == status, status != null)
            .Skip(PaginationHelper.CalculateSkip(pageSize, page))
            .Take(PaginationHelper.CalculateTake(pageSize));
    }
}
