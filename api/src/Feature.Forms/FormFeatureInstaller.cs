using Dapper;
using Feature.Forms.Models;
using Microsoft.Extensions.DependencyInjection;
using Module.Forms.Models;
using Vote.Monitor.Core.Converters;

namespace Feature.Forms;

public static class FormFeatureInstaller
{
    public static IServiceCollection AddFormFeature(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(typeof(FormAccessModel[]), new JsonToObjectConverter<FormAccessModel[]>());
        SqlMapper.AddTypeHandler(typeof(BaseQuestionModel[]), new JsonToObjectConverter<BaseQuestionModel[]>());

        return services;
    }
}
