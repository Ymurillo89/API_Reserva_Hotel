using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Hotel.Domain.Entities
{
    public class Habitacion
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string? TipoHabitacion { get; set; } 
        public decimal CostoBase { get; set; }
        public decimal Impuesto { get; set; }
        public string? Ubicacion { get; set; } 
        public bool EstaHabilitada { get; set; } = true;
    }
}
