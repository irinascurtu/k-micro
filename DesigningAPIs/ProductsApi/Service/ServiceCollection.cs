using ProductsApi.Data.Repositories;
using ProductsApi.Service.lifetimes;

namespace ProductsApi.Service
{
    public static class ServiceCollection
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddTransient<ITransientService, TransientService>();
            services.AddScoped<IScopedService, ScopedService>();
            services.AddSingleton<ISingletonService, SingletonService>();

            return services;
        }
    }
}
