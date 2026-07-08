namespace TuProyecto.Application.DTOs;

public record HotelDto(
    int Id,
    string Nombre,
    string Ciudad,
    string Direccion,
    string? Descripcion,
    bool EstaHabilitado
);