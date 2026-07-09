using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using API_Hotel.Domain.Entities;
using API_Hotel.Infrastructure.Data;
using Dapper;
using Domain.Repositories;


namespace TuProyecto.Infrastructure.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly DapperContext _context;

    public ReservaRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<bool> ExisteSolapamientoAsync(int habitacionId, string fechaEntrada, string fechaSalida)
    {
        using var connection = _context.CreateConnection();
        var parametros = new DynamicParameters();
        parametros.Add("@Opcion", "VerificarSolapamiento");
        parametros.Add("@HabitacionId", habitacionId);
        parametros.Add("@FechaEntrada", fechaEntrada);
        parametros.Add("@FechaSalida", fechaSalida);

        var conteo = await connection.ExecuteScalarAsync<int>("HotelSP_GestionReservas", parametros, commandType: CommandType.StoredProcedure);
        return conteo > 0;
    }

    public async Task<IEnumerable<HabitacionDisponible>> BuscarHabitacionesDisponiblesAsync(string? ciudad, string fechaEntrada, string fechaSalida)
    {
        using var connection = _context.CreateConnection();
        var parametros = new DynamicParameters();
        parametros.Add("@Opcion", "BuscarHabitacionesDisponibles");
        parametros.Add("@Ciudad", ciudad);
        parametros.Add("@FechaEntrada", fechaEntrada);
        parametros.Add("@FechaSalida", fechaSalida);

        return await connection.QueryAsync<HabitacionDisponible>("HotelSP_GestionReservas", parametros, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CrearReservaConHuespedesAsync(Reserva reserva)
    {
        using var connection = _context.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var paramReserva = new DynamicParameters();
            paramReserva.Add("@Opcion", "InsertarReserva");
            paramReserva.Add("@HotelId", reserva.HotelId);
            paramReserva.Add("@HabitacionId", reserva.HabitacionId);
            paramReserva.Add("@FechaEntrada", reserva.FechaEntrada);
            paramReserva.Add("@FechaSalida", reserva.FechaSalida);
            paramReserva.Add("@CantidadHuespedes", reserva.CantidadHuespedes);
            paramReserva.Add("@CostoTotal", reserva.CostoTotal);
            paramReserva.Add("@ImpuestoTotal", reserva.ImpuestoTotal);
            paramReserva.Add("@Estado", reserva.Estado);
            paramReserva.Add("@ContactoEmergenciaNombre", reserva.ContactoEmergenciaNombre);
            paramReserva.Add("@ContactoEmergenciaTelefono", reserva.ContactoEmergenciaTelefono);

            int reservaId = await connection.ExecuteScalarAsync<int>("HotelSP_GestionReservas", paramReserva, transaction, commandType: CommandType.StoredProcedure);

            foreach (var huesped in reserva.Huespedes)
            {
                var paramHuesped = new DynamicParameters();
                paramHuesped.Add("@Opcion", "InsertarHuesped");
                paramHuesped.Add("@ReservaId", reservaId);
                paramHuesped.Add("@Nombres", huesped.Nombres);
                paramHuesped.Add("@Apellidos", huesped.Apellidos);
                paramHuesped.Add("@FechaNacimiento", huesped.FechaNacimiento);
                paramHuesped.Add("@Genero", huesped.Genero);
                paramHuesped.Add("@TipoDocumento", huesped.TipoDocumento);
                paramHuesped.Add("@NumeroDocumento", huesped.NumeroDocumento);
                paramHuesped.Add("@Correo", huesped.Correo);
                paramHuesped.Add("@Telefono", huesped.Telefono);

                await connection.ExecuteScalarAsync<int>("HotelSP_GestionReservas", paramHuesped, transaction, commandType: CommandType.StoredProcedure);
            }

            transaction.Commit();
            return reservaId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<IEnumerable<ReservaDetalle>> ObtenerReservasConDetallesAsync()
    {
        using var connection = _context.CreateConnection();
        var parametros = new DynamicParameters();
        parametros.Add("@Opcion", "ObtenerTodasLasReservasDetalle");

        string jsonResult = await connection.ExecuteScalarAsync<string>("dbo.HotelSP_GestionReservas", parametros, commandType: CommandType.StoredProcedure);

        if (string.IsNullOrEmpty(jsonResult))
            return new List<ReservaDetalle>();

        var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<List<ReservaDetalle>>(jsonResult, opciones)
               ?? new List<ReservaDetalle>();
    }
}