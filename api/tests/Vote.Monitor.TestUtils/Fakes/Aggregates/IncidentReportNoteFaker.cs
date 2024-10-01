using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportNoteAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class IncidentReportNoteFaker : PrivateFaker<IncidentReportNote>
{
    public IncidentReportNoteFaker(Guid? id = null,
        string? text = null,
        ElectionRound? electionRound = null,
        IncidentReport? incidentReport = null)
    {
        UsePrivateConstructor();

        electionRound ??= new ElectionRoundAggregateFaker().Generate();
        incidentReport ??= new IncidentReportFaker(electionRound: electionRound).Generate();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.Text, text ?? string.Empty);
        RuleFor(fake => fake.ElectionRound, electionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound.Id);
        RuleFor(fake => fake.IncidentReport, incidentReport);
        RuleFor(fake => fake.IncidentReportId, incidentReport.Id);
    }
}