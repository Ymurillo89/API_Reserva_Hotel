using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TuProyecto.Application.Features.Reservas.Commands;
using TuProyecto.Application.Features.Reservas.Queries;
using Microsoft.AspNetCore.Authorization;
using API_Hotel.Application.DTOs;

namespace TuProyecto.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Viajero,Agente")] 
public class ReservasController : ControllerBase
{
    private readonly ISender _mediator;

    public ReservasController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("buscar")]
    [ProducesResponseType(typeof(IEnumerable<HabitacionDisponibleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> BuscarHabitaciones([FromQuery] string? ciudad,[FromQuery] string fechaEntrada,[FromQuery] string fechaSalida)
    {
        try
        {
            var query = new BuscarHabitacionesDisponiblesQuery(ciudad, fechaEntrada, fechaSalida);
            var resultado = await _mediator.Send(query);
            return Ok(resultado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<IActionResult> CrearReserva([FromBody] CrearReservaCommand command)
    {
        try
        {
            var reservaId = await _mediator.Send(command);
            return CreatedAtAction(nameof(BuscarHabitaciones), new { id = reservaId }, reservaId);
        }
        catch (ArgumentException ex) { return BadRequest(new { Mensaje = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { Mensaje = ex.Message }); }
        catch (KeyNotFoundException ex) { return NotFound(new { Mensaje = ex.Message }); }
    }
}