using API_Hotel.Application;
using API_Hotel.Application.DTOs;
using API_Hotel.Application.Events;
using API_Hotel.Domain.Entities;
using API_Hotel.Domain.Repositories;
using Domain.Repositories;
using MediatR;


namespace Application.Features.Reservas.Commands;

public record CrearReservaCommand(
    int HotelId,
    int HabitacionId,
    string FechaEntrada,
    string FechaSalida,
    string ContactoEmergenciaNombre,
    string ContactoEmergenciaTelefono,
    List<HuespedDto> Huespedes
) : IRequest<int>;

public class CrearReservaCommandHandler : IRequestHandler<CrearReservaCommand, int>
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IEventPublisher _eventPublisher;

    public CrearReservaCommandHandler(IReservaRepository reservaRepository, IHotelRepository hotelRepository,IEventPublisher eventPublisher)
    {
        _reservaRepository = reservaRepository;
        _hotelRepository = hotelRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<int> Handle(CrearReservaCommand request, CancellationToken cancellationToken)
    {
        if (!DateTime.TryParse(request.FechaEntrada, out DateTime fechaEntrada))
        {
            throw new ArgumentException("La fecha de entrada no tiene un formato válido (usa YYYY-MM-DD).");
        }
        if (!DateTime.TryParse(request.FechaSalida, out DateTime fechaSalida))
        {
            throw new ArgumentException("La fecha de salida no tiene un formato válido (usa YYYY-MM-DD).");
        }

        if (fechaSalida <= fechaEntrada)
        {
            throw new ArgumentException("La fecha de salida debe ser posterior a la fecha de entrada.");
        }

        bool estaOcupada = await _reservaRepository.ExisteSolapamientoAsync(
            request.HabitacionId, request.FechaEntrada, request.FechaSalida);

        if (estaOcupada)
        {
            throw new InvalidOperationException("La habitación seleccionada ya está reservada en esas fechas.");
        }
                
        var habitacion = await _hotelRepository.ObtenerHabitacionPorIdAsync(request.HabitacionId)
             ?? throw new KeyNotFoundException("La habitación especificada no existe.");

        // Calcular noches reales
        int noches = (fechaSalida.Date - fechaEntrada.Date).Days;
        if (noches == 0) noches = 1;
        decimal costoTotal = habitacion.CostoBase * noches;
        decimal impuestoTotal = habitacion.Impuesto * noches;


        var reserva = new Reserva
        {
            HotelId = request.HotelId,
            HabitacionId = request.HabitacionId,
            FechaEntrada = request.FechaEntrada,
            FechaSalida = request.FechaSalida,
            CantidadHuespedes = request.Huespedes.Count,
            CostoTotal = costoTotal,
            ImpuestoTotal = impuestoTotal,
            Estado = "Confirmada",
            ContactoEmergenciaNombre = request.ContactoEmergenciaNombre,
            ContactoEmergenciaTelefono = request.ContactoEmergenciaTelefono,
            Huespedes = request.Huespedes.Select(h => new Huesped
            {
                Nombres = h.Nombres,
                Apellidos = h.Apellidos,
                FechaNacimiento = h.FechaNacimiento,
                Genero = h.Genero,
                TipoDocumento = h.TipoDocumento,
                NumeroDocumento = h.NumeroDocumento,
                Correo = h.Correo,
                Telefono = h.Telefono
            }).ToList()
        };

        int nuevaReservaId = await _reservaRepository.CrearReservaConHuespedesAsync(reserva);

        var reservaEvent = new ReservaCreadaEvent
        {
            ReservaId = nuevaReservaId,
            Mesaje = $"Se ha creado una nueva reserva con ID: {nuevaReservaId}",
            FechaEntrada = request.FechaEntrada,
            Correo = request.Huespedes.FirstOrDefault()?.Correo ?? "correo@prueba.com",
            Nombres = request.Huespedes.FirstOrDefault()?.Nombres ?? "Huésped Principal",
            HotelNombre = "Hotel Sistema"
        };

     
        _eventPublisher.Publish(reservaEvent, "reserva_creada_queue");
        return nuevaReservaId;
    }
}