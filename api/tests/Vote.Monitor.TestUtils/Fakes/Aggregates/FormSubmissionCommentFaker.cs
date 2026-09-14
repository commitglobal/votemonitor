using Vote.Monitor.Domain.Entities.FormSubmissionCommentAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class FormSubmissionCommentFaker : PrivateFaker<FormSubmissionComment>
{
    public FormSubmissionCommentFaker(Guid? id = null,
        string? text = null,
        Guid? electionRoundId = null,
        Guid? submissionId = null,
        Guid? questionId = null,
        Guid? authorId = null)
    {
        UsePrivateConstructor();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.Text, text ?? "a submission comment");
        RuleFor(fake => fake.ElectionRoundId, fake => electionRoundId ?? fake.Random.Guid());
        RuleFor(fake => fake.SubmissionId, fake => submissionId ?? fake.Random.Guid());
        RuleFor(fake => fake.QuestionId, questionId);
        RuleFor(fake => fake.CreatedBy, fake => authorId ?? fake.Random.Guid());
    }
}
