using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Hotel.Application.Events
{
    public class ReservaCreadaEvent
    {
        public int ReservaId { get; set; }
        public string? Mesaje { get; set; }
        public string? FechaEntrada { get; set; }
        public string? FechaSalida { get; set; }
        public string? Correo { get; set; }
        public string? Nombres { get; set; }
        public string? HotelNombre { get; set; }
        public string? TipoHabitacion { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal ImpuestoTotal { get; set; }
        public string? CorreoAgente { get; set; }
        public int CantidadHuespedes { get; set; }
    }
}
