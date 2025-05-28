using Microsoft.Extensions.DependencyInjection;
using OnlineClothingStore.Application;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Infrastructure.Repositories;

namespace OnlineClothingStore.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            return services;
        }
    }
}
