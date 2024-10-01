using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportAttachmentAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class IncidentReportAttachmentFaker : PrivateFaker<IncidentReportAttachment>
{
    public IncidentReportAttachmentFaker(Guid? id = null,
        string? fileName = null,
        string? filePath = null,
        ElectionRound? electionRound = null,
        IncidentReport? incidentReport = null)
    {
        UsePrivateConstructor();

        electionRound ??= new ElectionRoundAggregateFaker().Generate();
        incidentReport ??= new IncidentReportFaker(electionRound: electionRound).Generate();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.FileName, fileName ?? string.Empty);
        RuleFor(fake => fake.FilePath, filePath ?? string.Empty);
        RuleFor(fake => fake.ElectionRound, electionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound.Id);

        RuleFor(fake => fake.IncidentReport, incidentReport);
        RuleFor(fake => fake.IncidentReportId, incidentReport.Id);
    }
}