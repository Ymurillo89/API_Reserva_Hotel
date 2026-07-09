using API_Hotel.Domain.Entities;
using API_Hotel.Domain.Repositories;
using MediatR;
using System.Text.Json.Serialization;

namespace Application.Features.Hoteles.Commands;

public class ActualizarHotelCommandHandler : IRequestHandler<ActualizarHotelCommand, bool>
{
    private readonly IHotelRepository _repository;
    public ActualizarHotelCommandHandler(IHotelRepository repository) 
    { 
        _repository = repository; 
    }

    public async Task<bool> Handle(ActualizarHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel = new Hotel
        {
            Id = request.Id,
            Nombre = request.Nombre,
            Ciudad = request.Ciudad,
            Direccion = request.Direccion,
            Descripcion = request.Descripcion
        };
        await _repository.ActualizarHotelAsync(hotel);
        return true;
    }
}

public record ActualizarHotelCommand(
    [property: JsonIgnore] int Id,
    string Nombre,
    string Ciudad,
    string Direccion,
    string? Descripcion
) : IRequest<bool>;