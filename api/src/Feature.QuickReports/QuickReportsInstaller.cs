using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Converters;

namespace Feature.QuickReports;

public static class QuickReportsInstaller
{
    public static IServiceCollection AddQuickReportsFeature(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(typeof(QuickReportAttachmentModel[]), new JsonToObjectConverter<QuickReportAttachmentModel[]>());

        return services;
    }
}
