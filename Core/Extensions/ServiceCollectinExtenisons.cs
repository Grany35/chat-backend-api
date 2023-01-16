using Core.Utilities.Hashing;
using Core.Utilities.IoC;
using Entities.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions
{
    public static class ServiceCollectinExtenisons
    {
        public static IServiceCollection AddDepdencyResolvers(this IServiceCollection services, ICoreModule[] modules)
        {
            foreach (var modul in modules)
            {
                modul.Load(services);
            }

            return ServiceTool.Create(services);
        }
    }
}
