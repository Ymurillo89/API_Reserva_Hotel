# AI Rules - Sistema de Gestión y Reserva de Hoteles (API_Reserva_Hotel)

Reglas de arquitectura y desarrollo que deben seguirse estrictamente en este proyecto.

## Contexto del Proyecto
Backend profesional desarrollado para una prueba técnica nivel Senior / Lead en .NET 8.
Maneja dos Historias de Usuario principales:
- **HU1 (Administración de Hoteles)**: Gestión integral de Hoteles, Habitaciones, Activación/Desactivación y listado de Reservas con detalle de huéspedes para Agentes de Viaje.
- **HU2 (Reserva de Habitaciones)**: Búsqueda de disponibilidad por ciudad y fechas, reserva con registro detallado de huéspedes y contacto de emergencia para Viajeros.

## Stack y Tecnologías Obligatorias
- **Plataforma**: .NET 8 (C# 12)
- **Acceso a Datos**: Dapper + Microsoft.Data.SqlClient con SQL Server (NO Entity Framework)
- **Arquitectura**: Clean Architecture + DDD (Monolito Modular) + CQRS con MediatR
- **Notificaciones Asíncronas**: RabbitMQ
- **Contenedores**: Docker multi-stage y Docker Compose (API + SQL Server 2022 + RabbitMQ + Notifications Worker)

---

## 🏗️ Cómo Trabajar en Este Proyecto

### Agregar un Nuevo COMANDO (Escritura)

**Patrón**: Record Query/Command → Handler → Controller Endpoint

**Paso 1**: Crear en `Application/Features/[Feature]/Commands/[NombreCommand].cs`
```csharp
public record [NombreCommand](param1, param2) : IRequest<ReturnType>;
```

**Paso 2**: Handler en el mismo archivo
```csharp
public class [NombreCommand]Handler : IRequestHandler<[NombreCommand], ReturnType>
{
    public async Task<ReturnType> Handle([NombreCommand] request, CancellationToken ct)
    {
        // Validaciones
        // Operaciones
        // Publicar eventos si aplica
        return resultado;
    }
}
```

**Paso 3**: Endpoint en Controller
```csharp
[HttpPost/Put/Delete("endpoint")]
[Authorize(Roles = "...")]
public async Task<IActionResult> MethodName(params)
{
    var command = new [NombreCommand](...);
    var result = await _mediator.Send(command);
    return Ok(result);
}
```

**MediatR auto-registra** - No requiere configuración manual en Program.cs

---

### Agregar una Nueva QUERY (Lectura)

**Patrón**: Record Query → Handler → Controller GET

**Ubicación**: `Application/Features/[Feature]/Queries/[NombreQuery].cs`

```csharp
public record [NombreQuery](int Id) : IRequest<[DtoResult]>;

public class [NombreQuery]Handler : IRequestHandler<[NombreQuery], [DtoResult]>
{
    public async Task<[DtoResult]> Handle([NombreQuery] request, CancellationToken ct)
    {
        // SIEMPRE retorna DTO, nunca entidades
        return new [DtoResult](...);
    }
}
```

Endpoint:
```csharp
[HttpGet("{id}")]
[Authorize(Roles = "...")]
public async Task<IActionResult> GetById(int id)
{
    return Ok(await _mediator.Send(new [NombreQuery](id)));
}
```

---

### Agregar un EVENTO y NOTIFICACIÓN

**1. Crear evento**: `Application/Events/[NombreEvent].cs`
```csharp
public class [NombreEvent]
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public decimal Monto { get; set; }
}
```

**2. Publicar desde Command**:
```csharp
var evento = new [NombreEvent] { Id = ..., Email = ..., Monto = ... };
_eventPublisher.Publish(evento, "queue_name");
```

**3. Consumir en Worker** (`API_Hotel.Notifications/Worker.cs`):
```csharp
consumer.Received += async (model, ea) =>
{
    var evento = JsonSerializer.Deserialize<JsonElement>(message);
    var email = evento.GetProperty("Email").GetString();
    var monto = evento.GetProperty("Monto").GetDecimal();
    
    await _emailService.EnviarCorreoAsync(email, "Asunto", "Cuerpo");
};
```

---

### Agregar un Nuevo SERVICIO

**1. Interfaz**: `Application/Services/I[NombreService].cs`
```csharp
public interface I[NombreService]
{
    Task<ResultType> MetodoAsync(params);
}
```

**2. Implementación**: `Infrastructure/Services/[NombreService].cs`
```csharp
public class [NombreService] : I[NombreService]
{
    private readonly IConfiguration _config;
    private readonly ILogger<[NombreService]> _logger;

    public async Task<ResultType> MetodoAsync(params)
    {
        _logger.LogInformation("...");
        return resultado;
    }
}
```

**3. Registrar en Program.cs**:
```csharp
builder.Services.AddScoped<I[NombreService], [NombreService]>();
```

---

### Agregar un STORED PROCEDURE

**Ubicación**: `Infrastructure/Data/DatabaseInitializer.cs` en el método `InitializeAsync()`

```csharp
var scriptSp = @"
    CREATE OR ALTER PROCEDURE [dbo].[HotelSP_[NombreSP]]
        @Opcion VARCHAR(50) = '',
        @Param1 INT = NULL,
        @Param2 NVARCHAR(100) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        
        IF (@Opcion = 'Operacion1')
        BEGIN
            SELECT ... FROM ...
        END
    END
";

await connection.ExecuteAsync(scriptSp);
```

**Llamar desde Repository**:
```csharp
var parametros = new DynamicParameters();
parametros.Add("@Opcion", "Operacion1");
parametros.Add("@Param1", valor);

await connection.ExecuteAsync("HotelSP_[NombreSP]", parametros, 
    commandType: CommandType.StoredProcedure);
```

---

### Crear un DTO

**Ubicación**: `Application/DTOs/[NombreDto].cs`

```csharp
namespace API_Hotel.Application.DTOs
{
    public record [NombreDto](
        int Id,
        string Nombre,
        decimal Monto
    );
}
```

**Regla**: Record, nunca retornar entidades de BD directamente.

---

## 📋 Checklist por Funcionalidad

✅ **Comando**:
- [ ] Record del comando (IRequest<T>)
- [ ] Handler (IRequestHandler)
- [ ] Endpoint en Controller
- [ ] Eventos publicados si aplica

✅ **Query**:
- [ ] Record de query (IRequest<DtoResult>)
- [ ] Handler que retorna DTO
- [ ] Endpoint GET
- [ ] Autorización correcta

✅ **Evento**:
- [ ] Clase de evento creada
- [ ] Publicado desde command handler
- [ ] Consumido en Worker
- [ ] Notificación enviada (email)

✅ **Servicio**:
- [ ] Interfaz I[Nombre]
- [ ] Implementación
- [ ] Registrado en DI
- [ ] Inyectado donde se necesita

✅ **Stored Procedure**:
- [ ] Creado en DatabaseInitializer
- [ ] Llamado desde Repository
- [ ] Parámetros seguros (NO concatenación)

---

## 🚫 Prohibiciones Estrictas

❌ **NO usar Entity Framework** - Solo Dapper + SQL parametrizado  
❌ **NO retornar entidades** de BD desde handlers - Siempre DTOs  
❌ **NO comentarios innecesarios** - Código autoexplicativo  
❌ **NO SQL sin parámetros** - Siempre DynamicParameters  
❌ **NO publicar eventos sin consumidor** - Todo evento debe tener handler  
❌ **NO registrar servicios sin interfaz** - Siempre DI con abstracciones  
❌ **NO modificar docker-compose.yml** sin justificación  

---

## 📌 Estructura de Carpetas

Cuando agregues una funcionalidad nueva importante, sigue:

```
Application/Features/[Feature]/
├── Commands/
│   └── [NombreCommand].cs
├── Queries/
│   └── [NombreQuery].cs
└── Dtos/
    └── [NombreDto].cs

Application/Events/
└── [NombreEvent].cs

Infrastructure/Services/
└── [NombreService].cs
```

---

## 🔗 Referencias

- 📖 [architecture.md](./architecture.md) - Decisiones de diseño y ADRs
- 🗄️ [database.md](./database.md) - Schema y stored procedures
- 📡 [api-spec.md](./api-spec.md) - Endpoints documentados
- 📖 [README.md](../README.md) - Quick start y overview
