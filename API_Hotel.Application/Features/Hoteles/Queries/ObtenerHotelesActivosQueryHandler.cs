using API_Hotel.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuProyecto.Application.DTOs;

namespace API_Hotel.Application.Features.Hoteles.Queries
{
    public record ObtenerHotelesActivosQuery : IRequest<IEnumerable<HotelDto>>;

    public class ObtenerHotelesActivosQueryHandler : IRequestHandler<ObtenerHotelesActivosQuery, IEnumerable<HotelDto>>
    {
        private readonly IHotelRepository _hotelRepository;
        public ObtenerHotelesActivosQueryHandler(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public async Task<IEnumerable<HotelDto>> Handle(ObtenerHotelesActivosQuery request, CancellationToken cancellationToken)
        {
            var hoteles = await _hotelRepository.ObtenerTodosActivosAsync();

            return hoteles.Select(h=> new HotelDto(
                h.Id,
                h.Nombre,
                h.Ciudad,
                h.Direccion,
                h.Descripcion,
                h.EstaHabilitado
            ));
        }
    }
}
