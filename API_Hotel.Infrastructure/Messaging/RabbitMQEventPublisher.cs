using API_Hotel.Application;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API_Hotel.Infrastructure.Messaging
{
    public class RabbitMQEventPublisher : IEventPublisher
    {
        private readonly string _hostname;
        public RabbitMQEventPublisher(IConfiguration configuration)
        {
            _hostname = configuration["RabbitMQ:HostName"] ?? "localhost";
        }
        public void Publish<T>(T @event, string queueName)
        {
            var factory = new ConnectionFactory() { HostName = _hostname };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            // Aseguramos que la cola exista
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            // Convertimos nuestro objeto C# a JSON
            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);
            // Publicamos el mensaje en la cola
            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
