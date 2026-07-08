using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Hotel.Application.DTOs
{
    public record CrearHotelCommand(
     string Nombre,
     string Ciudad,
     string Direccion,
     string? Descripcion
    ) : IRequest<int>;
}
