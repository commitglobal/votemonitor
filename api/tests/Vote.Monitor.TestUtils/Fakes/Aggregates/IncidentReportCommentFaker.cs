using Vote.Monitor.Domain.Entities.IncidentReportCommentAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class IncidentReportCommentFaker : PrivateFaker<IncidentReportComment>
{
    public IncidentReportCommentFaker(Guid? id = null,
        string? text = null,
        Guid? electionRoundId = null,
        Guid? incidentReportId = null,
        Guid? authorId = null)
    {
        UsePrivateConstructor();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.Text, text ?? "an incident report comment");
        RuleFor(fake => fake.ElectionRoundId, fake => electionRoundId ?? fake.Random.Guid());
        RuleFor(fake => fake.IncidentReportId, fake => incidentReportId ?? fake.Random.Guid());
        RuleFor(fake => fake.CreatedBy, fake => authorId ?? fake.Random.Guid());
    }
}
