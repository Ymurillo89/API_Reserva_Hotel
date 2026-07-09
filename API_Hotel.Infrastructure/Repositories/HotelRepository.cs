using API_Hotel.Domain.Entities;
using API_Hotel.Domain.Repositories;
using API_Hotel.Infrastructure.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Hotel.Infrastructure.Repositories
{
    internal class HotelRepository : IHotelRepository
    {
        private readonly DapperContext _context;

        public HotelRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hotel>> ObtenerTodosActivosAsync()
        {
            using var connection = _context.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("@Opcion", "ObtenerTodosActivos");
            return await connection.QueryAsync<Hotel>("HotelSP_GestionHoteles",parametros,commandType: CommandType.StoredProcedure);
        }

        public async Task<Hotel?> ObtenerPorIdAsync(int id)
        {
            using var connection = _context.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("@Opcion", "ObtenerPorId");
            parametros.Add("@Id", id);
            return await connection.QuerySingleOrDefaultAsync<Hotel>("HotelSP_GestionHoteles",parametros,commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertarHotelAsync(Hotel hotel)
        {
            using var connection = _context.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("@Opcion", "InsertarHotel");
            parametros.Add("@Nombre", hotel.Nombre);
            parametros.Add("@Ciudad", hotel.Ciudad);
            parametros.Add("@Direccion", hotel.Direccion);
            parametros.Add("@Descripcion", hotel.Descripcion);
            parametros.Add("@EstaHabilitado", hotel.EstaHabilitado);
            return await connection.ExecuteScalarAsync<int>("HotelSP_GestionHoteles",parametros,commandType: CommandType.StoredProcedure);
        }

        public async Task ActualizarHotelAsync(Hotel hotel)
        {
            using var connection = _context.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("@Opcion", "ActualizarHotel");
            parametros.Add("@Id", hotel.Id);
            parametros.Add("@Nombre", hotel.Nombre);
            parametros.Add("@Ciudad", hotel.Ciudad);
            parametros.Add("@Direccion", hotel.Direccion);
            parametros.Add("@Descripcion", hotel.Descripcion);
            parametros.Add("@EstaHabilitado", hotel.EstaHabilitado);
            await connection.ExecuteAsync("HotelSP_GestionHoteles",parametros,commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Habitacion>> ObtenerHabitacionesPorHotelIdAsync(int hotelId)
        {
            using var connection = _context.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("@Opcion", "ObtenerHabitacionesPorHotelId");
            parametros.Add("@HotelId", hotelId);
            return await connection.QueryAsync<Habitacion>("HotelSP_GestionHoteles",parametros,commandType: CommandType.StoredProcedure);

        }

        public async Task<int> InsertarHabitacionAsync(Habitacion habitacion)
        {
            using var connection = _context.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("@Opcion", "InsertarHabitacion");
            parametros.Add("@HotelId", habitacion.HotelId);
            parametros.Add("@TipoHabitacion", habitacion.TipoHabitacion);
            parametros.Add("@CostoBase", habitacion.CostoBase);
            parametros.Add("@Impuesto", habitacion.Impuesto);
            parametros.Add("@Ubicacion", habitacion.Ubicacion);
            parametros.Add("@EstaHabilitado", habitacion.EstaHabilitada);
            return await connection.ExecuteScalarAsync<int>("HotelSP_GestionHoteles",parametros,commandType: CommandType.StoredProcedure);
        }

        public async Task CambiarEstadoHotelAsync(int id, bool estaHabilitado)
        {
            using var connection = _context.CreateConnection();
            var parametros = new DynamicParameters();
            parametros.Add("@Opcion", "CambiarEstadoHotel");
            parametros.Add("@Id", id);
            parametros.Add("@EstaHabilitado", estaHabilitado);

            await connection.ExecuteAsync("HotelSP_GestionHoteles", parametros, commandType: CommandType.StoredProcedure);
        }

        public async Task EliminarHotelAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parametros = new DynamicParameters();
            parametros.Add("@Opcion", "EliminarHotel");
            parametros.Add("@Id", id);

            await connection.ExecuteAsync("HotelSP_GestionHoteles", parametros, commandType: CommandType.StoredProcedure);
        }

        public async Task<Habitacion?> ObtenerHabitacionPorIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parametros = new DynamicParameters();
            parametros.Add("@Opcion", "ObtenerHabitacionPorId");
            parametros.Add("@Id", id);

            return await connection.QuerySingleOrDefaultAsync<Habitacion>("HotelSP_GestionHoteles", parametros, commandType: CommandType.StoredProcedure);
        }

        public async Task ActualizarHabitacionAsync(Habitacion habitacion)
        {
            using var connection = _context.CreateConnection();
            var parametros = new DynamicParameters();
            parametros.Add("@Opcion", "ActualizarHabitacion");
            parametros.Add("@Id", habitacion.Id);
            parametros.Add("@HotelId", habitacion.HotelId);
            parametros.Add("@TipoHabitacion", habitacion.TipoHabitacion);
            parametros.Add("@CostoBase", habitacion.CostoBase);
            parametros.Add("@Impuesto", habitacion.Impuesto);
            parametros.Add("@Ubicacion", habitacion.Ubicacion);
            parametros.Add("@EstaHabilitado", habitacion.EstaHabilitada);

            await connection.ExecuteAsync("HotelSP_GestionHoteles", parametros, commandType: CommandType.StoredProcedure);
        }


    }
}
