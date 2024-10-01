using Vote.Monitor.TestUtils.Fakes;

namespace Feature.IssueReports.Notes.UnitTests.Endpoints;

public sealed class IssueReportNoteModelFaker : PrivateFaker<IssueReportNoteModel>
{
    public IssueReportNoteModelFaker()
    {
        RuleFor(fake => fake.Id, fake => fake.Random.Guid());
        RuleFor(fake => fake.Text, fake => fake.Lorem.Sentence());
        RuleFor(fake => fake.ElectionRoundId, fake => fake.Random.Guid());
        RuleFor(fake => fake.IssueReportId, fake => fake.Random.Guid());
        RuleFor(fake => fake.FormId, fake => fake.Random.Guid());
        RuleFor(fake => fake.QuestionId, fake => fake.Random.Guid());
        RuleFor(fake => fake.CreatedAt, fake => fake.Date.Recent());
        RuleFor(fake => fake.UpdatedAt, fake => fake.Date.Recent());
    }
}