using API_Hotel.Domain.Entities;
using API_Hotel.Domain.Repositories;
using MediatR;


namespace Application.Features.Hoteles.Commands;

public class CrearHotelCommandHandler : IRequestHandler<CrearHotelCommand, int>
{
    private readonly IHotelRepository _hotelRepository;

    public CrearHotelCommandHandler(IHotelRepository hotelRepository)
    {
        _hotelRepository = hotelRepository;
    }

    public async Task<int> Handle(CrearHotelCommand request, CancellationToken cancellationToken)
    {
       
        var nuevoHotel = new Hotel
        {
            Nombre = request.Nombre,
            Ciudad = request.Ciudad,
            Direccion = request.Direccion,
            Descripcion = request.Descripcion,
            EstaHabilitado = true
        };
        
        var nuevoId = await _hotelRepository.InsertarHotelAsync(nuevoHotel);

        return nuevoId;
    }
}

public record CrearHotelCommand(
     string Nombre,
     string Ciudad,
     string Direccion,
     string? Descripcion
    ) : IRequest<int>;