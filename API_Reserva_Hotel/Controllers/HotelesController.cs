using API_Hotel.Application.DTOs;
using API_Hotel.Application.Features.Hoteles.Queries;
using MediatR;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using TuProyecto.Application.DTOs;


namespace TuProyecto.API.Controllers; // <-- Ajusta a tu namespace

[ApiController]
[Route("api/[controller]")]
public class HotelesController : ControllerBase
{
    private readonly ISender _mediator;

    public HotelesController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// GET: /api/hoteles
    /// Obtiene la lista de todos los hoteles activos en la base de datos.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HotelDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerHotelesActivos()
    {
        var query = new ObtenerHotelesActivosQuery();
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// POST: /api/hoteles
    /// Crea un nuevo hotel en la base de datos.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<IActionResult> CrearHotel([FromBody] CrearHotelCommand command)
    {
        var nuevoId = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObtenerHotelesActivos), new { id = nuevoId }, nuevoId);
    }
}