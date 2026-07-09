using System;

namespace API_Hotel.Application.DTOs;

public record HabitacionDto(int Id, int HotelId, string TipoHabitacion, decimal CostoBase, decimal Impuesto, string Ubicacion, bool EstaHabilitada);
