using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Data;

namespace Vote.Monitor.Core;
public static  class CoreSetupStartup
{
    public static IServiceCollection AddCoreStartup(this IServiceCollection services, IConfiguration config)
    {

       services.AddDbContext<AppDbContext>(options =>
      {

           options.UseNpgsql(config.GetConnectionString("PostgresVoteApiConnctionString"));
       });
        return services;
    }
}
