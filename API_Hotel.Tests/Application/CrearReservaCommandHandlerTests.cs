using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Application.Features.Reservas.Commands;
using API_Hotel.Application.DTOs;
using API_Hotel.Domain.Entities;
using API_Hotel.Domain.Repositories;
using Domain.Repositories;
using API_Hotel.Application;

namespace API_Hotel.Tests.Application
{
    public class CrearReservaCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Debe_Crear_Reserva_Y_Calcular_Precio_Correctamente()
        {
            // ==========================================
            // 1. ARRANGE
            // ==========================================
            var comando = new CrearReservaCommand(
                HotelId: 1,
                HabitacionId: 1,
                FechaEntrada: "2026-10-10",
                FechaSalida: "2026-10-15", // 5 noches
                ContactoEmergenciaNombre: "Juan Perez",
                ContactoEmergenciaTelefono: "123456",
                Huespedes: new List<HuespedDto>()
            );

            var mockReservaRepo = new Mock<IReservaRepository>();
            var mockHotelRepo = new Mock<IHotelRepository>();
            var mockEventPublisher = new Mock<IEventPublisher>();

            // Simular que no hay solapamiento
            mockReservaRepo
                .Setup(repo => repo.ExisteSolapamientoAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            // Simular obtener la habitación con sus costos
            mockHotelRepo
                .Setup(repo => repo.ObtenerHabitacionPorIdAsync(1))
                .ReturnsAsync(new Habitacion { CostoBase = 100000, Impuesto = 19000, TipoHabitacion = "Basica" });

            mockHotelRepo
                .Setup(repo => repo.ObtenerPorIdAsync(1))
                .ReturnsAsync(new Hotel { Nombre = "Hotel Test" });

            // Simular creación de reserva retornando ID 50
            mockReservaRepo
                .Setup(repo => repo.CrearReservaConHuespedesAsync(It.IsAny<Reserva>()))
                .ReturnsAsync(50);

            var handler = new CrearReservaCommandHandler(
                mockReservaRepo.Object, 
                mockHotelRepo.Object, 
                mockEventPublisher.Object
            );

            // ==========================================
            // 2. ACT
            // ==========================================
            var resultadoId = await handler.Handle(comando, CancellationToken.None);

            // ==========================================
            // 3. ASSERT
            // ==========================================
            Assert.Equal(50, resultadoId);

            // Verificamos que se calculó el costo correctamente (5 noches * 100,000)
            mockReservaRepo.Verify(repo => repo.CrearReservaConHuespedesAsync(It.Is<Reserva>(r => 
                r.CostoTotal == 500000 && 
                r.ImpuestoTotal == 95000
            )), Times.Once);

            // Verificamos que se haya enviado el evento a RabbitMQ
            mockEventPublisher.Verify(pub => pub.Publish(It.IsAny<object>(), "reserva_creada_queue"), Times.Once);
        }

        [Fact]
        public async Task Handle_Debe_Lanzar_Excepcion_Si_Habitacion_Esta_Ocupada()
        {
            // ARRANGE
            var comando = new CrearReservaCommand(
                HotelId: 1, HabitacionId: 1, 
                FechaEntrada: "2026-10-10", FechaSalida: "2026-10-15", 
                ContactoEmergenciaNombre: "", ContactoEmergenciaTelefono: "", 
                Huespedes: new List<HuespedDto>()
            );

            var mockReservaRepo = new Mock<IReservaRepository>();
            var mockHotelRepo = new Mock<IHotelRepository>();
            var mockEventPublisher = new Mock<IEventPublisher>();

            // Simular que SÍ hay solapamiento
            mockReservaRepo
                .Setup(repo => repo.ExisteSolapamientoAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var handler = new CrearReservaCommandHandler(
                mockReservaRepo.Object, mockHotelRepo.Object, mockEventPublisher.Object
            );

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.Handle(comando, CancellationToken.None)
            );

            Assert.Equal("La habitación seleccionada ya está reservada en esas fechas.", exception.Message);
        }

        [Fact]
        public async Task Handle_Debe_Lanzar_Excepcion_Si_Fechas_Son_Incoherentes()
        {
            // ARRANGE: Fecha de salida anterior a la entrada
            var comando = new CrearReservaCommand(
                HotelId: 1, HabitacionId: 1, 
                FechaEntrada: "2026-10-15", FechaSalida: "2026-10-10", 
                ContactoEmergenciaNombre: "", ContactoEmergenciaTelefono: "", 
                Huespedes: new List<HuespedDto>()
            );

            var mockReservaRepo = new Mock<IReservaRepository>();
            var mockHotelRepo = new Mock<IHotelRepository>();
            var mockEventPublisher = new Mock<IEventPublisher>();

            var handler = new CrearReservaCommandHandler(
                mockReservaRepo.Object, mockHotelRepo.Object, mockEventPublisher.Object
            );

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => handler.Handle(comando, CancellationToken.None)
            );

            Assert.Equal("La fecha de salida debe ser posterior a la fecha de entrada.", exception.Message);
        }
    }
}
