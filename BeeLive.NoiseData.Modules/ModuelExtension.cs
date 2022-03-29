using BeeLive.Core.Repositories;
using BeeLive.NoiseData.Core.Repositories;
using BeeLive.NoiseData.Core.Settings;
using BeeLive.NoiseData.Persistence;
using BeeLive.NoiseData.Service;
using BeeLive.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeeLive.NoiseData.Modules
{
    public static class ModuelExtension
    {
        public static IServiceCollection AddNoiseData(this IServiceCollection services, IConfigurationRoot config) 
        {
            services.AddTransient<INoiseDataRepository, NoiseDataRepository>();
            services.AddTransient<INoiseDataService, NoiseDataService>();
            services.Configure<NoiseDataSettings>(x => config.GetSection("NoiseData").Bind(x));
            return services;
        }
    }
}