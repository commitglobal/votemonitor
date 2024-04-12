using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.Mailing.Ses;
using Vote.Monitor.Core.Services.Mailing.Smtp;

namespace Vote.Monitor.Core.Services.Mailing;
public static class MailingInstaller
{
    public const string SectionKey = "Mailing";
    public static IServiceCollection AddMailing(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        switch (configuration.GetValue<MailSenderType>("MailSenderType"))
        {
            case MailSenderType.SMTP:
                var smtpSection = configuration.GetSection(SmtpOptions.SectionName);
                return serviceCollection.AddSmtp(smtpSection);

            case MailSenderType.SES:
                var sesSection = configuration.GetSection(SesOptions.SectionName);
                return serviceCollection.AddSes(sesSection);

            default:
                throw new ArgumentException("Unknown configuration for MailSenderType", nameof(configuration));
        }
    }
}
