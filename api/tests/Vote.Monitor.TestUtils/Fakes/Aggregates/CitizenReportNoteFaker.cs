using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.CitizenReportNoteAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class CitizenReportNoteFaker : PrivateFaker<CitizenReportNote>
{
    public CitizenReportNoteFaker(Guid? id = null,
        string? text = null,
        ElectionRound? electionRound = null,
        CitizenReport? citizenReport = null)
    {
        UsePrivateConstructor();

        electionRound ??= new ElectionRoundAggregateFaker().Generate();
        citizenReport ??= new CitizenReportFaker(electionRound: electionRound).Generate();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.Text, text ?? string.Empty);
        RuleFor(fake => fake.ElectionRound, electionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound.Id);
        RuleFor(fake => fake.CitizenReport, citizenReport);
        RuleFor(fake => fake.CitizenReportId, citizenReport.Id);
    }
}