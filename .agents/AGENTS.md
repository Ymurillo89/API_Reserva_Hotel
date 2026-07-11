# Reglas del Workspace — API_Reserva_Hotel

**OBLIGATORIO PARA CUALQUIER AGENTE O IA:**
Antes de modificar, refactorizar o crear cualquier archivo de código o arquitectura en este proyecto, DEBES leer y obedecer estrictamente las instrucciones documentadas en:
- `docs/ai-rules.md`
- `docs/architecture.md`
- `docs/database.md`
- `docs/api-spec.md`

## Reglas Críticas Resumidas
1. **Stack**: .NET 8, **Dapper + Microsoft.Data.SqlClient** con **SQL Server** (NO usar Entity Framework para consultas de datos).
2. **CQRS**: Separar Commands y Queries utilizando MediatR.
3. **Transacciones**: Las operaciones de base de datos se hacen a través de Procedimientos Almacenados orquestados con el parámetro `@Option`.
4. **Mensajería**: El sistema usa RabbitMQ para notificaciones asíncronas.
5. **Mantenimiento de Docs**: Si agregas endpoints, comandos o tablas, debes actualizar inmediatamente los archivos en la carpeta `docs/`.
