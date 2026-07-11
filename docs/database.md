# Base de Datos SQL Server — API_Reserva_Hotel

El acceso a base de datos se realiza de forma directa y performante utilizando **Dapper** y consultas optimizadas sobre **SQL Server 2022**.

## Tablas Principales
- **`Hot_tblHoteles`**: Almacena información del hotel.
- **`Hot_tblHabitaciones`**: Almacena habitaciones asociadas al hotel.
- **`Hot_tblReservas`**: Almacena las reservas generadas por los usuarios.
- **`Hot_tblHuespedes`**: Almacena los huéspedes asociados a cada reserva.

## Procedimientos Almacenados (Orquestados por `@Option`)

Toda la lógica de acceso a datos está encapsulada en procedimientos almacenados mediante el patrón de opción (`@Option`), invocado desde los repositorios con Dapper.

### 1. `dbo.HotelSP_GestionHoteles`
Encargado de la lógica CRUD y consultas de hoteles y habitaciones:
| `@Option` | Descripción |
|---|---|
| `GetById` | Obtiene un hotel por ID. |
| `GetAllActive` | Lista todos los hoteles habilitados. |
| `GetRoomsByHotelId` | Obtiene habitaciones de un hotel. |
| `InsertHotel` / `UpdateHotel` | Crear / Actualizar hoteles. |
| `InsertRoom` / `UpdateRoom` | Crear / Actualizar habitaciones. |

### 2. `dbo.HotelSP_GestionReservas`
Manejo transaccional para buscar y concretar reservas:
| `@Option` | Descripción |
|---|---|
| `SearchAvailableRooms` | Filtra habitaciones libres (sin cruce de fechas) por ciudad y capacidad de huéspedes. |
| `CheckOverlapping` | Valida que la habitación no esté ya ocupada en las fechas solicitadas. |
| `InsertBooking` | Inserta el encabezado de reserva transaccionalmente. |
| `InsertGuest` | Inserta los huéspedes a la reserva. |
| `GetAllBookings` | Lista detallada de reservas para el agente. |
