using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal interface IEmailGenerator<in TEmailProps> where TEmailProps : BaseEmailProps
{
    string Generate(TEmailProps props);
}
