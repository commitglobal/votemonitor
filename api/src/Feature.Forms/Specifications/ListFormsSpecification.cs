using Vote.Monitor.Domain.Specifications;

namespace Feature.Forms.Specifications;

public sealed class ListFormsSpecification : Specification<FormAggregate, FormSlimModel>
{
    public ListFormsSpecification(List.Request request)
    {
        Query
            .Where(x=>x.ElectionRoundId == request.ElectionRoundId && x.MonitoringNgo.NgoId == request.NgoId)
            .Search(x => x.Code, "%" + request.CodeFilter + "%", !string.IsNullOrEmpty(request.CodeFilter))
            .Where(x => x.Status == request.Status, request.Status != null)
            .Where(x => x.DefaultLanguage == request.LanguageCode || x.Languages.Contains(request.LanguageCode), !string.IsNullOrWhiteSpace(request.LanguageCode))
            .ApplyOrdering(request)
            .Paginate(request);

        Query.Select(form => FormSlimModel.FromEntity(form));
    }
}
