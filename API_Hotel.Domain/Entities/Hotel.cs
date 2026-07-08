using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Hotel.Domain.Entities
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Ciudad { get; set; } 
        public string Direccion { get; set; }
        public string? Descripcion { get; set; }
        public bool EstaHabilitado { get; set; }
        public bool EstaEliminado { get; set; } 
        public string FechaCreacion { get; set; } 
    }
}
