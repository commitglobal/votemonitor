using Vote.Monitor.Domain.Entities.ObserverAggregate;

namespace Vote.Monitor.Domain.Entities.FeedbackAggregate
{
    public class Feedback : BaseEntity, IAggregateRoot
    {
#pragma warning disable CS8618 // Required by Entity Framework
        private Feedback()
        {
        }
#pragma warning restore CS8618

        public Guid ElectionRoundId { get; private set; }
        public ElectionRound ElectionRound { get; private set; }
        public Guid ObserverId { get; private set; }
        public Observer Observer { get; private set; }
        public string UserFeedback { get; private set; }
        public DateTime TimeSubmitted { get; set; }
        public Dictionary<string, string> Metadata { get; private set; }
        public Feedback(
            Guid electionRoundId,
            Guid observerId,
            string userFeedback,
            DateTime timeSubmitted,
            Dictionary<string, string> metadata) : base(Guid.NewGuid())
        {
            ElectionRoundId = electionRoundId;
            ObserverId = observerId;
            UserFeedback = userFeedback;
            TimeSubmitted = timeSubmitted;
            Metadata = metadata;
            CreatedOn = timeSubmitted;
        }
    }
}
