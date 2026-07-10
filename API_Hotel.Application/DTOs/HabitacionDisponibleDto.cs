using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Hotel.Application.DTOs
{
    public record HabitacionDisponibleDto(
        int HabitacionId,
        int HotelId,
        string NombreHotel,
        string Ciudad,
        string TipoHabitacion,
        decimal CostoBase,
        decimal Impuesto,
        string Ubicacion,
        int Capacidad
    );
}
