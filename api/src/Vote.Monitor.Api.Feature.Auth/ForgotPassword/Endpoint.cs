using Job.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Core.Options;
using Vote.Monitor.Core.Services.EmailTemplating;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Api.Feature.Auth.ForgotPassword;
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
        var user = await userManager.FindByEmailAsync(request.Email.Normalize());
        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
        {
            logger.LogWarning("Possible user enumeration. Unknown email received {email}", request.Email);
            // Don't reveal that the user does not exist or is not confirmed
            return TypedResults.Ok();
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        string code = await userManager.GeneratePasswordResetTokenAsync(user);
        var endpointUri = new Uri(Path.Combine($"{_apiConfig.WebAppUrl}", "reset-password"));
        string passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "token", code);

        var emailProps = new ResetPasswordEmailProps(string.Empty, passwordResetUrl);
        var mail = emailFactory.GenerateEmail(EmailTemplateType.ResetPassword, emailProps);

        jobService.SendEmail(request.Email, mail.Subject, mail.Body);

        return TypedResults.Ok();
    }
}
