using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using API_Hotel.Application.Features.Hotel.Commands;
using API_Hotel.Domain.Entities;
using API_Hotel.Domain.Repositories;

namespace API_Hotel.Tests.Application
{
    public class CrearHabitacionCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Debe_Llamar_A_InsertarHabitacionAsync_Y_Retornar_El_Id()
        {
            // ==========================================
            // 1. ARRANGE (Preparar el escenario)
            // ==========================================
            
            // Creamos un comando de prueba con datos ficticios
            var comando = new CrearHabitacionCommand(
                HotelId: 1,
                TipoHabitacion: "Suite",
                CostoBase: 150000,
                Impuesto: 20000,
                Ubicacion: "Piso 5",
                EstaHabilitada: true
            );

            // Creamos un "Mock" (Simulación) de la base de datos (IHotelRepository).
            // ¡Esto es magia pura! Evita que toquemos el SQL Server.
            var mockRepository = new Mock<IHotelRepository>();

            // Le decimos al Mock: "Cuando alguien te llame a InsertarHabitacionAsync con CUALQUIER habitación, retorna el número 99"
            mockRepository
                .Setup(repo => repo.InsertarHabitacionAsync(It.IsAny<Habitacion>()))
                .ReturnsAsync(99);

            // Instanciamos el Handler pasándole nuestro repositorio falso
            var handler = new CrearHabitacionCommandHandler(mockRepository.Object);


            // ==========================================
            // 2. ACT (Ejecutar la acción)
            // ==========================================
            
            // Ejecutamos la función como si estuviéramos en la API
            var resultadoId = await handler.Handle(comando, CancellationToken.None);


            // ==========================================
            // 3. ASSERT (Verificar los resultados)
            // ==========================================
            
            // Comprobamos que el ID devuelto sea exactamente 99 (lo que programamos en el Mock)
            Assert.Equal(99, resultadoId);

            // Verificamos matemáticamente que el método InsertarHabitacionAsync se haya llamado EXACTAMENTE UNA VEZ
            mockRepository.Verify(repo => repo.InsertarHabitacionAsync(It.IsAny<Habitacion>()), Times.Once);
        }
    }
}
