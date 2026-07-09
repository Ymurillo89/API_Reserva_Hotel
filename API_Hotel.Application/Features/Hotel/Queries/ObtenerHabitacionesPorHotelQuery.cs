using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API_Hotel.Application.DTOs;
using API_Hotel.Domain.Repositories;

namespace API_Hotel.Application.Features.Hotel.Queries;

public record ObtenerHabitacionesPorHotelQuery(int HotelId) : IRequest<IEnumerable<HabitacionDto>>;

public class ObtenerHabitacionesPorHotelQueryHandler : IRequestHandler<ObtenerHabitacionesPorHotelQuery, IEnumerable<HabitacionDto>>
{
    private readonly IHotelRepository _repository;

    public ObtenerHabitacionesPorHotelQueryHandler(IHotelRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<HabitacionDto>> Handle(ObtenerHabitacionesPorHotelQuery request, CancellationToken cancellationToken)
    {
        var habitaciones = await _repository.ObtenerHabitacionesPorHotelIdAsync(request.HotelId);

        return habitaciones.Select(h => new HabitacionDto(
            h.Id, h.HotelId, h.TipoHabitacion, h.CostoBase, h.Impuesto, h.Ubicacion, h.EstaHabilitada
        ));
    }
}
