using API_Hotel.Application.Features.Hoteles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using TuProyecto.Application.Features.Hoteles.Commands;


namespace TuProyecto.API.Controllers; // <-- Ajusta a tu namespace

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
        // Aseguramos que el Id de la ruta coincida con el del body
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
}