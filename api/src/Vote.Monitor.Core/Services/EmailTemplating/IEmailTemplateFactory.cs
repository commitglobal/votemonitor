using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating;

public interface IEmailTemplateFactory
{
    string GenerateEmailTemplate<TEmailProps>(EmailTemplateType templateType, TEmailProps emailProps) where TEmailProps : BaseEmailProps;
}
