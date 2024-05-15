using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating;

public interface IEmailTemplateFactory
{
    EmailModel GenerateEmail(EmailTemplateType templateType, BaseEmailProps emailProps);
}
