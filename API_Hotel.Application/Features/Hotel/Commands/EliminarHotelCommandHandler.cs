using API_Hotel.Domain.Repositories;
using MediatR;

namespace Application.Features.Hoteles.Commands;

public class EliminarHotelCommandHandler : IRequestHandler<EliminarHotelCommand, bool>
{
    private readonly IHotelRepository _repository;

    public EliminarHotelCommandHandler(IHotelRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(EliminarHotelCommand request, CancellationToken cancellationToken)
    {
        await _repository.EliminarHotelAsync(request.Id);
        return true;
    }
}

public record EliminarHotelCommand(int Id) : IRequest<bool>;