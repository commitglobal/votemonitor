using System.Text;
using Job.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Core.Options;
using Vote.Monitor.Core.Services.EmailTemplating;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Feature.Auth.ForgotPassword;
public class Endpoint(
    UserManager<ApplicationUser> userManager,
    IJobService jobService,
    IOptions<ApiConfiguration> apiConfig,
    IEmailTemplateFactory emailFactory,
    ILogger<Endpoint> logger)
    : Endpoint<Request, Results<Ok, ProblemHttpResult>>
{
    private readonly ApiConfiguration _apiConfig = apiConfig.Value;

    public override void Configure()
    {
        Post("/api/auth/forgot-password");
        AllowAnonymous();
    }

    public override async Task<Results<Ok, ProblemHttpResult>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
        {
            logger.LogWarning("Possible user enumeration. Unknown email received {email}", request.Email);
            // Don't reveal that the user does not exist or is not confirmed
            return TypedResults.Ok();
        }

        var mail = await userManager.IsEmailConfirmedAsync(user)
            ? await GetResetPasswordEmail(user)
            : GetAcceptInviteEmail(user);

        jobService.EnqueueSendEmail(request.Email, mail.Subject, mail.Body);

        return TypedResults.Ok();
    }

    private async Task<EmailModel> GetResetPasswordEmail(ApplicationUser user)
    {
        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        
        var endpointUri = new Uri(Path.Combine($"{_apiConfig.WebAppUrl}", "reset-password"));
        string passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "token", code);

        var emailProps = new ResetPasswordEmailProps(FullName: user.DisplayName, CdnUrl: _apiConfig.WebAppUrl, ResetPasswordUrl: passwordResetUrl);
        var mail = emailFactory.GenerateResetPasswordEmail(emailProps);

        return mail;
    }

    private EmailModel GetAcceptInviteEmail(ApplicationUser user)
    {
        var endpointUri = new Uri(Path.Combine($"{_apiConfig.WebAppUrl}", "accept-invite"));
        string acceptInviteUrl =
            QueryHelpers.AddQueryString(endpointUri.ToString(), "invitationToken", user.InvitationToken);

        var confirmEmailProps = new ConfirmEmailProps(
            CdnUrl: _apiConfig.WebAppUrl,
            FullName: user.DisplayName,
            ConfirmUrl: acceptInviteUrl);

        var mail = emailFactory.GenerateConfirmAccountEmail(confirmEmailProps);
        return mail;
    }
}
