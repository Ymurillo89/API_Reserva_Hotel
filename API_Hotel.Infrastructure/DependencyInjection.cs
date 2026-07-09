using API_Hotel.Domain.Repositories;
using API_Hotel.Infrastructure.Data;
using API_Hotel.Infrastructure.Repositories;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TuProyecto.Infrastructure.Repositories;
using TuProyecto.Infrastructure.Services;

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
            return services;
        }
    }
}
