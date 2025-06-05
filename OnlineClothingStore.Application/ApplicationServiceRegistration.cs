using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OnlineClothingStore.Application.Behaviors;

namespace OnlineClothingStore.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
