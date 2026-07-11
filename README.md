# Sistema de Gestión y Reserva de Hoteles

API backend escalable para gestión de hoteles, habitaciones y reservas con notificaciones por correo en tiempo real.

**Estado**: ✅ 100% requisitos funcionales implementados

---

## 📋 Requisitos Cumplidos

### ✅ Funcionalidades Obligatorias

**Historia 1 - Administración de Hoteles**
- CRUD de hoteles (crear, editar, eliminar lógicamente)
- Gestión de habitaciones
- Habilitar/deshabilitar independientemente
- Listar reservas con detalles

**Historia 2 - Reserva de Habitaciones**
- Búsqueda por ciudad, fechas y cantidad huéspedes
- Creación de reserva con datos completos
- Registro de contacto de emergencia
- Notificaciones por correo (huésped + agente)

---

## 🛠️ Stack Técnico

| Componente | Tecnología |
|-----------|-----------|
| Lenguaje | C# (.NET 8.0 LTS) |
| Framework | ASP.NET Core |
| Arquitectura | CQRS + MediatR |
| ORM | Dapper |
| Base de Datos | SQL Server 2022 |
| Mensajería | RabbitMQ |
| Email | MailKit + SMTP |
| API | REST + Swagger |
| Tests | xUnit + Moq |
| Contenedores | Docker Compose |

---

## 🚀 Quick Start

### Con Docker (Recomendado)

```bash
git clone <repo>
cd API_Reserva_Hotel
docker-compose up
```

- API: http://localhost:8080
- Swagger: http://localhost:8080/swagger
- RabbitMQ: http://localhost:15672 (guest/guest)

### Local

```bash
dotnet restore
dotnet build
dotnet run --project API_Reserva_Hotel
dotnet run --project API_Hotel.Notifications
```

**Requiere:**
- SQL Server 2022
- RabbitMQ
- .NET 8.0 SDK

---

## 📚 Documentación

| Documento | Contenido |
|-----------|-----------|
| [ARCHITECTURE.md](docs/ARCHITECTURE.md) | Decisiones de diseño, ADRs, diagramas C4 |
| [DEVELOPER_GUIDE.md](docs/DEVELOPER_GUIDE.md) | Cómo agregar nuevas funcionalidades |
| [SECURITY.md](docs/SECURITY.md) | Prácticas de seguridad implementadas |
| [API.md](docs/API.md) | Endpoints y ejemplos de uso |

---

## 🔐 Seguridad

- JWT Authentication con expiración configurable
- RBAC (Roles: Agente, Viajero)
- Validación de inputs + SQL parametrizado
- HTTPS ready
- Secrets en variables de entorno

Ver [SECURITY.md](docs/SECURITY.md) para detalles.

---

## 📊 Arquitectura

**CQRS Pattern**: Comandos (escritura) separados de Queries (lectura)  
**Event-Driven**: Notificaciones asincrónicas via RabbitMQ  
**Domain-Driven**: Bounded Contexts bien definidos

Ver [ARCHITECTURE.md](docs/ARCHITECTURE.md) para diagramas C4 y ADRs.

---

## 👨‍💻 Desarrollo

Para agregar nuevas funcionalidades, ver [DEVELOPER_GUIDE.md](docs/DEVELOPER_GUIDE.md) con:
- Cómo agregar Comandos
- Cómo agregar Queries
- Cómo agregar Eventos
- Cómo agregar Servicios
- Cómo agregar Stored Procedures

Checklist completo incluido.

---

## 📡 API Principal

| Método | Endpoint | Rol | Descripción |
|--------|----------|-----|-------------|
| GET | `/api/hoteles` | Agente | Listar hoteles |
| POST | `/api/hoteles` | Agente | Crear hotel |
| PUT | `/api/hoteles/{id}` | Agente | Actualizar hotel |
| DELETE | `/api/hoteles/{id}` | Agente | Eliminar hotel |
| GET | `/api/reservas/buscar` | Viajero, Agente | Buscar habitaciones disponibles |
| POST | `/api/reservas` | Viajero, Agente | Crear reserva |
| POST | `/api/auth/login` | - | Obtener JWT token |

Ver [API.md](docs/API.md) para documentación completa.

---

## 🔄 Flujos Principales

### Crear Reserva
1. POST `/api/reservas` con datos completos
2. Handler valida, crea reserva, publica evento
3. Worker recibe `ReservaCreadaEvent` de RabbitMQ
4. Envía 2 correos (huésped + agente)

### Buscar Disponibilidad
1. GET `/api/reservas/buscar?ciudad=X&fechaEntrada=YYYY-MM-DD&fechaSalida=YYYY-MM-DD&cantidadHuespedes=N`
2. Query ejecuta stored procedure optimizado
3. Retorna lista de habitaciones disponibles

---

## 🤖 Uso de IA en Desarrollo

**Herramienta**: Claude AI (Anthropic)

**Casos de uso**:
- Generación de estructura CQRS
- Implementación de SmtpEmailService
- Stored Procedures optimizados
- Docker & docker-compose.yml

**Validación**: Todos los componentes pasaron verificación de seguridad, performance y funcionalidad.

Ver [ARCHITECTURE.md](docs/ARCHITECTURE.md#uso-de-herramientas-de-ia) para detalles.

---

## 📋 Justificación .NET 8.0

Se eligió .NET 8.0 LTS sobre .NET 10 por:
1. Soporte hasta Nov 2026 (estabilidad)
2. Ecosistema maduro (Dapper, MediatR, MailKit)
3. 18+ meses en producción
4. Performance comparable, mayor predictibilidad

Ver [ARCHITECTURE.md](docs/ARCHITECTURE.md#adr-004-net-80-vs-net-10) para ADR completo.

---

## 📝 Configuración

### Variables de Entorno (docker-compose.yml)

```yaml
SQL Server:
  SA_PASSWORD: SuperSecurePassword123!

RabbitMQ:
  User: guest
  Pass: guest

SMTP (Gmail):
  Server: smtp.gmail.com
  Port: 587
  Username: tu_email@gmail.com
  Password: tu_contraseña_app
  UseTLS: true
```

### Credenciales por Defecto

| Servicio | Usuario | Contraseña |
|----------|---------|-----------|
| SQL Server | sa | SuperSecurePassword123! |
| RabbitMQ | guest | guest |

---

## 📦 Estructura de Carpetas

```
API_Reserva_Hotel/
├── API_Reserva_Hotel/          # API principal
├── API_Hotel.Domain/           # Entidades y agregados
├── API_Hotel.Application/      # CQRS, DTOs, Events
├── API_Hotel.Infrastructure/   # Persistencia, Messaging
├── API_Hotel.Notifications/    # Background Worker
├── API_Hotel.Tests/            # Tests unitarios
├── docs/                       # Documentación
│   ├── ARCHITECTURE.md
│   ├── DEVELOPER_GUIDE.md
│   ├── SECURITY.md
│   └── API.md
├── Dockerfile                  # Multi-stage
├── docker-compose.yml          # Orquestación
└── README.md                   # Este archivo
```

---

## 🧪 Testing

```bash
dotnet test
```

Tests implementados:
- CrearHabitacionCommandHandlerTests
- HotelesControllerTests

---

## 📞 Soporte

- Issues: GitHub Issues
- Documentación: Ver carpeta `docs/`
- Stack: CQRS + Event-Driven + Domain-Driven

---

**Versión**: 2.0  
**Última actualización**: 2025-07-10  
**Estado**: Producción Ready ✅
