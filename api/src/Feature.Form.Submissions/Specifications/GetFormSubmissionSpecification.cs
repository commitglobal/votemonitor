﻿using Ardalis.Specification;

namespace Feature.Form.Submissions.Specifications;

public sealed class GetFormSubmissionSpecification : SingleResultSpecification<FormSubmission>
{
    public GetFormSubmissionSpecification(Guid electionRoundId, Guid pollingStationId, Guid formId, Guid observerId)
    {
        Query.Where(x =>
            x.ElectionRoundId == electionRoundId
            && x.MonitoringObserver.ObserverId == observerId
            && x.MonitoringObserver.ElectionRoundId == electionRoundId
            && x.PollingStationId == pollingStationId
            && x.Form.ElectionRoundId == electionRoundId
            && x.FormId == formId);
    }

    public GetFormSubmissionSpecification(Guid electionRoundId, Guid ngoId, Guid submissionId)
    {
        Query.Where(x =>
            x.ElectionRoundId == electionRoundId
            && x.MonitoringObserver.MonitoringNgo.ElectionRoundId == electionRoundId
            && x.MonitoringObserver.MonitoringNgo.NgoId == ngoId
            && x.Id == submissionId);
    }
}
