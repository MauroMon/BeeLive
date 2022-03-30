using BeeLive.Hive.Core.Repositories;
using BeeLive.Hive.Persistence;
using BeeLive.Hive.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BeeLive.Hive.Modules
{
    public static class ModulExtension
    {
        public static IServiceCollection AddHive(this IServiceCollection services)
        {
            services.AddTransient<IHiveRepository, HiveRespository>();
            services.AddTransient<IHiveService, HiveService>();
            return services;
        }
    }
}