using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using API_Hotel.Infrastructure.Messaging;

namespace API_Hotel.Notifications;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly string _hostname;
    private readonly IEmailService _emailService;
    private IConnection _connection;
    private IModel _channel;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
        _hostname = configuration["RabbitMQ:HostName"] ?? "localhost";

        InicializarRabbitMQ();
    }

    private void InicializarRabbitMQ()
    {
        var factory = new ConnectionFactory { HostName = _hostname };

        while (_connection == null)
        {
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: "reserva_creada_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                _logger.LogInformation("Conectado a RabbitMQ exitosamente.");
            }
            catch
            {
                _logger.LogWarning("Esperando a RabbitMQ...");
                Thread.Sleep(3000);
            }
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Escuchando mensajes de reservas nuevas...");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var reserva = JsonSerializer.Deserialize<JsonElement>(message);

                var correoHuesped = reserva.GetProperty("Correo").GetString();
                var nombres = reserva.GetProperty("Nombres").GetString();
                var hotel = reserva.GetProperty("HotelNombre").GetString();
                var fechaEntrada = reserva.GetProperty("FechaEntrada").GetString();
                var fechaSalida = reserva.GetProperty("FechaSalida").GetString();
                var tipoHabitacion = reserva.GetProperty("TipoHabitacion").GetString();
                var costoTotal = reserva.GetProperty("CostoTotal").GetDecimal();
                var impuestoTotal = reserva.GetProperty("ImpuestoTotal").GetDecimal();
                var cantidadHuespedes = reserva.GetProperty("CantidadHuespedes").GetInt32();
                var reservaId = reserva.GetProperty("ReservaId").GetInt32();

                var cuerpoHuesped = GenerarHtmlCorreoHuesped(nombres, hotel, fechaEntrada, fechaSalida, tipoHabitacion, costoTotal, impuestoTotal, cantidadHuespedes, reservaId);
                await _emailService.EnviarCorreoHtmlAsync(correoHuesped, $"Confirmación de Reserva - {hotel}", cuerpoHuesped);

                if (reserva.TryGetProperty("CorreoAgente", out var correoAgenteElement) && correoAgenteElement.ValueKind != JsonValueKind.Null)
                {
                    var correoAgente = correoAgenteElement.GetString();
                    var cuerpoAgente = GenerarHtmlCorreoAgente(nombres, hotel, fechaEntrada, fechaSalida, tipoHabitacion, cantidadHuespedes, costoTotal, impuestoTotal, reservaId);
                    await _emailService.EnviarCorreoHtmlAsync(correoAgente, $"Nueva Reserva Confirmada - {hotel}", cuerpoAgente);
                }

                _logger.LogInformation($"Correos de reserva #{reservaId} enviados exitosamente a {correoHuesped}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando mensaje de reserva");
            }
        };

        _channel.BasicConsume(queue: "reserva_creada_queue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    private string GenerarHtmlCorreoHuesped(string nombres, string hotel, string fechaEntrada, string fechaSalida, string tipoHabitacion, decimal costoTotal, decimal impuestoTotal, int cantidadHuespedes, int reservaId)
    {
        var totalConImpuesto = costoTotal + impuestoTotal;
        return $@"
        <html>
        <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
            <div style='max-width: 600px; margin: 0 auto; border: 1px solid #ddd; border-radius: 8px; padding: 20px;'>
                <h2 style='color: #2c3e50;'>Confirmación de Reserva</h2>
                <p>Estimado(a) <strong>{nombres}</strong>,</p>
                <p>Nos complace confirmar que su reserva ha sido procesada exitosamente.</p>
                <div style='background: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                    <h3 style='color: #2c3e50; margin-top: 0;'>Detalles de la Reserva</h3>
                    <p><strong>ID de Reserva:</strong> #{reservaId}</p>
                    <p><strong>Hotel:</strong> {hotel}</p>
                    <p><strong>Tipo de Habitación:</strong> {tipoHabitacion}</p>
                    <p><strong>Fecha de Entrada:</strong> {fechaEntrada}</p>
                    <p><strong>Fecha de Salida:</strong> {fechaSalida}</p>
                    <p><strong>Cantidad de Huéspedes:</strong> {cantidadHuespedes}</p>
                </div>
                <div style='background: #e8f4f8; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                    <h3 style='color: #2c3e50; margin-top: 0;'>Resumen de Costos</h3>
                    <p><strong>Costo Base:</strong> ${costoTotal:F2}</p>
                    <p><strong>Impuestos:</strong> ${impuestoTotal:F2}</p>
                    <p style='font-size: 16px; border-top: 2px solid #2c3e50; padding-top: 10px; margin-top: 10px;'>
                        <strong>Total:</strong> ${totalConImpuesto:F2}
                    </p>
                </div>
                <p>Si tiene alguna pregunta o necesita modificar su reserva, no dude en contactarnos.</p>
                <p>¡Esperamos contar con su visita!</p>
                <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; font-size: 12px; color: #666;'>
                    <p>Este es un correo automático. Por favor, no responda directamente. Para asistencia, contacte a nuestro equipo de reservas.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    private string GenerarHtmlCorreoAgente(string nombres, string hotel, string fechaEntrada, string fechaSalida, string tipoHabitacion, int cantidadHuespedes, decimal costoTotal, decimal impuestoTotal, int reservaId)
    {
        var totalConImpuesto = costoTotal + impuestoTotal;
        return $@"
        <html>
        <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
            <div style='max-width: 600px; margin: 0 auto; border: 1px solid #ddd; border-radius: 8px; padding: 20px;'>
                <h2 style='color: #2c3e50;'>Nueva Reserva Confirmada</h2>
                <p>Se ha registrado una nueva reserva en el sistema.</p>
                <div style='background: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                    <h3 style='color: #2c3e50; margin-top: 0;'>Información de la Reserva</h3>
                    <p><strong>ID de Reserva:</strong> #{reservaId}</p>
                    <p><strong>Huésped Principal:</strong> {nombres}</p>
                    <p><strong>Hotel:</strong> {hotel}</p>
                    <p><strong>Tipo de Habitación:</strong> {tipoHabitacion}</p>
                    <p><strong>Fecha de Entrada:</strong> {fechaEntrada}</p>
                    <p><strong>Fecha de Salida:</strong> {fechaSalida}</p>
                    <p><strong>Cantidad de Huéspedes:</strong> {cantidadHuespedes}</p>
                </div>
                <div style='background: #e8f4f8; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                    <h3 style='color: #2c3e50; margin-top: 0;'>Resumen Financiero</h3>
                    <p><strong>Costo Base:</strong> ${costoTotal:F2}</p>
                    <p><strong>Impuestos:</strong> ${impuestoTotal:F2}</p>
                    <p style='font-size: 16px; border-top: 2px solid #2c3e50; padding-top: 10px; margin-top: 10px;'>
                        <strong>Total:</strong> ${totalConImpuesto:F2}
                    </p>
                </div>
            </div>
        </body>
        </html>";
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}