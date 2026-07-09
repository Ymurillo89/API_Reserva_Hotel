using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Hotel.Application.DTOs
{
    public record ReservaDetalleDto(
        int Id, 
        string NombreHotel, 
        string Ciudad, 
        string TipoHabitacion,
        string FechaEntrada, 
        string FechaSalida, 
        int CantidadHuespedes,
        decimal CostoTotal, 
        decimal ImpuestoTotal, 
        string Estado,
        string ContactoEmergenciaNombre, 
        string ContactoEmergenciaTelefono,
        List<HuespedDto> Huespedes
    );
}
