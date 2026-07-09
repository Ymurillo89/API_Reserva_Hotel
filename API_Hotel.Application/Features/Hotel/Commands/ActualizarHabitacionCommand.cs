using MediatR;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using API_Hotel.Domain.Entities;
using API_Hotel.Domain.Repositories;

namespace API_Hotel.Application.Features.Hotel.Commands;

public record ActualizarHabitacionCommand(
    [property: JsonIgnore] int HabitacionId, 
    int HotelId, 
    string TipoHabitacion, 
    decimal CostoBase, 
    decimal Impuesto, 
    string Ubicacion, 
    bool EstaHabilitada
) : IRequest<bool>;

public class ActualizarHabitacionCommandHandler : IRequestHandler<ActualizarHabitacionCommand, bool>
{
    private readonly IHotelRepository _repository;

    public ActualizarHabitacionCommandHandler(IHotelRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ActualizarHabitacionCommand request, CancellationToken cancellationToken)
    {
        var habitacion = new Habitacion
        {
            Id = request.HabitacionId,
            HotelId = request.HotelId,
            TipoHabitacion = request.TipoHabitacion,
            CostoBase = request.CostoBase,
            Impuesto = request.Impuesto,
            Ubicacion = request.Ubicacion,
            EstaHabilitada = request.EstaHabilitada
        };

        await _repository.ActualizarHabitacionAsync(habitacion);
        return true;
    }
}
