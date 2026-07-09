using API_Hotel.Domain.Repositories;
using MediatR;
namespace Application.Features.Hoteles.Commands; 

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