
using API_Hotel.Domain.Entities;

namespace Domain.Repositories; 

public interface IReservaRepository
{
    Task<bool> ExisteSolapamientoAsync(int habitacionId, string fechaEntrada, string fechaSalida);
    Task<IEnumerable<HabitacionDisponible>> BuscarHabitacionesDisponiblesAsync(string? ciudad, string fechaEntrada, string fechaSalida);
    Task<int> CrearReservaConHuespedesAsync(Reserva reserva);
    Task<IEnumerable<ReservaDetalle>> ObtenerReservasConDetallesAsync();
}