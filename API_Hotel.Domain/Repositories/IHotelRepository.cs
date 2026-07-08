using API_Hotel.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Hotel.Domain.Repositories
{
    public interface IHotelRepository
    {
        // Hoteles
        Task<IEnumerable<Hotel>> ObtenerTodosActivosAsync();
        Task<Hotel?> ObtenerPorIdAsync(int id);
        Task<int> InsertarHotelAsync(Hotel hotel);
        Task ActualizarHotelAsync(Hotel hotel);
        Task CambiarEstadoHotelAsync(int id, bool estaHabilitado);
        Task EliminarHotelAsync(int id);
        // Habitaciones
        Task<IEnumerable<Habitacion>> ObtenerHabitacionesPorHotelIdAsync(int hotelId);
        Task<Habitacion?> ObtenerHabitacionPorIdAsync(int id);
        Task<int> InsertarHabitacionAsync(Habitacion habitacion);
        Task ActualizarHabitacionAsync(Habitacion habitacion);
    }
}
