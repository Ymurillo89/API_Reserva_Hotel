# Especificación de la API — API_Reserva_Hotel

La API expone múltiples endpoints orientados a cubrir las historias de usuario.

## Endpoints de Administración (HotelesController)
- `POST /api/Hoteles`: Crear hotel.
- `PUT /api/Hoteles/{id}`: Editar hotel y cambiar estado de habilitado/deshabilitado.
- `DELETE /api/Hoteles/{id}`: Eliminar hotel lógicamente o físicamente.
- `POST /api/Hoteles/{hotelId}/habitaciones`: Agregar habitación a un hotel.
- `PUT /api/Hoteles/habitaciones/{habitacionId}`: Editar habitación y su estado.
- `GET /api/Hoteles/reservas`: Consultar listado completo de todas las reservas de todos los hoteles, con detalle de huéspedes.
- `GET /api/Hoteles/activos`: Obtener la lista de hoteles activos.

## Endpoints de Cliente (ReservasController)
- `GET /api/Reservas/buscar`: Buscar habitaciones disponibles en una ciudad, en un rango de fechas, que tengan cierta capacidad para huéspedes.
- `POST /api/Reservas`: Crear reserva, asignando contacto de emergencia y el registro individual de cada huésped. Detona el evento a RabbitMQ.
