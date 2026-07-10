using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Hotel.Domain.Entities
{
    public class HabitacionDisponible
    {
        public int HabitacionId { get; set; }
        public int HotelId { get; set; }
        public string? NombreHotel { get; set; }
        public string? Ciudad { get; set; }
        public string? TipoHabitacion { get; set; }
        public decimal CostoBase { get; set; }
        public decimal Impuesto { get; set; }
        public string? Ubicacion { get; set; }
        public int Capacidad { get; set; }
    }
}
