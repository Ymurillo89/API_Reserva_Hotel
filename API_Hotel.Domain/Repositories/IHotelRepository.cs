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
        Task<IEnumerable<Hotel>> ObtenerTodosActivosAsync();
        Task<Hotel?> ObtenerPorIdAsync(int id);
        Task<int> InsertarHotelAsync(Hotel hotel);
        Task ActualizarHotelAsync(Hotel hotel);

        // Operaciones de Habitaciones
        Task<IEnumerable<Habitacion>> ObtenerHabitacionesPorHotelIdAsync(int hotelId);
        Task<int> InsertarHabitacionAsync(Habitacion habitacion);
    }
}
