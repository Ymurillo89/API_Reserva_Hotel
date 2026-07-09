using MediatR;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using API_Hotel.Domain.Entities;
using API_Hotel.Domain.Repositories;

namespace API_Hotel.Application.Features.Hotel.Commands;

public record CrearHabitacionCommand(
    [property: JsonIgnore] int HotelId, 
    string TipoHabitacion, 
    decimal CostoBase, 
    decimal Impuesto, 
    string Ubicacion, 
    bool EstaHabilitada
) : IRequest<int>;

public class CrearHabitacionCommandHandler : IRequestHandler<CrearHabitacionCommand, int>
{
    private readonly IHotelRepository _repository;

    public CrearHabitacionCommandHandler(IHotelRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CrearHabitacionCommand request, CancellationToken cancellationToken)
    {
        var habitacion = new Habitacion
        {
            HotelId = request.HotelId,
            TipoHabitacion = request.TipoHabitacion,
            CostoBase = request.CostoBase,
            Impuesto = request.Impuesto,
            Ubicacion = request.Ubicacion,
            EstaHabilitada = request.EstaHabilitada
        };

        return await _repository.InsertarHabitacionAsync(habitacion);
    }
}
