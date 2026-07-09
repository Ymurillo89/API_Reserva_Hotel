using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API_Hotel.Application.DTOs;
using Domain.Repositories;
using MediatR;

namespace TuProyecto.Application.Features.Reservas.Queries;


public class ObtenerReservasDetalleQueryHandler : IRequestHandler<ObtenerReservasDetalleQuery, IEnumerable<ReservaDetalleDto>>
{
    private readonly IReservaRepository _repository;

    public ObtenerReservasDetalleQueryHandler(IReservaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ReservaDetalleDto>> Handle(ObtenerReservasDetalleQuery request, CancellationToken cancellationToken)
    {
        var reservas = await _repository.ObtenerReservasConDetallesAsync();

        return reservas.Select(r => new ReservaDetalleDto(
            r.Id, r.NombreHotel, r.Ciudad, r.TipoHabitacion,
            r.FechaEntrada,
            r.FechaSalida,
            r.CantidadHuespedes, r.CostoTotal, r.ImpuestoTotal, r.Estado,
            r.ContactoEmergenciaNombre, r.ContactoEmergenciaTelefono,
            r.Huespedes.Select(h => new HuespedDto(
                h.Nombres, h.Apellidos, h.FechaNacimiento, h.Genero,
                h.TipoDocumento, h.NumeroDocumento, h.Correo, h.Telefono
            )).ToList()
        ));
    }
}
public record ObtenerReservasDetalleQuery() : IRequest<IEnumerable<ReservaDetalleDto>>;