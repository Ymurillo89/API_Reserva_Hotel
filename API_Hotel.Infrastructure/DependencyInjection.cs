using API_Hotel.Application;
using API_Hotel.Domain.Repositories;
using API_Hotel.Infrastructure.Data;
using API_Hotel.Infrastructure.Messaging;
using API_Hotel.Infrastructure.Repositories;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using API_Hotel.Infrastructure.Infrastructure.Services;
using API_Hotel.Infrastructure.Infrastructure.Repositories;


namespace API_Hotel.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<DapperContext>();
            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IJwtAuthService, JwtAuthService>();
            services.AddScoped<IReservaRepository, ReservaRepository>();

            services.AddTransient<DatabaseInitializer>();
            services.AddScoped<IEventPublisher, RabbitMQEventPublisher>();

            return services;
        }
    }
}
