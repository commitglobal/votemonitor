using Ardalis.Specification;
using Feature.Form.Submissions.ListEntries;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Form.Submissions.Specifications;

public sealed class ListFormSubmissionEntriesSpecification : Specification<FormSubmission, FormSubmissionEntries>
{
    public ListFormSubmissionEntriesSpecification(Request request)
    {
        Query
            .Include(x => x.Form)
            .Include(x => x.PollingStation)
            .Include(x => x.MonitoringObserver)
            .ThenInclude(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .Where(x => x.ElectionRoundId == request.ElectionRoundId && x.MonitoringObserver.MonitoringNgo.NgoId == request.NgoId)
            .Where(x => x.MonitoringObserverId == request.MonitoringObserverId, request.MonitoringObserverId.HasValue)
            .Where(x => x.Form.Code == request.FormCodeFilter, !string.IsNullOrWhiteSpace(request.FormCodeFilter))
            .Where(x => x.Form.FormType == request.FormTypeFilter, !string.IsNullOrWhiteSpace(request.FormTypeFilter))
            .Where(x => x.PollingStation.Level1 == request.Level1Filter, !string.IsNullOrWhiteSpace(request.Level1Filter))
            .Where(x => x.PollingStation.Level2 == request.Level2Filter, !string.IsNullOrWhiteSpace(request.Level2Filter))
            .Where(x => x.PollingStation.Level3 == request.Level3Filter, !string.IsNullOrWhiteSpace(request.Level3Filter))
            .Where(x => x.PollingStation.Level4 == request.Level4Filter, !string.IsNullOrWhiteSpace(request.Level4Filter))
            .Where(x => x.PollingStation.Level5 == request.Level5Filter, !string.IsNullOrWhiteSpace(request.Level5Filter))
            .Where(x => x.PollingStation.Number == request.Level5Filter, !string.IsNullOrWhiteSpace(request.PollingStationNumberFilter))
            .Where(x => x.NumberOfFlaggedAnswers > 0, request.HasFlaggedAnswers is true)
            .ApplyOrdering(request)
            .Paginate(request);

        Query.Select(x => new FormSubmissionEntries
        {
            FormCode = x.Form.Code,
            FormType = x.Form.FormType,
            PollingStationId = x.PollingStation.Id,
            PollingStationLevel1 = x.PollingStation.Level1,
            PollingStationLevel2 = x.PollingStation.Level2,
            PollingStationLevel3 = x.PollingStation.Level3,
            PollingStationLevel4 = x.PollingStation.Level4,
            PollingStationLevel5 = x.PollingStation.Level5,
            PollingStationNumber = x.PollingStation.Number,
            SubmittedAt = x.LastModifiedOn ?? x.CreatedOn,
            MonitoringObserverId = x.MonitoringObserverId,
            FirstName = x.MonitoringObserver.Observer.ApplicationUser.FirstName,
            LastName = x.MonitoringObserver.Observer.ApplicationUser.LastName,
            NumberOfQuestionAnswered = x.NumberOfQuestionAnswered,
            NumberOfFlaggedAnswers = x.NumberOfFlaggedAnswers
        });
    }
}
