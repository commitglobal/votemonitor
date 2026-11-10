using Vote.Monitor.Core.Services.EmailTemplating;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.UnitTests;

public class EmailTests
{
    // The factory is internal; its interface is what the app uses.
    private static readonly IEmailTemplateFactory Factory = (IEmailTemplateFactory)Activator.CreateInstance(
        typeof(IEmailTemplateFactory).Assembly.GetType("Vote.Monitor.Core.Services.EmailTemplating.EmailTemplateFactory")!,
        nonPublic: true)!;

    private static readonly string Year = DateTime.UtcNow.Year.ToString();

    private static IEnumerable<EmailModel> AllEmails() =>
    [
        Factory.GenerateResetPasswordEmail(new ResetPasswordEmailProps("Ana", "https://cdn", "https://app/reset")),
        Factory.GenerateConfirmAccountEmail(new ConfirmEmailProps("https://cdn", "Ana", "https://app/confirm")),
        Factory.GenerateNewUserInvitationEmail(new InvitationNewUserEmailProps("Ana", "https://cdn", "https://app/accept", "Alfa", "Round A")),
        Factory.GenerateInvitationExistingUserEmail(new InvitationExistingUserEmailProps("Ana", "https://cdn", "Alfa", "Round A")),
        Factory.GenerateCitizenReportEmail(new CitizenReportEmailProps("Title", "Heading", "Preview", [], "https://cdn")),
    ];

    [Fact]
    public void Every_email_carries_the_current_year_in_both_bodies()
    {
        foreach (var email in AllEmails())
        {
            email.Body.Should().Contain($"© {Year} Commit Global").And.NotContain("~$").And.NotContain("2024");
            email.TextBody.Should().Contain($"© {Year} Commit Global").And.NotContain("~$").And.NotContain("<");
        }
    }

    [Fact]
    public void Text_body_of_the_new_user_invitation()
    {
        var email = Factory.GenerateNewUserInvitationEmail(
            new InvitationNewUserEmailProps("Ana Popescu", "https://cdn", "https://app/accept?token=a&b=1", "Alfa", "Round A"));

        email.TextBody.Should().StartWith("Hello, Ana Popescu\n");
        email.TextBody.Should().Contain("Alfa has invited you to be an observer for Round A.");
        email.TextBody.Should().Contain("in order to use the application:\nhttps://app/accept?token=a&b=1\n");
    }

    [Fact]
    public void Text_body_of_a_citizen_report_lists_the_answers()
    {
        var email = Factory.GenerateCitizenReportEmail(new CitizenReportEmailProps("Title", "Your report", "Preview",
        [
            new InputAnswerFragmentProps("What happened?", "Notes", [new NoteFragmentProps("1.", "A note")],
                "Attachments", [new AttachmentFragmentProps("photo.jpg", "https://cdn/photo.jpg")], "Queue outside"),
            new SelectAnswerFragmentProps("Which?", "Notes", [], "Attachments", [],
                [new SelectAnswerOptionFragmentProps("Yes", true), new SelectAnswerOptionFragmentProps("No", false)]),
            new RatingAnswerFragmentProps("How bad?", "Notes", [], "Attachments", [], 3, 2),
        ], "https://cdn"));

        email.TextBody.Should().Be(
            $"""
             Your report

             What happened?
             Queue outside

             Notes
             1. A note

             Attachments
             photo.jpg: https://cdn/photo.jpg

             Which?
             [x] Yes
             [ ] No

             How bad?
             ( 1 ) [ 2 ] ( 3 )

             © {Year} Commit Global. All rights reserved.

             """);
    }
}
