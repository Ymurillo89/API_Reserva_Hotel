using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Hotel.Domain.Entities
{
    public class Reserva
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public int HabitacionId { get; set; }
        public string? FechaEntrada { get; set; }
        public string? FechaSalida { get; set; }
        public int CantidadHuespedes { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal ImpuestoTotal { get; set; }
        public string Estado { get; set; } = "Confirmada";
        public string? ContactoEmergenciaNombre { get; set; } 
        public string? ContactoEmergenciaTelefono { get; set; } 
        public string? FechaCreacion { get; set; } 
        public List<Huesped>? Huespedes { get; set; } 
    }
}
