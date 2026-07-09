using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using API_Hotel.Application.DTOs;
using API_Hotel.Application.Features.Hotel.Queries;
using API.Controllers;

namespace API_Hotel.Tests.Controllers
{
    public class HotelesControllerTests
    {
        [Fact]
        public async Task ObtenerHabitaciones_Debe_Retornar_Ok_Con_Lista_De_Habitaciones()
        {
            // ==========================================
            // 1. ARRANGE (Preparar el escenario)
            // ==========================================
            int hotelIdPrueba = 1;

            // Creamos una lista ficticia que supuestamente viene de la base de datos
            var listaSimulada = new List<HabitacionDto>
            {
                new HabitacionDto(1, hotelIdPrueba, "Sencilla", 100000, 19000, "Piso 1", true),
                new HabitacionDto(2, hotelIdPrueba, "Doble", 200000, 38000, "Piso 2", true)
            };

            // Hacemos un Mock de ISender (MediatR), ya que el controlador usa MediatR para todo
            var mockMediator = new Mock<ISender>();

            // Le enseñamos al Mediator falso qué responder
            mockMediator
                .Setup(m => m.Send(It.IsAny<ObtenerHabitacionesPorHotelQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(listaSimulada);

            // Instanciamos el Controlador inyectándole nuestro Mediator falso
            var controller = new HotelesController(mockMediator.Object);


            // ==========================================
            // 2. ACT (Ejecutar la petición)
            // ==========================================
            
            // Simulamos la llamada HTTP GET /api/hoteles/1/habitaciones
            var actionResult = await controller.ObtenerHabitaciones(hotelIdPrueba);


            // ==========================================
            // 3. ASSERT (Verificar la respuesta HTTP)
            // ==========================================
            
            // Verificamos que el controlador nos respondió con un 200 OK (OkObjectResult)
            var okResult = Assert.IsType<OkObjectResult>(actionResult);

            // Extraemos la lista del cuerpo (Body) del 200 OK
            var habitacionesRetornadas = Assert.IsAssignableFrom<IEnumerable<HabitacionDto>>(okResult.Value);

            // Verificamos que la lista contenga exactamente las 2 habitaciones que simulamos
            Assert.NotNull(habitacionesRetornadas);
            Assert.Equal(2, System.Linq.Enumerable.Count(habitacionesRetornadas));
            
            // Comprobamos que MediatR se llamó una vez para mandar el Query
            mockMediator.Verify(m => m.Send(It.IsAny<ObtenerHabitacionesPorHotelQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
