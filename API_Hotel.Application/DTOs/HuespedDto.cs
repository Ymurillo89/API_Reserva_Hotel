using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Hotel.Application.DTOs
{
    public record HuespedDto(
     string Nombres,
     string Apellidos,
     string FechaNacimiento,
     string Genero,
     string TipoDocumento,
     string NumeroDocumento,
     string Correo,
     string Telefono
 );
}
