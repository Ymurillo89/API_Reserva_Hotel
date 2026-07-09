using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API_Hotel.Application.DTOs;
using Domain.Repositories;
using MediatR;


namespace TuProyecto.Application.Features.Reservas.Queries;


public class BuscarHabitacionesDisponiblesQueryHandler : IRequestHandler<BuscarHabitacionesDisponiblesQuery, IEnumerable<HabitacionDisponibleDto>>
{
    private readonly IReservaRepository _reservaRepository;

    public BuscarHabitacionesDisponiblesQueryHandler(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
    }

    public async Task<IEnumerable<HabitacionDisponibleDto>> Handle(BuscarHabitacionesDisponiblesQuery request, CancellationToken cancellationToken)
    {
        if (!DateTime.TryParse(request.FechaEntrada, out DateTime entrada) ||
            !DateTime.TryParse(request.FechaSalida, out DateTime salida))
        {
            throw new ArgumentException("Las fechas no tienen un formato válido (usa YYYY-MM-DD).");
        }

        if (salida <= entrada)
        {
            throw new ArgumentException("La fecha de salida debe ser posterior a la de entrada.");
        }

        // 2. Enviamos las fechas verdaderas (DateTime) al Repositorio
        var disponibles = await _reservaRepository.BuscarHabitacionesDisponiblesAsync(request.Ciudad, request.FechaEntrada, request.FechaSalida);

        return disponibles.Select(h => new HabitacionDisponibleDto(
            h.HabitacionId, h.HotelId, h.NombreHotel, h.Ciudad, h.TipoHabitacion, h.CostoBase, h.Impuesto, h.Ubicacion
        ));
    }
}

public record BuscarHabitacionesDisponiblesQuery(
    string? Ciudad,
    string FechaEntrada,
    string FechaSalida
) : IRequest<IEnumerable<HabitacionDisponibleDto>>;
