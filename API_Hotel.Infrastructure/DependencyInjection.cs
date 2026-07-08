using API_Hotel.Domain.Repositories;
using API_Hotel.Infrastructure.Data;
using API_Hotel.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API_Hotel.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<DapperContext>();
            services.AddScoped<IHotelRepository, HotelRepository>();
            //services.AddScoped<IHotelRepository, HotelRepository>();
            //services.AddScoped<IBookingRepository, BookingRepository>();
            //services.AddScoped<IEventPublisher, RabbitMQEventPublisher>();
            return services;
        }
    }
}
