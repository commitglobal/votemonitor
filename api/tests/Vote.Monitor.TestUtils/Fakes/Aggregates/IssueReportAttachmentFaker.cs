using Vote.Monitor.Domain.Entities.IssueReportAggregate;
using Vote.Monitor.Domain.Entities.IssueReportAttachmentAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class IssueReportAttachmentFaker : PrivateFaker<IssueReportAttachment>
{
    public IssueReportAttachmentFaker(Guid? id = null,
        string? fileName = null,
        string? filePath = null,
        ElectionRound? electionRound = null,
        IssueReport? issueReport = null)
    {
        UsePrivateConstructor();

        electionRound ??= new ElectionRoundAggregateFaker().Generate();
        issueReport ??= new IssueReportFaker(electionRound: electionRound).Generate();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.FileName, fileName ?? string.Empty);
        RuleFor(fake => fake.FilePath, filePath ?? string.Empty);
        RuleFor(fake => fake.ElectionRound, electionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound.Id);
        
        RuleFor(fake => fake.IssueReport, issueReport);
        RuleFor(fake => fake.IssueReportId, issueReport.Id);
    }
}