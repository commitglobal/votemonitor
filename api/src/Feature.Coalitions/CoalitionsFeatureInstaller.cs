using Dapper;
using Feature.NgoCoalitions.Models;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Converters;

namespace Feature.NgoCoalitions;

public static class CoalitionsFeatureInstaller
{
    public static IServiceCollection AddCoalitionsFeature(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(typeof(CoalitionMember[]), new JsonToObjectConverter<CoalitionMember[]>());

        return services;
    }
}
