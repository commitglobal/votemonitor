using Ardalis.Specification;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Form.Submissions.Specifications;

public sealed class GetFormSubmissions : Specification<FormSubmission, FormSubmissionModel>
{
    public GetFormSubmissions(List.Request request)
    {
        Query
            .Where(x => x.ElectionRoundId == request.ElectionRoundId 
                         && x.MonitoringObserver.MonitoringNgoId == request.NgoId)
            .ApplyOrdering(request)
            .Paginate(request);

         Query.Select(x => FormSubmissionModel.FromEntity(x));
    }
}
