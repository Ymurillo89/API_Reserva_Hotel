using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API_Hotel.Infrastructure.Data;

public class DatabaseInitializer
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IConfiguration configuration, ILogger<DatabaseInitializer> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        var builder = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = "master" // Nos conectamos a master primero para crear la DB
        };
        var masterConnectionString = builder.ConnectionString;

        // Intentamos 10 veces (útil para Docker, ya que el SQL tarda unos segundos en prender)
        int retries = 10;
        while (retries > 0)
        {
            try
            {
                // 1. CREAR LA BASE DE DATOS
                using (var masterConn = new SqlConnection(masterConnectionString))
                {
                    await masterConn.OpenAsync();
                    var createDbSql = @"
                    IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'API_HotelDB')
                    BEGIN
                        CREATE DATABASE API_HotelDB;
                    END";
                                        await masterConn.ExecuteAsync(createDbSql);
                                    }

                                    // 2. CREAR LAS TABLAS
                                    using var connection = new SqlConnection(connectionString);
                                    await connection.OpenAsync();

                                    var schemaSql = @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Hot_tblHoteles' AND xtype='U')
                    BEGIN
                        CREATE TABLE Hot_tblHoteles (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Nombre NVARCHAR(150) NOT NULL,
                            Ciudad NVARCHAR(100) NOT NULL,
                            Direccion NVARCHAR(250) NOT NULL,
                            Descripcion NVARCHAR(500) NULL,
                            EstaHabilitado BIT NOT NULL DEFAULT 1,
                            EstaEliminado BIT NOT NULL DEFAULT 0,
                            FechaCreacion DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Hot_tblHabitaciones' AND xtype='U')
                    BEGIN
                        CREATE TABLE Hot_tblHabitaciones (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            HotelId INT NOT NULL,
                            TipoHabitacion NVARCHAR(100) NOT NULL,
                            CostoBase DECIMAL(18,2) NOT NULL,
                            Impuesto DECIMAL(18,2) NOT NULL,
                            Ubicacion NVARCHAR(150) NULL,
                            EstaHabilitada BIT NOT NULL DEFAULT 1,
                            CONSTRAINT FK_Habitaciones_Hoteles FOREIGN KEY (HotelId) REFERENCES Hot_tblHoteles(Id)
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Hot_tblReservas' AND xtype='U')
                    BEGIN
                        CREATE TABLE Hot_tblReservas (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            HotelId INT NOT NULL,
                            HabitacionId INT NOT NULL,
                            FechaEntrada DATETIME2 NOT NULL,
                            FechaSalida DATETIME2 NOT NULL,
                            CantidadHuespedes INT NOT NULL,
                            CostoTotal DECIMAL(18,2) NOT NULL DEFAULT 0,
                            ImpuestoTotal DECIMAL(18,2) NOT NULL DEFAULT 0,
                            ContactoEmergenciaNombre NVARCHAR(150) NOT NULL,
                            ContactoEmergenciaTelefono NVARCHAR(50) NOT NULL,
                            Estado NVARCHAR(100) NOT NULL DEFAULT 'Pendiente',
                            FechaCreacion DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                            CONSTRAINT FK_Reservas_Hoteles FOREIGN KEY (HotelId) REFERENCES Hot_tblHoteles(Id),
                            CONSTRAINT FK_Reservas_Habitaciones FOREIGN KEY (HabitacionId) REFERENCES Hot_tblHabitaciones(Id)
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Hot_tblHuespedes' AND xtype='U')
                    BEGIN
                        CREATE TABLE Hot_tblHuespedes (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            ReservaId INT NOT NULL,
                            Nombres NVARCHAR(100) NOT NULL,
                            Apellidos NVARCHAR(100) NOT NULL,
                            FechaNacimiento DATETIME2 NULL,
                            Genero NVARCHAR(20) NULL,
                            TipoDocumento NVARCHAR(20) NULL,
                            NumeroDocumento NVARCHAR(50) NULL,
                            Correo NVARCHAR(150) NOT NULL,
                            Telefono NVARCHAR(50) NULL,
                            CONSTRAINT FK_Huespedes_Reservas FOREIGN KEY (ReservaId) REFERENCES Hot_tblReservas(Id) ON DELETE CASCADE
                        );
                    END

                    -- ================= DATOS SEMILLA (SEED DATA) =================
                    IF NOT EXISTS (SELECT 1 FROM Hot_tblHoteles)
                    BEGIN
                        INSERT INTO Hot_tblHoteles (Nombre, Ciudad, Direccion, Descripcion, EstaHabilitado, EstaEliminado, FechaCreacion)
                        VALUES 
                        ('Hotel Bogota Plaza', 'Bogota', 'Calle 100 # 18A-30', 'Hotel ejecutivo de lujo', 1, 0, GETUTCDATE()),
                        ('Cartagena Beach Resort', 'Cartagena', 'Bocagrande, Carrera 1', 'Resort frente al mar con todo incluido', 1, 0, GETUTCDATE());
                        
                        -- Habitaciones para el Hotel 1 (Bogotá)
                        INSERT INTO Hot_tblHabitaciones (HotelId, TipoHabitacion, CostoBase, Impuesto, Ubicacion, EstaHabilitada)
                        VALUES 
                        (1, 'Sencilla', 150000, 28500, 'Piso 2 - Vista interior', 1),
                        (1, 'Doble', 250000, 47500, 'Piso 3 - Vista a la calle', 1);

                        -- Habitaciones para el Hotel 2 (Cartagena)
                        INSERT INTO Hot_tblHabitaciones (HotelId, TipoHabitacion, CostoBase, Impuesto, Ubicacion, EstaHabilitada)
                        VALUES 
                        (2, 'Suite Presidencial', 850000, 161500, 'Penthouse - Vista al mar', 1);
                    END
                    ";


                await connection.ExecuteAsync(schemaSql);

                var scriptSpHoteles = @"
                    CREATE OR ALTER PROCEDURE [dbo].[HotelSP_GestionHoteles]
                    @Opcion NVARCHAR(50),
                    @Id INT = NULL,
                    @HotelId INT = NULL,
                    @Nombre NVARCHAR(150) = NULL,
                    @Ciudad NVARCHAR(100) = NULL,
                    @Direccion NVARCHAR(250) = NULL,
                    @Descripcion NVARCHAR(500) = NULL,
                    @EstaHabilitado BIT = NULL,
                    @TipoHabitacion NVARCHAR(50) = NULL,
                    @CostoBase DECIMAL(18,2) = NULL,
                    @Impuesto DECIMAL(18,2) = NULL,
                    @Ubicacion NVARCHAR(100) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- ================= HOTELES =================
                    IF @Opcion = 'ObtenerTodosActivos'
		                BEGIN
			                SELECT * FROM Hot_tblHoteles WHERE EstaEliminado = 0;
		                END

                    IF @Opcion = 'ObtenerPorId'
		                BEGIN
			                SELECT * FROM Hot_tblHoteles WHERE Id = @Id AND EstaEliminado = 0;
		                END

                    IF @Opcion = 'InsertarHotel'
		                BEGIN
			                INSERT INTO Hot_tblHoteles (Nombre, Ciudad, Direccion, Descripcion, EstaHabilitado, EstaEliminado, FechaCreacion)
			                VALUES (@Nombre, @Ciudad, @Direccion, @Descripcion, ISNULL(@EstaHabilitado, 1), 0, GETUTCDATE());
			                SELECT CAST(SCOPE_IDENTITY() AS INT) AS NuevoId;
		                END

                    IF @Opcion = 'ActualizarHotel'
		                BEGIN
			                UPDATE Hot_tblHoteles
			                SET Nombre = ISNULL(@Nombre, Nombre),
				                Ciudad = ISNULL(@Ciudad, Ciudad),
				                Direccion = ISNULL(@Direccion, Direccion),
				                Descripcion = ISNULL(@Descripcion, Descripcion)
			                WHERE Id = @Id AND EstaEliminado = 0;
		                END

                    IF @Opcion = 'CambiarEstadoHotel'
		                BEGIN
			                UPDATE Hot_tblHoteles SET EstaHabilitado = @EstaHabilitado WHERE Id = @Id AND EstaEliminado = 0;
		                END

                    IF @Opcion = 'EliminarHotel'
		                BEGIN
			                UPDATE Hot_tblHoteles SET EstaEliminado = 1 WHERE Id = @Id;
		                END

                    -- ================= HABITACIONES =================
                    IF @Opcion = 'ObtenerHabitacionesPorHotelId'
		                BEGIN
			                SELECT * FROM Hot_tblHabitaciones WHERE HotelId = @HotelId;
		                END

                    IF @Opcion = 'ObtenerHabitacionPorId'
		                BEGIN
			                SELECT * FROM Hot_tblHabitaciones WHERE Id = @Id;
		                END

                    IF @Opcion = 'InsertarHabitacion'
		                BEGIN
			                INSERT INTO Hot_tblHabitaciones (HotelId, TipoHabitacion, CostoBase, Impuesto, Ubicacion, EstaHabilitada)
			                VALUES (@HotelId, @TipoHabitacion, @CostoBase, @Impuesto, @Ubicacion, ISNULL(@EstaHabilitado, 1));
			                SELECT CAST(SCOPE_IDENTITY() AS INT) AS NuevoId;
		                END

                    IF @Opcion = 'ActualizarHabitacion'
		                BEGIN
			                UPDATE Hot_tblHabitaciones
			                SET TipoHabitacion = ISNULL(@TipoHabitacion, TipoHabitacion),
				                CostoBase = ISNULL(@CostoBase, CostoBase),
				                Impuesto = ISNULL(@Impuesto, Impuesto),
				                Ubicacion = ISNULL(@Ubicacion, Ubicacion),
				                EstaHabilitada = ISNULL(@EstaHabilitado, EstaHabilitada)
			                WHERE Id = @Id AND HotelId = @HotelId;
		                END
                END
                ";
                                await connection.ExecuteAsync(scriptSpHoteles);

                var scriptSpReservas = @"
                    CREATE OR ALTER   PROCEDURE [dbo].[HotelSP_GestionReservas]
                        @Opcion VARCHAR(50) = '',
                        @ReservaId INT = NULL,
                        @HotelId INT = NULL,
                        @HabitacionId INT = NULL,
                        @Ciudad NVARCHAR(100) = NULL,
                        @FechaEntrada NVARCHAR(100) = NULL,
                        @FechaSalida NVARCHAR(100) = NULL,
                        @CantidadHuespedes INT = NULL,
                        @CostoTotal DECIMAL(18,2) = NULL,
                        @ImpuestoTotal DECIMAL(18,2) = NULL,
                        @Estado NVARCHAR(100) = NULL,
                        @ContactoEmergenciaNombre NVARCHAR(150) = NULL,
                        @ContactoEmergenciaTelefono NVARCHAR(50) = NULL,
                        @Nombres NVARCHAR(100) = NULL,
                        @Apellidos NVARCHAR(100) = NULL,
                        @FechaNacimiento DATETIME2 = NULL,
                        @Genero NVARCHAR(20) = NULL,
                        @TipoDocumento NVARCHAR(20) = NULL,
                        @NumeroDocumento NVARCHAR(50) = NULL,
                        @Correo NVARCHAR(150) = NULL,
                        @Telefono NVARCHAR(50) = NULL
                    AS
                    BEGIN
                        SET NOCOUNT ON;

                        -- 1. Buscar Habitaciones Disponibles
                        IF (@Opcion = 'BuscarHabitacionesDisponibles')
		                    BEGIN
			                    SELECT r.Id, r.HotelId, h.Nombre AS NombreHotel, h.Ciudad, r.TipoHabitacion, r.CostoBase, r.Impuesto, r.Ubicacion
			                    FROM Hot_tblHabitaciones r WITH (NOLOCK)
			                    INNER JOIN Hot_tblHoteles h WITH (NOLOCK) ON h.Id = r.HotelId
			                    WHERE (@Ciudad IS NULL OR h.Ciudad = @Ciudad)
			                      AND h.EstaHabilitado = 1
			                      AND h.EstaEliminado = 0
			                      AND r.EstaHabilitada = 1
			                      AND NOT EXISTS (
				                      SELECT 1 FROM Hot_tblReservas b WITH (NOLOCK)
				                      WHERE b.HabitacionId = r.Id
			                      AND b.Estado = 'Confirmada'
					                    AND b.FechaEntrada < @FechaSalida
					                    AND b.FechaSalida > @FechaEntrada
			                      );
		                    END

                        IF (@Opcion = 'VerificarSolapamiento')
		                    BEGIN
			                    SELECT COUNT(1)
			                    FROM Hot_tblReservas WITH (NOLOCK)
			                    WHERE HabitacionId = @HabitacionId
			                      AND Estado  = 'Confirmada'
			                      AND FechaEntrada < @FechaSalida
			                      AND FechaSalida > @FechaEntrada;
		                    END

                        IF (@Opcion = 'InsertarReserva')
                        BEGIN
                            INSERT INTO Hot_tblReservas (HotelId, HabitacionId, FechaEntrada, FechaSalida, CantidadHuespedes, CostoTotal, ImpuestoTotal, Estado, ContactoEmergenciaNombre, ContactoEmergenciaTelefono, FechaCreacion)
                            VALUES (@HotelId, @HabitacionId, @FechaEntrada, @FechaSalida, @CantidadHuespedes, @CostoTotal, @ImpuestoTotal, ISNULL(@Estado, 'Pendiente'), @ContactoEmergenciaNombre, @ContactoEmergenciaTelefono, GETUTCDATE());
        
                            SELECT CAST(SCOPE_IDENTITY() AS INT);
                        END

                        IF (@Opcion = 'InsertarHuesped')
		                    BEGIN
			                    INSERT INTO Hot_tblHuespedes (ReservaId, Nombres, Apellidos, FechaNacimiento, Genero, TipoDocumento, NumeroDocumento, Correo, Telefono)
			                    VALUES (@ReservaId, @Nombres, @Apellidos, @FechaNacimiento, @Genero, @TipoDocumento, @NumeroDocumento, @Correo, @Telefono);
        
			                    SELECT CAST(SCOPE_IDENTITY() AS INT);
		                    END

                        IF (@Opcion = 'ObtenerTodasLasReservasDetalle')
                        BEGIN
                            SELECT (
                                SELECT 
                                    r.Id AS ReservaId,
                                    h.Nombre AS NombreHotel,
                                    hab.TipoHabitacion,
                                    r.FechaEntrada,
                                    r.FechaSalida,
                                    r.CostoTotal,
                                    r.ImpuestoTotal,
                                    r.Estado,
                                    r.ContactoEmergenciaNombre,
                                    r.ContactoEmergenciaTelefono,
                                    (
                                        SELECT hu.Nombres, hu.Apellidos, hu.NumeroDocumento AS NumeroDocumento
                                        FROM Hot_tblHuespedes hu
                                        WHERE hu.ReservaId = r.Id
                                        FOR JSON PATH
                                    ) AS Huespedes
                                FROM Hot_tblReservas r
                                INNER JOIN Hot_tblHoteles h ON h.Id = r.HotelId
                                INNER JOIN Hot_tblHabitaciones hab ON hab.Id = r.HabitacionId
                                FOR JSON PATH
                            ) AS JsonData;
                        END
                    END

                ";
                                await connection.ExecuteAsync(scriptSpReservas);
                // IMPORTANTE: Aquí puedes concatenar y ejecutar tus Scripts de los SPs (HotelSP_GestionHoteles y HotelSP_GestionReservas)
                // de la misma manera usando connection.ExecuteAsync(tuScriptSP);

                _logger.LogInformation("Base de datos inicializada correctamente por Dapper.");
                break;
            }
            catch (Exception ex)
            {
                retries--;
                if (retries == 0)
                {
                    _logger.LogWarning(ex, "No se pudo inicializar SQL Server. Verifica que el contenedor esté corriendo.");
                }
                else
                {
                    await Task.Delay(2000); // Esperamos 2 segundos antes de reintentar
                }
            }
        }
    }
}