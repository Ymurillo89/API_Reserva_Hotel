using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace API_Hotel.Notifications;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly string _hostname;
    private IConnection _connection;
    private IModel _channel;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _hostname = configuration["RabbitMQ:HostName"] ?? "localhost";

        InicializarRabbitMQ();
    }

    private void InicializarRabbitMQ()
    {
        var factory = new ConnectionFactory { HostName = _hostname };

        // Reintentamos conectar si RabbitMQ aún no ha prendido
        while (_connection == null)
        {
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: "reserva_creada_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                _logger.LogInformation("? Conectado a RabbitMQ exitosamente.");
            }
            catch
            {
                _logger.LogWarning("? Esperando a RabbitMQ...");
                Thread.Sleep(3000);
            }
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("?? Escuchando mensajes de reservas nuevas...");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // Decodificamos el JSON que nos mandó la API de Hoteles
            var reserva = JsonSerializer.Deserialize<JsonElement>(message);
            var correo = reserva.GetProperty("Correo").GetString();
            var nombres = reserva.GetProperty("Nombres").GetString();
            var hotel = reserva.GetProperty("HotelNombre").GetString();

            _logger.LogInformation($"\n============================================");
            _logger.LogInformation($"?? ENVIANDO CORREO A: {correo}");
            _logger.LogInformation($"Estimado(a) {nombres},");
            _logger.LogInformation($"Su reserva en {hotel} ha sido confirmada.");
            _logger.LogInformation($"============================================\n");
        };

        _channel.BasicConsume(queue: "reserva_creada_queue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}