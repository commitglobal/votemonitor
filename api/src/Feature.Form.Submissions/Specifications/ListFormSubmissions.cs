using Ardalis.Specification;

namespace Feature.Form.Submissions.Specifications;

public sealed class ListFormSubmissions : Specification<FormSubmission>
{
    public ListFormSubmissions(ListByForm.Request request)
    {
        Query
            .Include(x => x.MonitoringObserver)
            .ThenInclude(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .Where(x => x.ElectionRoundId == request.ElectionRoundId &&
                        x.MonitoringObserver.MonitoringNgo.NgoId == request.NgoId);
    }
}
