namespace Feature.Feedback.Specifications;

public sealed class GetSubmittedFeedbackSpecification : Specification<FeedbackAggregate, FeedbackModel>
{
    public GetSubmittedFeedbackSpecification(Guid electionRoundId)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Include(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .AsNoTracking();

        Query.Select(x => new()
        {
            Id = x.Id,
            ElectionRoundId = x.ElectionRoundId,
            ObserverId = x.ObserverId,
            FirstName = x.Observer.ApplicationUser.FirstName,
            LastName = x.Observer.ApplicationUser.LastName,
            UserFeedback = x.UserFeedback,
            TimeSubmitted = x.TimeSubmitted
        });
    }
}
