using API_Hotel.Domain.Repositories;
using MediatR;
namespace TuProyecto.Application.Features.Hoteles.Commands; 


// 2. EL HANDLER: Ejecuta el cambio de estado directamente en 1 sola llamada a Dapper
public class CambiarEstadoHotelCommandHandler : IRequestHandler<CambiarEstadoHotelCommand, bool>
{
    private readonly IHotelRepository _repository;

    public CambiarEstadoHotelCommandHandler(IHotelRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(CambiarEstadoHotelCommand request, CancellationToken cancellationToken)
    {
        await _repository.CambiarEstadoHotelAsync(request.Id, request.EstaHabilitado);
        return true;
    }
}
public record CambiarEstadoHotelCommand(int Id, bool EstaHabilitado) : IRequest<bool>;