using API_Hotel.Application.DTOs;
using API_Hotel.Application.Features.Hotel.Commands;
using API_Hotel.Application.Features.Hotel.Queries;
using API_Hotel.Application.Features.Hoteles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using TuProyecto.Application.Features.Hoteles.Commands;
using TuProyecto.Application.Features.Reservas.Queries;

namespace TuProyecto.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Agente")]
public class HotelesController : ControllerBase
{
    private readonly ISender _mediator;

    public HotelesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HotelDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerHotelesActivos()
    {
        var query = new ObtenerHotelesActivosQuery();
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<IActionResult> CrearHotel([FromBody] CrearHotelCommand command)
    {
        var nuevoId = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerHotelesActivos), new { id = nuevoId }, nuevoId);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ActualizarHotel(int id, [FromBody] ActualizarHotelCommand command)
    {
        var comandoConId = command with { Id = id };
        await _mediator.Send(comandoConId);
        return Ok(new { Mensaje = "Hotel actualizado exitosamente." });
    }

    [HttpPatch("{id:int}/estado")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CambiarEstadoHotel(int id, [FromQuery] bool estaHabilitado)
    {
        var command = new CambiarEstadoHotelCommand(id, estaHabilitado);
        await _mediator.Send(command);
        return Ok(new { Mensaje = $"Estado del hotel cambiado a: {(estaHabilitado ? "Habilitado" : "Deshabilitado")}" });
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> EliminarHotel(int id)
    {
        var command = new EliminarHotelCommand(id);
        await _mediator.Send(command);
        return Ok(new { Mensaje = "Hotel eliminado exitosamente." });
    }
      
    [HttpGet("reservas")]
    [ProducesResponseType(typeof(IEnumerable<ReservaDetalleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodasLasReservas()
    {
        var query = new ObtenerReservasDetalleQuery();
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    [HttpGet("{hotelId:int}/habitaciones")]
    [ProducesResponseType(typeof(IEnumerable<HabitacionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerHabitaciones(int hotelId)
    {
        var query = new ObtenerHabitacionesPorHotelQuery(hotelId);
        var habitaciones = await _mediator.Send(query);
        return Ok(habitaciones);
    }

    [HttpPost("{hotelId:int}/habitaciones")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<IActionResult> CrearHabitacion(int hotelId, [FromBody] CrearHabitacionCommand command)
    {
        var commandConHotelId = command with { HotelId = hotelId };
        var habitacionId = await _mediator.Send(commandConHotelId);
        return CreatedAtAction(nameof(ObtenerHabitaciones), new { hotelId = hotelId }, habitacionId);
    }

    [HttpPut("habitaciones/{habitacionId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ActualizarHabitacion(int habitacionId, [FromBody] ActualizarHabitacionCommand command)
    {
        var commandConId = command with { HabitacionId = habitacionId };
        await _mediator.Send(commandConId);
        return Ok(new { Mensaje = "Habitación actualizada exitosamente." });
    }
}